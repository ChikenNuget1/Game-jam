using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public float timeRemaining = 60f;
    public bool isRunning = true;

    public TextMeshProUGUI timerText;

    public WaveManager waveManager;

    // Update is called once per frame
    void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            isRunning = false;

            gameOver();
        }
        updateUI();
    }

    public void addTime(float amount)
    {
        timeRemaining += amount;
    }

    void updateUI()
    {
        timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
    }

    void gameOver()
    {
        Debug.Log("Game Over");

        waveManager.enabled = false;
    }
}
