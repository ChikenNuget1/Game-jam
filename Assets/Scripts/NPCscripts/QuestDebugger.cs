using UnityEngine;

public class QuestDebugger : MonoBehaviour
{
    [Header("NPC References")]
    public NPC npc1;
    public NPC npc2;
    public NPC npc3;
    public NPC npc4;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7)) CompleteQuest(npc1, "NPC1");
        if (Input.GetKeyDown(KeyCode.Alpha8)) CompleteQuest(npc2, "NPC2");
        if (Input.GetKeyDown(KeyCode.Alpha9)) CompleteQuest(npc3, "NPC3");
        if (Input.GetKeyDown(KeyCode.Alpha0)) CompleteQuest(npc4, "NPC4");
    }

    void CompleteQuest(NPC npc, string name)
    {
        if (npc == null)
        {
            Debug.LogWarning($"QuestDebugger: {name} is not assigned!");
            return;
        }

        npc.CompleteQuest();
        Debug.Log($"QuestDebugger: Completed quest for {name}");
    }
}