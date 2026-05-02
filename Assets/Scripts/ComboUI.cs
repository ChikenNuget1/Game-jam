using System.Collections;
using TMPro;
using UnityEngine;

public class ComboUI : MonoBehaviour
{
    public TextMeshProUGUI comboText;

    public IEnumerator ShowCombo (int comboCount)
    {
        if (comboCount == 1) yield break;

        // Colour change based on score
        if (comboCount >= 5)
        {
            comboText.color = Color.purple;
        } else if (comboCount >= 3)
        {
            comboText.color = Color.red;
        }
        else
        {
            comboText.color = Color.white;
        }

        int totalScore = comboCount * comboCount;
        comboText.text = "+" + totalScore + "!";

        comboText.alpha = 1f;
        comboText.transform.localScale = Vector3.one * 0.5f;

        float duration = 0.5f;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float scale = Mathf.Lerp(0.5f, 1.5f, t);
            comboText.transform.localScale = Vector3.one * scale;

            comboText.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }
        comboText.alpha = 0f;
    }
}
