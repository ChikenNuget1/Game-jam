using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public float score = 0f;
    public TextMeshProUGUI scoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateUI();
    }
    
    public void addScore(int amount)
    {
        score += amount;
        Debug.Log(score);
        updateUI();
    }

    void updateUI()
    {
        scoreText.SetText(score.ToString());
        
    }
}
