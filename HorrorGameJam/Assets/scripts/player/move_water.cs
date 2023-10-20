using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    [SerializeField] GameObject tinok;

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
        force = new Vector2(0, 0);
        hasFlashlight = true;
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
        rb.velocity = new Vector2(speed * MoveHorizontal + force.x, MoveVertical * speed + force.y);
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

                Destroy(flashlightCheck);
            }
        }
        else
        {
            if (falshlightText == null) return;

            falshlightText.SetActive(false);
        }

        if(!disableControls)
        {
            flashlightLight2D.SetActive(hasFlashlight);
        }

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
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("CreepyBob"))
        {
            BoxCollider2D[] colliders = other.gameObject.GetComponents<BoxCollider2D>();
            for(int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }

            disableControls = true;
            GameObject prefab = Instantiate(creepyBobCutscene, transform.position, Quaternion.identity);
            prefab.GetComponent<Animator>().SetTrigger("creepyBob");

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<CapsuleCollider2D>().enabled = false;
            flashlightLight2D.SetActive(false);

            Light2D[] pregabLights = prefab.GetComponentsInChildren<Light2D>();
            if (!hasFlashlight)
            {
                prefab.GetComponentInChildren<SpriteRenderer>().sprite = defaultSprite;

                for(int i = 0; i < pregabLights.Length; i++)
                {
                    pregabLights[i].enabled = false;
                }
            }
            else
            {
                prefab.GetComponentInChildren<SpriteRenderer>().sprite = hasFlashlightSprite;

                for (int i = 0; i < pregabLights.Length; i++)
                {
                    pregabLights[i].enabled = true;
                }
            }
        }

        else if(other.CompareTag("BallPoolSpawn"))
        {
            FindObjectOfType<PanasEManager>().stopSpawning = false;
            FindObjectOfType<PanasEManager>().inBallsPool = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Cave"))
        {
            FindObjectOfType<PanasEManager>().stopSpawning = true;
        }

        else if (other.CompareTag("BallPoolSpawn"))
        {
            FindObjectOfType<PanasEManager>().stopSpawning = true;
            FindObjectOfType<PanasEManager>().inBallsPool = false;
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

        rb.gravityScale = defaultGravityScale;

        Instantiate(tinok);
    }
}
