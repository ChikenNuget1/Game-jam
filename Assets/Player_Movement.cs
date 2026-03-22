using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 input = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) input.y += 1;
        if (Keyboard.current.sKey.isPressed) input.y -= 1;
        if (Keyboard.current.aKey.isPressed) input.x -= 1;
        if (Keyboard.current.dKey.isPressed) input.x += 1;

        // Normalize raw input FIRST (fixes diagonal speed)
        input = input.normalized;

        // THEN apply isometric transformation
        movement = new Vector2(
            input.x,
            input.y * 0.5f
        );

        // No second normalize here — it would break the isometric ratio

        if (movement != Vector2.zero)
            lastDirection = movement;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }
}