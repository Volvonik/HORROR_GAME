using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed;
    float horizontalInput;
    [SerializeField] float currentSize = 0.5f;

    [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] GameObject groundCheck;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float coyoteTimeCounter;

    [Header("Gravity")]
    [SerializeField] float velocityWhenIncreasingGravity;
    [SerializeField] float defaultGravityScale = 4f;
    [SerializeField] float fallingGravityScale = 7f;

    [Header("Components")]
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = defaultGravityScale;
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if(Mathf.Abs(rb.velocity.y) < velocityWhenIncreasingGravity)
        {
            rb.gravityScale = fallingGravityScale;
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
        }

        Jump();
        FlipSprite();
    }

    void FixedUpdate()
    {
        rb.velocity = new(horizontalInput * moveSpeed, rb.velocity.y);
    }

    void Jump()
    {
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.05f, groundLayer);

        if(isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            rb.velocity = new(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    void FlipSprite()
    {
        bool isRunning = horizontalInput != 0;
        if (isRunning)
        {
            transform.localScale = new Vector2(-horizontalInput * currentSize, currentSize);
        }
    }
}
