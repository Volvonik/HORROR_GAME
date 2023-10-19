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

    [Header("Flashlight")]
    bool hasFlashlight;
    [SerializeField] LayerMask flashlightLayer;
    [SerializeField] float raycastLength;
    [SerializeField] GameObject flashlightCheck;
    [SerializeField] GameObject falshlightText;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite hasFlashlightSprite;
    [SerializeField] GameObject flashlightLight2D;

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
        PickFlashLight();
    }

    void PickFlashLight()
    {
        RaycastHit2D flashlightRaycast = Physics2D.Raycast(flashlightCheck.transform.position, -transform.right, raycastLength, flashlightLayer);
        Debug.DrawRay(flashlightCheck.transform.position, -transform.right * raycastLength, Color.green);

        if(flashlightRaycast)
        {
            falshlightText.SetActive(true);

            if(Input.GetKeyDown(KeyCode.Space))
            {
                hasFlashlight = true;
                
                Destroy(flashlightRaycast.transform.gameObject);
            }
        }
        else
        {
            falshlightText.SetActive(false);
        }

        flashlightLight2D.SetActive(hasFlashlight);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if(hasFlashlight)
        {
            spriteRenderer.sprite = hasFlashlightSprite;
            return;
        }

        spriteRenderer.sprite = defaultSprite;
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
            transform.rotation = (horizontalInput > 0) ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            //transform.localScale = new Vector2(-horizontalInput * currentSize, currentSize);
        }
    }
}
