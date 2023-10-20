using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_water : MonoBehaviour
{

    Rigidbody2D rb;
    [SerializeField] GameObject creepyBobCutscene;

    [SerializeField] float speed;
    bool disableControls;

    public static Vector2 force;

    public static Vector2 movement;

    float MoveHorizontal;
    float MoveVertical;

    float defaultGravityScale;

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
        force = new Vector2(1, 1);
        //hasFlashlight = true;
        defaultGravityScale = rb.gravityScale;
    }

    void Update()
    {
        MoveHorizontal = Input.GetAxis("Horizontal");
        MoveVertical = Input.GetAxis("Vertical");

        PickFlashLight();
        FlipSprite();
    }
    void FixedUpdate()
    {
        if (disableControls)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            return;
        }
        rb.velocity = new Vector2(speed * MoveHorizontal * force.x, MoveVertical * speed * force.y);
    }

    void PickFlashLight()
    {
        RaycastHit2D flashlightRaycast = Physics2D.Raycast(flashlightCheck.transform.position, -transform.right, raycastLength, flashlightLayer);
        Debug.DrawRay(flashlightCheck.transform.position, -transform.right * raycastLength, Color.green);

        if (flashlightRaycast)
        {
            falshlightText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                hasFlashlight = true;

                Destroy(flashlightRaycast.transform.gameObject);
            }
        }
        else
        {
            if (falshlightText == null) return;

            falshlightText.SetActive(false);
        }

        flashlightLight2D.SetActive(hasFlashlight);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (hasFlashlight)
        {
            spriteRenderer.sprite = hasFlashlightSprite;
            return;
        }

        spriteRenderer.sprite = defaultSprite;
    }

    void FlipSprite()
    {
        bool isRunning =MoveHorizontal != 0;
        if (isRunning)
        {
            transform.rotation = (MoveHorizontal > 0) ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            //transform.localScale = new Vector2(-horizontalInput * currentSize, currentSize);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("CreepyBob"))
        {
            other.enabled = false;

            disableControls = true;
            GameObject prefab = Instantiate(creepyBobCutscene, transform.position, Quaternion.identity);
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<CapsuleCollider2D>().enabled = false;
            flashlightLight2D.SetActive(false);

            if(hasFlashlight)
            {
                prefab.GetComponentInChildren<SpriteRenderer>().sprite = hasFlashlightSprite;
                prefab.GetComponent<Animator>().SetTrigger("creepyBob");
            }
        }
    }

    public void EnableMovementAfterCutscene(GameObject cutscenePlayer)
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<CapsuleCollider2D>().enabled = true;
        flashlightLight2D.SetActive(true);
        disableControls = false;

        transform.position = cutscenePlayer.transform.position;
        transform.rotation = cutscenePlayer.transform.rotation;
    }
}
