using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public float score = 0f;
    public TextMeshProUGUI scoreText;

    void Start()
    {
        updateUI();
    }

    public void addScore(int amount)
    {
        score += amount;
        Debug.Log(score);
        updateUI();

        // ── QUEST HOOK ──────────────────────────────────────────
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnScoreChanged((int)score);
        // ────────────────────────────────────────────────────────
    }

    void updateUI()
    {
        scoreText.SetText(score.ToString());
    }
}