using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float chaseSpeed = 2f;
    public float jumpForce = 2f;
    public LayerMask groundLayer;

    private Rigidbody2D enemyRb;
    private bool isGrounded;
    private bool shouldJump;

    public int damage = 1;
    
    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        // is grounded?
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);

        // player direction
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        // player above detection
        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 6f, 1 << player.gameObject.layer);

        if (isGrounded)
        {
            // chase player
            enemyRb.linearVelocity = new Vector2(direction * chaseSpeed, enemyRb.linearVelocity.y);

            // Jump if there's a gap ahead && no ground infront
            // Else if there's player above and platform above

            // If Ground
            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, groundLayer);
            // If gap
            RaycastHit2D gabAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer);
            // If platform above
            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 6f, groundLayer);

            if (!groundInFront.collider && !gabAhead.collider)
            {
                shouldJump = true;
            }
            else if (isPlayerAbove && platformAbove.collider)
            {
                shouldJump = true;
            }
        }

    }

    private void FixedUpdate()
    {
        if (isGrounded && shouldJump)
        {
            shouldJump = false;
            Vector2 direction = (player.position - transform.position).normalized;

            Vector2 jumpDirection = direction * jumpForce;

            enemyRb.AddForce(new Vector2(jumpDirection.x, jumpForce), ForceMode2D.Impulse);
        }
    }
}
