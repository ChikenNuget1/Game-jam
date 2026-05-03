using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// NPC — supports dynamic quest descriptions injected into dialogue.
/// In your NPCDialogue asset, use {quest} anywhere in questActiveLines
/// and it will be replaced with the active quest's description at runtime.
///
/// Example questActiveLines entry:
///   "I need your help! {quest} Can you do it?"
/// Becomes at runtime:
///   "I need your help! Herd 3 waves of cats! Can you do it?"
/// </summary>
public class NPC : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")]
    public NPCDialogue dialogueData;
    public string npcID = "quest_npc_main";

    [Header("Quest Pool")]
    public QuestDefinition[] questPool;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;

    [Header("References")]
    public GameObject hotbar;
    public PlayerMovement playerMovement;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    private string[] currentLines;
    private bool[] currentAutoProgress;
    private NPCQuestState questState;

    private void Start()
    {
        questState = HubManager.Instance != null
            ? HubManager.Instance.GetNPCState(npcID)
            : new NPCQuestState(npcID);
    }

    public bool CanInteract() => !isDialogueActive;

    public void Interact()
    {
        if (dialogueData == null) return;

        if (isDialogueActive)
            NextLine();
        else
            StartDialogue();
    }

    void StartDialogue()
    {
        if (playerMovement != null)
            playerMovement.canMove = false;

        isDialogueActive = true;
        dialogueIndex = 0;

        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;
        dialoguePanel.SetActive(true);

        if (hotbar != null)
            hotbar.SetActive(false);

        currentLines = GetCurrentLines();
        currentAutoProgress = GetCurrentAutoProgress();

        StartCoroutine(TypeLine());
    }

    string[] GetCurrentLines()
    {
        // Quest just completed — show completion dialogue
        if (questState.questComplete && !questState.completionSeen)
            return dialogueData.questCompleteLines;

        // No active quest — pick a random one and start it
        if (!QuestManager.Instance.HasActiveQuest())
        {
            AssignRandomQuest();

            if (!questState.introSeen)
            {
                questState.introSeen = true;
                SaveState();
                return InjectQuestDescription(CombineLines(dialogueData.introLines, dialogueData.questActiveLines));
            }

            return InjectQuestDescription(dialogueData.questActiveLines);
        }

        // Quest active — show active lines with description injected
        return InjectQuestDescription(dialogueData.questActiveLines);
    }

    /// <summary>
    /// Replaces {quest} in any dialogue line with the active quest's description.
    /// Also replaces {target} with the numeric target (wave number or score).
    /// </summary>
    string[] InjectQuestDescription(string[] lines)
    {
        if (QuestManager.Instance == null || !QuestManager.Instance.HasActiveQuest())
            return lines;

        QuestDefinition quest = QuestManager.Instance.activeQuest;

        string questDesc = quest.questDescription;
        string target = quest.questType == QuestType.BeatWave
            ? quest.targetWave.ToString()
            : quest.targetScore.ToString();

        string[] injected = new string[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            injected[i] = lines[i]
                .Replace("{quest}", questDesc)
                .Replace("{target}", target);
        }
        return injected;
    }

    void AssignRandomQuest()
    {
        if (questPool == null || questPool.Length == 0)
        {
            Debug.LogWarning($"[NPC] {npcID} has no quests in questPool!");
            return;
        }

        int index = Random.Range(0, questPool.Length);
        QuestDefinition chosen = questPool[index];

        QuestManager.Instance.StartQuest(chosen);

        questState.questComplete = false;
        questState.completionSeen = false;
        SaveState();

        Debug.Log($"[NPC] Assigned quest: {chosen.questTitle}");
    }

    bool[] GetCurrentAutoProgress()
    {
        if (questState.questComplete && !questState.completionSeen)
            return dialogueData.questCompleteAutoProgress;

        if (!questState.introSeen)
            return dialogueData.introAutoProgress;

        return dialogueData.questActiveAutoProgress;
    }

    string[] CombineLines(string[] a, string[] b)
    {
        string[] combined = new string[a.Length + b.Length];
        a.CopyTo(combined, 0);
        b.CopyTo(combined, a.Length);
        return combined;
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(currentLines[dialogueIndex]);
            isTyping = false;
        }
        else if (++dialogueIndex < currentLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach (char letter in currentLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (currentAutoProgress != null
            && currentAutoProgress.Length > dialogueIndex
            && currentAutoProgress[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    public void CompleteQuest()
    {
        questState.questComplete = true;
        SaveState();
    }

    public void EndDialogue()
    {
        StopAllCoroutines();

        if (questState.questComplete && !questState.completionSeen)
        {
            questState.completionSeen = true;
            SaveState();
        }

        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);

        if (hotbar != null) hotbar.SetActive(true);
        if (playerMovement != null) playerMovement.canMove = true;
    }

    void SaveState()
    {
        if (HubManager.Instance != null)
            HubManager.Instance.SaveNPCState(questState);
    }
}