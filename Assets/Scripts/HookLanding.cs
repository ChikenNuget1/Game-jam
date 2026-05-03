using UnityEngine;

public class HookLanding : MonoBehaviour
{
    private bool hasLanded = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we hit the WaterSurface
        if (collision.gameObject.name == "WaterSurface" && !hasLanded)
        {
            hasLanded = true;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;        // Stop all movement instantly
            rb.angularVelocity = 0f;            // Stop any rotation
            rb.bodyType = RigidbodyType2D.Static; // Freeze it completely in place
        }
    }
}