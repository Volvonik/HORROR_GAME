using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class move_water : MonoBehaviour
{
    [SerializeField] GameObject transition;
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
    [SerializeField] AudioClip BabyRunAway;
    [SerializeField] AudioClip defaultTheme;
    [SerializeField] AudioClip legDropSFX;
    bool isPlayingDefaultMusic;

    [Header("Flashlight")]
    private static bool hasFlashlight;
    [SerializeField] LayerMask flashlightLayer;
    [SerializeField] float raycastLength;
    [SerializeField] GameObject flashlightCheck;
    [SerializeField] GameObject flashlightText;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite hasFlashlightSprite;
    [SerializeField] GameObject flashlightLight2D;
    [SerializeField] AudioClip flashLightSFX;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        force = new Vector2(0, 0);
        defaultGravityScale = rb.gravityScale;
        hasFlashlight = true;

        if(GameObject.Find("Flashlight") != null && hasFlashlight) //So if you have the flashlight and you die the flashlight at the start still exists
        {
            Destroy(GameObject.Find("Flashlight"));
        }

        checkpoint.didsave = false;
        if(checkpoint.didsave)
        {
            transform.position = checkpoint.position;

            GameObject.Find("AudioManager").GetComponent<AudioSource>().Stop();
            GameObject.Find("AudioManager").GetComponent<AudioSource>().PlayOneShot(FindObjectOfType<checkpoint>().musicAfterCheckpoint);
        }
        else
        {
            GameObject.Find("AudioManager").GetComponent<AudioSource>().PlayOneShot(defaultTheme);
            isPlayingDefaultMusic = true;
        }
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
       
      
        
        rb.velocity = new Vector2(MoveHorizontal * speed + force.x, MoveVertical * speed + force.y);
    }

    void PickFlashLight()
    {
        if (flashlightCheck == null) return;

        RaycastHit2D flashlightRaycast = Physics2D.Raycast(flashlightCheck.transform.position, -transform.right, raycastLength, flashlightLayer);
        Debug.DrawRay(flashlightCheck.transform.position, -transform.right * raycastLength, Color.green);

        if (flashlightRaycast)
        {
            flashlightText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                hasFlashlight = true;

                GetComponent<AudioSource>().PlayOneShot(flashLightSFX);

                Destroy(flashlightText);
                Destroy(flashlightCheck);
                Destroy(flashlightRaycast.transform.gameObject);
            }
        }
        else
        {
            if (flashlightText == null) return;

            flashlightText.SetActive(false);
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
        bool isRunning = MoveHorizontal != 0;
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

        else if(other.CompareTag("Respawn"))
        {
            disableControls = true;
            transition.SetActive(true);
        }

        else if(other.CompareTag("BabyRanaway"))
        {
            if(!isPlayingDefaultMusic) { return; }

            GameObject.Find("AudioManager").GetComponent<AudioSource>().Stop();
            GameObject.Find("AudioManager").GetComponent<AudioSource>().Play();

            isPlayingDefaultMusic = false;
        }

        else if(other.CompareTag("LegoArena"))
        {
            GameObject.Find("Left_Leg").GetComponent<Animator>().SetTrigger("fall");
            GameObject.Find("Right_Leg").GetComponent<Animator>().SetTrigger("fall");

            GameObject.Find("arm_shpitz").GetComponent<AudioSource>().Play();

            Invoke("starthand", 2.3f);

            FindObjectOfType<ScreenShakeManager>().CameraShake(GameObject.Find("Right_Leg").GetComponent<CinemachineImpulseSource>());
            GetComponent<AudioSource>().PlayOneShot(legDropSFX);
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

        else if(other.CompareTag("BabyRanaway"))
        {
            if(disableControls || isPlayingDefaultMusic) { return; }

            GameObject.Find("AudioManager").GetComponent<AudioSource>().Stop();
            GameObject.Find("AudioManager").GetComponent<AudioSource>().PlayOneShot(defaultTheme);
            isPlayingDefaultMusic = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("LEGO"))
        {
            disableControls = true;
            transition.SetActive(true);
        }
    }

    public void EnableMovementAfterCutscene(GameObject cutscenePlayer)
    {
        if (hasFlashlight)
        {
            flashlightLight2D.SetActive(true);
        }
        GetComponent<SpriteRenderer>().enabled = true;

        GetComponent<CapsuleCollider2D>().enabled = true;

        disableControls = false;

        transform.position = cutscenePlayer.transform.position;
        transform.rotation = cutscenePlayer.transform.rotation;

        rb.gravityScale = defaultGravityScale;

        Instantiate(tinok);
    }

    public void RestartScene()
    {
        disableControls = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void starthand()
    {
        GameObject.Find("arm_shpitz#2").GetComponent<Animator>().SetTrigger("start");
        GameObject.Find("arm_shpitz").GetComponent<AudioSource>().loop = false;
        GameObject.Find("arm_shpitz").transform.position = new Vector2(transform.position.x, transform.position.y);
    }
}
