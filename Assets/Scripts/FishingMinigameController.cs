using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FishingMinigameController : MonoBehaviour
{
    public GameObject hookPrefab;
    public Transform rodTip;
    public Slider chargeBar;
    public LineRenderer fishingLine;
    public Image fadePanel;

    float force = 0f;
    bool isCharging = false;
    GameObject activeHook;

    void Start()
    {
        fishingLine.positionCount = 2;
        fishingLine.enabled = false;

        Color c = fadePanel.color;
        c.a = 0f;
        fadePanel.color = c;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            isCharging = true;
            force += Time.deltaTime * 20f;
            force = Mathf.Clamp(force, 0, 40);
            chargeBar.value = force;
        }

        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            Toss();
            isCharging = false;
            force = 0;
            chargeBar.value = 0;
        }

        if (activeHook != null && fishingLine.enabled)
        {
            fishingLine.SetPosition(0, rodTip.position);
            fishingLine.SetPosition(1, activeHook.transform.position);
        }
    }

    void Toss()
    {
        activeHook = Instantiate(hookPrefab, rodTip.position, Quaternion.identity);
        Rigidbody2D rb = activeHook.GetComponent<Rigidbody2D>();

        Vector2 tossDirection = new Vector2(0.3f, 2.5f).normalized;
        rb.AddForce(tossDirection * force, ForceMode2D.Impulse);

        fishingLine.enabled = true;
        StartCoroutine(WaitForHookToLand());
    }

    IEnumerator WaitForHookToLand()
    {
        yield return new WaitForSeconds(0.5f);

        while (activeHook != null)
        {
            Rigidbody2D rb = activeHook.GetComponent<Rigidbody2D>();

            if (rb.bodyType == RigidbodyType2D.Static)
            {
                yield return new WaitForSeconds(3f);
                StartCoroutine(FadeAndLoad());
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator FadeAndLoad()
    {
        float duration = 1f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            Color c = fadePanel.color;
            c.a = Mathf.Clamp01(timer / duration);
            fadePanel.color = c;
            yield return null;
        }

        SceneManager.LoadScene("Fishing");
    }
}