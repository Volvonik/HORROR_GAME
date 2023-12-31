using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject transition;

    [SerializeField] AudioClip deathSFX;

    [Header("Movement")]
    [SerializeField] float moveSpeed;
    float horizontalInput;
    [SerializeField] Vector3 spawnPoint;

    [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] GameObject groundCheck;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float coyoteTimeCounter;
    [SerializeField] AudioClip jumpSFX;

    [Header("Gravity")]
    [SerializeField] float velocityWhenIncreasingGravity;
    [SerializeField] float defaultGravityScale = 4f;
    [SerializeField] float fallingGravityScale = 7f;

    [Header("Components")]
    Rigidbody2D rb;
    Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = defaultGravityScale;
        animator = GetComponent<Animator>();

        QualitySettings.SetQualityLevel(options_menu.graphicsValue);
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if(horizontalInput == 0 )
        {
            animator.speed = 0f;
        }
        else
        {
            animator.speed = 1f;
        }

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
            coyoteTimeCounter = 0f;
            FindObjectOfType<AudioSource>().PlayOneShot(jumpSFX);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            rb.velocity = new(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ContinueGame"))
        {
            FindObjectOfType<PauseMenuScript>().isAllowedToPause = false;
            transition.SetActive(true);
            Invoke("NextScene", 2);
        }
        else if(other.CompareTag("RestartScene"))
        {
            transform.position = spawnPoint;
            FindObjectOfType<AudioSource>().PlayOneShot(deathSFX);
        }

    }
    private void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
