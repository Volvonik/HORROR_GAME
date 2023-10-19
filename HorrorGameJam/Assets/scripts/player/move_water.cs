using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_water : MonoBehaviour
{

    Rigidbody2D rb;

    [SerializeField]
    float speed;

    public static Vector2 force;

    public static Vector2 movement;

    float MoveHorizontal;
    float MoveVertical;


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
    }

    void Update()
    {
        MoveHorizontal = Input.GetAxis("Horizontal");
        MoveVertical = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(speed * MoveHorizontal * force.x, MoveVertical * speed * force.y);

        PickFlashLight();

        FlipSprite();
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
}
