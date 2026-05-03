using UnityEngine;

public enum QuestType
{
    BeatWave,
    ReachScore
}

/// <summary>
/// Define a quest in the Inspector.
/// Create via: right-click in Project → Create → Quest Definition
/// </summary>
[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest Definition")]
public class QuestDefinition : ScriptableObject
{
    [Header("Identity")]
    public string questID;           // e.g. "beat_wave_3", "reach_score_50"
    public string questTitle;        // e.g. "Herd Master"
    public string questDescription;  // shown in dialogue, e.g. "Herd 3 waves of cats!"

    [Header("Type")]
    public QuestType questType;

    [Header("Target (set one based on type)")]
    public int targetWave;           // used if QuestType.BeatWave
    public int targetScore;          // used if QuestType.ReachScore
}