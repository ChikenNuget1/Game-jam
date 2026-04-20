using UnityEngine;
using System.Collections;

public class CircleIndicator : MonoBehaviour
{
    // can fine tune these later
    [Header("Outer Shrinking Circle")]
    public float startRadius = 2f;
    public float shrinkSpeed = 1f;

    [Header("Inner Target Circle")]
    public float innerRadius = 0.15f;

    [Header("Timing")]
    public float timeLimit = 3f; // how long before it disappears and cat escapes

    [Header("References")]
    public LineRenderer outerCircle;
    public LineRenderer innerCircle;

    private float currentRadius;
    private bool active = false;
    private System.Action onSuccess;
    private System.Action onFail;

    public void Initialize(System.Action successCallback, System.Action failCallback)
    {
        onSuccess = successCallback;
        onFail = failCallback;
        currentRadius = startRadius;
        active = true;

        DrawCircle(outerCircle, currentRadius);
        DrawCircle(innerCircle, innerRadius);

        StartCoroutine(TimeLimitCoroutine());
    }

    void Update()
    {
        if (!active) return;

        // shrink outer circle
        currentRadius -= shrinkSpeed * Time.deltaTime;
        DrawCircle(outerCircle, currentRadius);

        // check if outer circle has passed the inner circle (missed)
        if (currentRadius <= innerRadius)
        {
            Fail();
            return;
        }

        // space to catch it
        if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // check if the circles are close enough to count as touching
            float tolerance = 0.1f;
            if (Mathf.Abs(currentRadius - innerRadius) <= tolerance)
                Success();
            else
                Fail();
        }
    }

    void DrawCircle(LineRenderer lr, float radius, int segments = 64)
    {
        lr.positionCount = segments + 1;
        for (int i = 0; i <= segments; i++)
        {
            float angle = 2f * Mathf.PI * i / segments;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            lr.SetPosition(i, new Vector3(x, y, 0f));
        }
    }

    void Success()
    {
        active = false;
        onSuccess?.Invoke();
        Destroy(gameObject);
    }

    void Fail()
    {
        active = false;
        onFail?.Invoke();
        Destroy(gameObject);
    }

    IEnumerator TimeLimitCoroutine()
    {
        yield return new WaitForSeconds(timeLimit);
        if (active) Fail();
    }
}