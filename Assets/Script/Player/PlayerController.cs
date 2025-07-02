using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 10f;

    [Header("Components")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // Input variables
    private Vector2 moveInput;
    private Vector2 currentVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();


        rb.gravityScale = 0f;
        rb.drag = 0f;
    }

    void Update()
    {
        HandleInput();
        HandleSpriteFlip();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleInput()
    {

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;
    }

    void HandleMovement()
    {

        Vector2 targetVelocity = moveInput * moveSpeed;


        float accelRate = (moveInput != Vector2.zero) ? acceleration : deceleration;

        currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity,
                                            accelRate * Time.fixedDeltaTime);


        rb.velocity = currentVelocity;
    }

    void HandleSpriteFlip()
    {

        if (moveInput.x > 0)
            spriteRenderer.flipX = false;
        else if (moveInput.x < 0)
            spriteRenderer.flipX = true;
    }


    public Vector2 GetMoveDirection()
    {
        return moveInput;
    }


    public Vector3 GetPosition()
    {
        return transform.position;
    }
}