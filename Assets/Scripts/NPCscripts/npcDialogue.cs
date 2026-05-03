using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
    public AudioClip voiceSound;
    public float voicePitch = 1f;

    [Header("Initial Dialogue (plays once)")]
    public string[] introLines;
    public bool[] introAutoProgress;

    [Header("Quest")]
    public bool hasQuest;

    [Header("Quest Active (repeating)")]
    public string[] questActiveLines;
    public bool[] questActiveAutoProgress;

    [Header("Quest Complete (plays once)")]
    public string[] questCompleteLines;
    public bool[] questCompleteAutoProgress;

    [Header("Post-Quest (repeating)")]
    public string[] postQuestLines;
    public bool[] postQuestAutoProgress;
}