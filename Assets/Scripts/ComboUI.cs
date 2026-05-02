using System.Collections;
using TMPro;
using UnityEngine;

public class ComboUI : MonoBehaviour
{
    public TextMeshProUGUI comboText;

    public IEnumerator ShowCombo (int comboCount)
    {
        comboText.text = "+" + comboCount + "!";

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
