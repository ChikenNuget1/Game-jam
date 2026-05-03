using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Scene Names")]
    public string hubSceneName = "HubArea";

    // ── Active quest state ─────────────────────────────────────
    public QuestDefinition activeQuest { get; private set; }
    public bool questComplete { get; private set; } = false;

    private HashSet<string> pendingNPCCompletions = new HashSet<string>();

    public bool fishingMap2Unlocked { get; private set; } = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ── Called by NPC ──────────────────────────────────────────

    public void StartQuest(QuestDefinition quest)
    {
        activeQuest = quest;
        questComplete = false;
        Debug.Log($"[QuestManager] Quest started: {quest.questTitle}");
    }

    // ── Called by WaveManager ──────────────────────────────────

    public void OnWaveCompleted(int waveNumber)
    {
        if (activeQuest == null || questComplete) return;
        if (activeQuest.questType != QuestType.BeatWave) return;

        if (waveNumber >= activeQuest.targetWave)
            CompleteActiveQuest();
    }

    // ── Called by ScoreManager ─────────────────────────────────

    public void OnScoreChanged(int currentScore)
    {
        if (activeQuest == null || questComplete) return;
        if (activeQuest.questType != QuestType.ReachScore) return;

        if (currentScore >= activeQuest.targetScore)
            CompleteActiveQuest();
    }

    // ── Internal completion ────────────────────────────────────

    void CompleteActiveQuest()
    {
        questComplete = true;
        fishingMap2Unlocked = true;

        Debug.Log($"[QuestManager] Quest complete: {activeQuest.questTitle}");

        pendingNPCCompletions.Add("quest_npc_main");

        // Clear active quest so NPC assigns a fresh random one next visit
        activeQuest = null;

        SceneManager.LoadScene(hubSceneName);
    }

    // ── Called by HubQuestBridge ───────────────────────────────

    public HashSet<string> GetAndClearPendingNPCCompletions()
    {
        var pending = new HashSet<string>(pendingNPCCompletions);
        pendingNPCCompletions.Clear();
        return pending;
    }

    // ── Helpers ────────────────────────────────────────────────

    public bool HasActiveQuest() => activeQuest != null;

    public string GetActiveQuestDescription()
    {
        if (activeQuest == null) return "";
        return activeQuest.questDescription;
    }
}