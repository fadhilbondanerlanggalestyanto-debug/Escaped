using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool wasMoving;
    private SpriteRenderer spriteRenderer;
    private bool isDashing;
    private float dashTime;
    private float dashDirection;

    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
       float moveInput = Input.GetAxis("Horizontal");
       rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
       if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,jumpForce);
        }
        SetAnimation(moveInput);

        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }

        if(Input.GetKeyDown(KeyCode.Q) && !isDashing)
        {
            StartDash();
        }

        if(isDashing)
        {
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed,0);

            dashTime-= Time.deltaTime;

            if (dashTime <= 0)
            {
                isDashing = false;
            }
        }
    }

    private void StartDash()
{
    isDashing = true;
    dashTime = dashDuration;

    if (spriteRenderer.flipX)
        dashDirection = -1;
    else
        dashDirection = 1;
}

   private void FixedUpdate()
{
    isGrounded = Physics2D.OverlapCircle(
        groundCheck.position,
        groundCheckRadius,
        groundLayer
    );

    if (rb.linearVelocity.x != 0)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.right * Mathf.Sign(rb.linearVelocity.x),
            0.5f,
            groundLayer
        );

        if (hit.collider != null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
}

    private void SetAnimation(float moveInput)
    {

        if (isDashing)
        {
            animator.Play("Player_Dash");
            return;
        }

       if(isGrounded)
        {
            if(moveInput != 0)
            {
                animator.Play("Player_Run");
                wasMoving = true;
            }
            else if (wasMoving)
            {
                animator.Play("Player_Stopping");
                wasMoving = false;
            }
        }
        else
        {
            if(rb.linearVelocityY > 0)
            {
                animator.Play("Player_Jump");
            }
            else
            {
                animator.Play("Player_Fall");
            }
        }
    }   

        private void OnCollisionStay2D(Collision2D collision)
    {
    if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
             if (Mathf.Abs(contact.normal.x) > 0.5f)
                {
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                    break;
                }
            }
        }
    }
}
