using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach this to a GameObject in your Hub scene.
/// On Start, it asks QuestManager for any NPCs that need their quest marked complete
/// (because the completion happened while the hub wasn't loaded), finds them in the
/// scene, and calls CompleteQuest() on them so their dialogue updates correctly.
///
/// SETUP:
///   - Attach to any persistent GameObject in the Hub scene.
///   - No Inspector wiring needed — it finds NPCs automatically by npcID.
/// </summary>
public class HubQuestBridge : MonoBehaviour
{
    void Start()
    {
        if (QuestManager.Instance == null) return;

        HashSet<string> pending = QuestManager.Instance.GetAndClearPendingNPCCompletions();

        if (pending.Count == 0) return;

        // Find all NPCs in the scene and complete any that are pending
        NPC[] allNPCs = FindObjectsByType<NPC>(FindObjectsSortMode.None);

        foreach (NPC npc in allNPCs)
        {
            if (pending.Contains(npc.npcID))
            {
                npc.CompleteQuest();
                Debug.Log($"[HubQuestBridge] Completed quest for NPC: {npc.npcID}");
            }
        }
    }
}