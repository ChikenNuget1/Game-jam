using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Singleton that tracks quest progress and unlocks across all scenes.
/// Persists via DontDestroyOnLoad.
///
/// SETUP:
///   - Create an empty GameObject in your first loaded scene, attach this.
///   - Set targetWave in the Inspector.
///   - On your quest NPC, set npcID to "quest_npc_main".
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    // ── Quest ID constants ─────────────────────────────────────
    public const string QUEST_CATCH_FISH = "quest_catch_fish";
    public const string QUEST_BEAT_WAVE = "quest_beat_wave";
    public const string QUEST_UNLOCK_MAP2 = "quest_unlock_map2";

    // ── State ──────────────────────────────────────────────────
    private HashSet<string> activeQuests = new HashSet<string>();
    private HashSet<string> completedQuests = new HashSet<string>();
    private HashSet<string> pendingNPCCompletions = new HashSet<string>();

    public bool fishingMap2Unlocked { get; private set; } = false;

    [Header("Wave Quest Settings")]
    public int targetWave = 3;

    [Header("Scene Names")]
    public string hubSceneName = "HubArea";

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

    // ── Public API ─────────────────────────────────────────────

    public void StartQuest(string questID)
    {
        if (!completedQuests.Contains(questID))
            activeQuests.Add(questID);
    }

    public bool IsQuestActive(string questID) => activeQuests.Contains(questID);
    public bool IsQuestComplete(string questID) => completedQuests.Contains(questID);

    public void CompleteQuest(string questID)
    {
        if (completedQuests.Contains(questID)) return;

        activeQuests.Remove(questID);
        completedQuests.Add(questID);

        Debug.Log($"[QuestManager] Quest completed: {questID}");
        HandleQuestCompletion(questID);
    }

    // ── Called by WaveManager ──────────────────────────────────

    public void OnWaveCompleted(int waveNumber)
    {
        if (!IsQuestComplete(QUEST_BEAT_WAVE) && waveNumber >= targetWave)
        {
            CompleteQuest(QUEST_BEAT_WAVE);

            // Queue NPC update for when hub loads
            pendingNPCCompletions.Add("quest_npc_main");

            // Immediately return to hub
            SceneManager.LoadScene(hubSceneName);
        }
    }

    // ── Called by HubQuestBridge on hub scene load ─────────────

    public HashSet<string> GetAndClearPendingNPCCompletions()
    {
        var pending = new HashSet<string>(pendingNPCCompletions);
        pendingNPCCompletions.Clear();
        return pending;
    }

    // ── Side effects ───────────────────────────────────────────

    void HandleQuestCompletion(string questID)
    {
        switch (questID)
        {
            case QUEST_BEAT_WAVE:
                fishingMap2Unlocked = true;
                StartQuest(QUEST_UNLOCK_MAP2);
                Debug.Log("[QuestManager] Fishing map 2 unlocked!");
                break;

            case QUEST_CATCH_FISH:
                Inventory inv = FindFirstObjectByType<Inventory>();
                if (inv != null && inv.fish != null)
                    inv.AddItem(inv.fish, 1);
                break;
        }
    }
}