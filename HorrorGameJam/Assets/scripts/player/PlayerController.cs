using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed;
    float horizontalInput;

    [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] GameObject groundCheck;

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

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
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
            transform.localScale = new Vector2(-horizontalInput, 1);
        }
    }
}
