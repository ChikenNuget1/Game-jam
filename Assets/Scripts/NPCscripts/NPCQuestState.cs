using System;

[Serializable]
public class NPCQuestState
{
    public string npcID;
    public bool introSeen;
    public bool questAccepted;
    public bool questComplete;
    public bool completionSeen;

    public NPCQuestState(string id)
    {
        npcID = id;
        introSeen = false;
        questAccepted = false;
        questComplete = false;
        completionSeen = false;
    }
}