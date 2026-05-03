using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Singleton that tracks the current active quest and completion state.
/// Supports BeatWave and ReachScore quest types.
/// Persists via DontDestroyOnLoad.
///
/// SETUP:
///   - Attach to a GameObject in your first loaded scene.
///   - Create QuestDefinition assets and assign them to the quest NPC's questPool.
///   - Set hubSceneName to "HubArea".
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Scene Names")]
    public string hubSceneName = "HubArea";

    // ── Active quest state ─────────────────────────────────────
    public QuestDefinition activeQuest { get; private set; }
    public bool questComplete { get; private set; } = false;

    // NPC to notify when quest completes
    private HashSet<string> pendingNPCCompletions = new HashSet<string>();

    // ── Fishing unlock (kept from before) ─────────────────────
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

    // ── Called by NPC when a quest is randomly assigned ────────

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

    // ── Called by ScoreManager when score changes ──────────────

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

    /// <summary>
    /// Returns a description of the current quest for use in NPC dialogue.
    /// e.g. "Herd 3 waves of cats!"
    /// </summary>
    public string GetActiveQuestDescription()
    {
        if (activeQuest == null) return "";
        return activeQuest.questDescription;
    }
}