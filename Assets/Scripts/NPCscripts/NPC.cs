using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")]
    public NPCDialogue dialogueData;
    public string npcID;

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
        if (!questState.introSeen)
        {
            questState.introSeen = true;
            SaveState();
            return dialogueData.hasQuest
                ? CombineLines(dialogueData.introLines, dialogueData.questActiveLines)
                : dialogueData.introLines;
        }

        if (dialogueData.hasQuest)
        {
            if (questState.questComplete && !questState.completionSeen)
                return dialogueData.questCompleteLines;

            if (!questState.questComplete)
                return dialogueData.questActiveLines;
        }

        return dialogueData.postQuestLines;
    }

    bool[] GetCurrentAutoProgress()
    {
        if (!questState.introSeen)
            return dialogueData.introAutoProgress;

        if (dialogueData.hasQuest)
        {
            if (questState.questComplete && !questState.completionSeen)
                return dialogueData.questCompleteAutoProgress;

            if (!questState.questComplete)
                return dialogueData.questActiveAutoProgress;
        }

        return dialogueData.postQuestAutoProgress;
    }

    string[] CombineLines(string[] a, string[] b)
    {
        string[] combined = new string[a.Length + b.Length];
        a.CopyTo(combined, 0);
        b.CopyTo(combined, a.Length);
        return combined;
    }

    bool[] CombineAutoProgress(bool[] a, bool[] b)
    {
        bool[] combined = new bool[a.Length + b.Length];
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

        if (dialogueData.hasQuest && questState.questComplete && !questState.completionSeen)
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