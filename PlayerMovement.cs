using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashForce = 10f;
    public float climbSpeed = 3f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public LayerMask waterLayer;

    private Rigidbody2D rb;
    private PlayerInput input;
    private PlayerAnimationController animator;

    private bool isGrounded;
    private bool isClimbing;
    private bool isDashing = false;
    private float dashTimer;
    private float dashDuration = 0.2f;    
    private int difficultyMultiplier = 1;
    private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    private enum MoveState { Idle, Run, Jump, Dash, Climb}
    private MoveState currentState = MoveState.Idle;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
        animator = GetComponent<PlayerAnimationController>();

        difficultyMultiplier = Mathf.Max(1, GameManager.Instance?.GetDifficulty() ?? 1);
        dashForce /= difficultyMultiplier;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGamePaused) return;

        CheckGrounded();        
        HandleState();
        UpdateAnimator();
        FlipSprite();
        FallWater();

        jumpBufferCounter -= Time.deltaTime;
        if (input.JumpPressed)
        {
            Debug.Log("Space is Pressed");
            jumpBufferCounter = jumpBufferTime;
        }
        
    }

       
    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGamePaused) { return; }
        
        Vector2 currentVel = rb.linearVelocity;


        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f) isDashing = false;
            currentVel.x = Mathf.Sign(currentVel.x) * dashForce;
        }
        else if (isClimbing)
        {
            rb.gravityScale = 0;
            float vertical = Mathf.Abs(input.Vertical) > 0.1f ? input.Vertical * climbSpeed : 0f;
            float horizontal = input.Horizontal * moveSpeed;

            currentVel = new Vector2(horizontal, vertical);
        }

        else
        {
            isClimbing = false;
            rb.gravityScale = 1;
            currentVel.x = input.Horizontal * moveSpeed;

        }

        if (jumpBufferCounter > 0 && isGrounded)
        {
            currentVel.y = jumpForce;
            jumpBufferCounter = 0;
        }

        rb.linearVelocity = currentVel;

        
    }
    
    private void HandleState ()
    {
        if (isClimbing)
        {
            currentState = MoveState.Climb;
        }
        else if (!isGrounded)
        {
            currentState = MoveState.Jump;
        }
        else if (isDashing)
        {
            currentState = MoveState.Dash;
        }
        else if (Mathf.Abs(input.Horizontal) > 0.1f && isGrounded)
        {
            currentState = MoveState.Run;
        }
        else
        {
            currentState = MoveState.Idle;
        }

        if (input.DashPressed && !isDashing && !isClimbing)
        {
            Dash();
        }

    }

    private void Dash()
    {            
        isDashing = true;
        dashTimer = dashDuration;
    }
    private IEnumerator PerformDash()
    {
        isDashing = true;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashForce, rb.linearVelocity.y);
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }
    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void FlipSprite()
    {
        if (input.Horizontal != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(input.Horizontal), 1, 1);
        }
    }
    private void UpdateAnimator()
    {
        animator.SetMovement(input.Horizontal, input.Vertical);
            
        animator.SetBool("IsGrounded", isGrounded);

        if (!isGrounded && rb.linearVelocity.y > 0.1f && !isClimbing)
        {
            animator.SetBool("Jump", true);
        }
        else if (isGrounded || isClimbing)
        {
            animator.SetBool("Jump", false);
        }
        //if (isDashing)
        //{
        //    animator.SetTrigger("Dash");
        //}

        animator.SetBool("IsClimbing", isClimbing);        
    }

    private void FallWater()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, waterLayer))
        {
            Debug.Log("Fell into water. Teleporting.");
            transform.position = new Vector3(transform.position.x - 2f, transform.position.y + 3f, 0f);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Ladder") && Mathf.Abs(input.Vertical) > 0.1f)
        {
            isClimbing = true;
        }        
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = false;
            rb.gravityScale = 1;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    public void TakeDamageAnimation()
    {
        animator.SetHit(true);
        rb.angularVelocity /= 2;
        Invoke(nameof(ResetHit), 0.5f);
    }

    private void ResetHit()
    {
        animator.SetHit(false);
    }
}
