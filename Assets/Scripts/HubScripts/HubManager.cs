using UnityEngine;
using System.Collections.Generic;

public class HubManager : MonoBehaviour
{
    public static HubManager Instance { get; private set; }

    // Your existing hub state
    public Vector3 playerPosition;
    public int hubScore;

    // NPC quest states
    private Dictionary<string, NPCQuestState> npcStates = new Dictionary<string, NPCQuestState>();

    private void Awake()
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

    public void SaveNPCState(NPCQuestState state)
    {
        npcStates[state.npcID] = state;
    }

    public NPCQuestState GetNPCState(string npcID)
    {
        if (npcStates.ContainsKey(npcID))
            return npcStates[npcID];

        // First time seeing this NPC — create fresh state
        var newState = new NPCQuestState(npcID);
        npcStates[npcID] = newState;
        return newState;
    }

    public void SaveHubState(Vector3 position, int score)
    {
        playerPosition = position;
        hubScore = score;
    }
}