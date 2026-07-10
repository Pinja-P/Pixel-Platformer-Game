using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public Animator animator;
    public ParticleSystem dustFX;

    // Movement
    public float moveSpeed = 5f;
    float horizontalMovement;
    bool isFacingRight = true;

    // Jumping
    public float jumpPower = 10f;
    public int maxJumps = 2;
    int jumpsRemaining;

    // Wall Movement
    public float wallSlideSpeed = 2f;
    bool isWallSliding;
    // wall jumping
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    // Ground Check
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.04f);
    public LayerMask groundLayer;
    bool isGrounded;

    // Wall Check
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.06f, 0.74f);
    public LayerMask wallLayer;

    // Gravity
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;
    
    void Update()
    {
        // Keeps running these programs
        GroundCheck();
        WallSlide();
        WallJump();
        Gravity();
        
        if (!isWallJumping)
        {   
            // Ads velocity so we can move
            playerRb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, playerRb.linearVelocity.y);
            Flip();
        }

        // gives animation to our actions
        animator.SetFloat("yVelocity", playerRb.linearVelocity.y);
        animator.SetFloat("magnitude", playerRb.linearVelocity.magnitude);
        animator.SetBool("isWallSliding", isWallSliding);

    }

    private void Gravity()
    {
        if (playerRb.linearVelocity.y < 0)
        {
            // Fall speed increases
            playerRb.gravityScale = baseGravity * fallSpeedMultiplier;
            // Caps max fallspeed
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, Mathf.Max(playerRb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            playerRb.gravityScale = baseGravity;
        }
    }

   


    // Movement
    public void Move(InputAction.CallbackContext context)
    {
        // movement left and right
        horizontalMovement = context.ReadValue<Vector2>().x;


    }
    
    // Jump action
    public void Jump(InputAction.CallbackContext context)
    {
        // if player is touching ground
        if (jumpsRemaining > 0)
        {
            // if jump button is held down = full height
            if (context.performed)
            {
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, jumpPower);
                jumpsRemaining--;

                // activates the JumpFX void
                JumpFX();
            }
            // if jump button is only tapped = half the height
            else if (context.canceled)
            {
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, playerRb.linearVelocity.y * 0.5f);
                jumpsRemaining--;

                // activates the JumpFX void
                JumpFX();

            }
        }

        // Wall Jump
        if (context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            // Jump away from the wall
            playerRb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0f;

            // activates the JumpFX void
            JumpFX();

            // Force flip
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
            // wall jump = 0.5f -- Jump again = 0.6f
        }
        
    }

    private void JumpFX()
    {
        // Gives jump its animation
        animator.SetTrigger("jump");

       
        // plays the dust trail animaton
        dustFX.Play();
      
        
    }

    // Wall slide action
    private void WallSlide()
    {
        if (!isGrounded & WallCheck() & horizontalMovement != 0)
        {
            isWallSliding = true;
            // Caps max slide fall speed
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, Mathf.Max(playerRb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    // Wall jump action
    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            // Jumps to the opposite direction
            wallJumpDirection = -transform.localScale.x;
            // Resets jump timer
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    // Makes a check box that we use to check if character is touching stuff
    private void OnDrawGizmosSelected()
    {
        // Ground check box
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        // Wall Check box
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }

    // Define touching ground
    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        
    }

    // Define touching wall
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    // Flips the character when moving left or right
    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;

            if (playerRb.linearVelocity.y == 0)
            {
                dustFX.Play();
            }
        }
    }


}
