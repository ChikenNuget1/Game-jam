using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerMovement — updated to drive animations and sprite flipping.
///
/// SETUP:
///   1. Add an Animator component to the player GameObject.
///   2. Create an Animator Controller with these parameters:
///        - isMoving (Bool)
///        - movingUp (Bool)
///   3. Create 4 animation clips and wire them up:
///        - idle_front  : single frame, player-front-stand
///        - idle_back   : single frame, player-back-stand
///        - run_front   : 3 frames, player-front-run sprites
///        - run_back    : 3 frames, player-back-run sprites
///   4. Transition logic in the Animator Controller:
///        - Any State → idle_front  : isMoving = false, movingUp = false
///        - Any State → idle_back   : isMoving = false, movingUp = true
///        - Any State → run_front   : isMoving = true,  movingUp = false
///        - Any State → run_back    : isMoving = true,  movingUp = true
///        (uncheck "Has Exit Time" on all transitions)
///   5. Assign the SpriteRenderer on the player to the spriteRenderer field below.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public bool canMove = true;

    [Header("References")]
    public SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private Vector2 lastDirection = Vector2.down; // default facing front

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!canMove)
        {
            movement = Vector2.zero;
            UpdateAnimator(Vector2.zero);
            return;
        }

        Vector2 input = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) input.y += 1;
        if (Keyboard.current.sKey.isPressed) input.y -= 1;
        if (Keyboard.current.aKey.isPressed) input.x -= 1;
        if (Keyboard.current.dKey.isPressed) input.x += 1;

        input = input.normalized;

        movement = new Vector2(
            input.x,
            input.y * 0.5f
        );

        if (input != Vector2.zero)
            lastDirection = input;

        UpdateAnimator(input);
        UpdateFlip(input);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }

    void UpdateAnimator(Vector2 input)
    {
        if (animator == null) return;

        bool isMoving = input != Vector2.zero;

        // Use lastDirection so idle faces the way you were last moving
        bool movingUp = lastDirection.y > 0;

        animator.SetBool("isMoving", isMoving);
        animator.SetBool("movingUp", movingUp);
    }

    void UpdateFlip(Vector2 input)
    {
        if (spriteRenderer == null) return;

        // Only flip based on horizontal input so vertical movement doesn't reset it
        if (input.x < 0)
            spriteRenderer.flipX = true;
        else if (input.x > 0)
            spriteRenderer.flipX = false;
        // if input.x == 0, keep whatever flip was last set
    }
}