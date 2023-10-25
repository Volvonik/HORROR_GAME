using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class move_water : MonoBehaviour
{
    [Header("Components")]
    AudioSource audioSource;
    AudioSource audioManager;
    Rigidbody2D rb;
    SpriteRenderer sp;
    PanasEManager fishSpawner;
    Animator animator;

    [Header("Movement")]
    bool disableControls;
    [SerializeField] float speed;
    public static Vector2 force;
    Vector2 moveInput;
    [SerializeField] GameObject transition;

    [Header("Cutscenes")]
    [SerializeField] GameObject creepyBobCutscene;
    float defaultGravityScale;
    [SerializeField] float timeUntilScreenShakeInBabyCutscene = 3.8f;
    [SerializeField] AudioClip caveShakeSFX;

    [Header("Audio")]
    [SerializeField] AudioClip BabyRunAway;
    [SerializeField] AudioClip defaultTheme;
    [SerializeField] AudioClip legDropSFX;
    [SerializeField] AudioClip openLegsSFX;
    [SerializeField] AudioClip hitByLegoSFX;
    [SerializeField] AudioClip deathSFX;
    public static AudioClip lastCheckpointMusic;
    bool isPlayingDefaultMusic;
    bool openLegsOnce;

    [Header("Baby Chase")]
    [SerializeField] GameObject tinok;

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

    [SerializeField]
    GameObject baby_scary;

    [Header("Pickup")]
    GameObject pickupObject;
    [SerializeField] GameObject pickupPosition;
    bool delay;

    public bool dinoIsAllowedToFollowPlayer = true; //After that we will create a trigger for dino arena
                                                    //and then it will set to true but now its always true

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        sp = GetComponent<SpriteRenderer>();
        fishSpawner = FindObjectOfType<PanasEManager>();
        animator = GetComponent<Animator>();

        force = new Vector2(0, 0);
        defaultGravityScale = rb.gravityScale;

        hasFlashlight = true;

        if(GameObject.Find("Flashlight") != null && hasFlashlight) //So if you have the flashlight and you die the flashlight at the start still exists
        {
            Destroy(GameObject.Find("Flashlight"));
        }

        //checkpoint.didsave = false;
        if(checkpoint.didsave)
        {
            transform.position = checkpoint.position;

            if(audioManager.isPlaying)
            {
                audioManager.Stop();
            }

            audioManager.clip = lastCheckpointMusic;
            audioManager.Play();

            if(lastCheckpointMusic == defaultTheme)
            {
                isPlayingDefaultMusic = true;
            }
        }
        else
        {
            audioManager.clip = defaultTheme;
            audioManager.Play();
            isPlayingDefaultMusic = true;
        }
    }

    void Update()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal") * speed + force.x, Input.GetAxis("Vertical") * speed + force.y);
        
        if(moveInput == new Vector2(0, 0))
        {
            animator.speed = 0f;
        }
        else
        {
            animator.speed = 1f;
        }

        FindObjectOfType<PauseMenuScript>().isAllowedToPause = !disableControls; //So if ur dead or ur in a cutscene u cant pause

        PickFlashLight();
        FlipSprite();

        if (FindObjectOfType<ArmScript>() == null && !openLegsOnce)
        {
            openLegsOnce = true;

            audioManager.PlayOneShot(openLegsSFX);
            GameObject.Find("Left_Leg").GetComponent<Animator>().SetTrigger("up");
            //GameObject.Find("Right_Leg").GetComponent<Animator>().SetTrigger("up"); //so you will not be able to come back after this
        }

        if(pickupObject == null)
        {
            return;
        }

        pickupObject.transform.position = pickupPosition.transform.position;

        if(Input.GetKeyDown("space") && delay)
        {
            pickupObject.GetComponent<Collider2D>().enabled = true;
            pickupObject = null;
            delay = false;
        }
    }

    void FixedUpdate()
    {
        if (disableControls)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            return;
        }

        rb.velocity = moveInput;
    }

    void PickFlashLight()
    {
        animator.SetBool("hasFlashLight", hasFlashlight);

        if (flashlightCheck == null) return;

        RaycastHit2D flashlightRaycast = Physics2D.Raycast(flashlightCheck.transform.position, -transform.right, raycastLength, flashlightLayer);
        Debug.DrawRay(flashlightCheck.transform.position, -transform.right * raycastLength, Color.green);

        if (flashlightRaycast)
        {
            flashlightText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                hasFlashlight = true;

                audioSource.PlayOneShot(flashLightSFX);

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
    }

    void FlipSprite()
    {
        if(disableControls)
        {
            return;
        }

        bool isRunning = moveInput.x != 0;
        if (isRunning)
        {
            transform.rotation = (moveInput.x > 0) ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
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

            sp.enabled = false;
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

            Invoke("CaveShake", timeUntilScreenShakeInBabyCutscene);
        }

        else if(other.CompareTag("BallPoolSpawn"))
        {
            fishSpawner.stopSpawning = false;
            fishSpawner.inBallsPool = true;
        }

        else if(other.CompareTag("Respawn") || other.CompareTag("Dino"))
        {
            audioSource.PlayOneShot(deathSFX);
            babyd();
        }

        else if(other.CompareTag("BabyRanaway"))
        {
            if(!isPlayingDefaultMusic) { return; }

            audioManager.Stop();
            audioManager.clip = BabyRunAway;
            audioManager.Play();

            isPlayingDefaultMusic = false;
        }

        else if(other.CompareTag("LegoArena"))
        {
            fishSpawner.stopSpawning = true;

            GameObject.Find("Left_Leg").GetComponent<Animator>().SetTrigger("fall");
            GameObject.Find("Right_Leg").GetComponent<Animator>().SetTrigger("fall");

            GameObject.Find("arm_shpitz").GetComponent<AudioSource>().Play();

            Invoke("starthand", 3.2f);

            FindObjectOfType<ScreenShakeManager>().CameraShake(GameObject.Find("Right_Leg").GetComponent<CinemachineImpulseSource>());
            audioManager.PlayOneShot(legDropSFX);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Cave"))
        {
            fishSpawner.stopSpawning = true;
        }

        else if (other.CompareTag("BallPoolSpawn"))
        {
            fishSpawner.inBallsPool = false;
        }

        else if(other.CompareTag("BabyRanaway"))
        {
            if(isPlayingDefaultMusic || disableControls) { return; }

            audioManager.Stop();
            audioManager.PlayOneShot(defaultTheme);
            isPlayingDefaultMusic = true;
        }

        else if(other.gameObject.CompareTag("LegoArena"))
        {
            fishSpawner.inLegoArena = false;
            fishSpawner.stopSpawning = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("LegoArena") && FindObjectOfType<ArmScript>() == null)
        {
            fishSpawner.inLegoArena = true;
            fishSpawner.stopSpawning = false;
        }

        if (other.CompareTag("pickup") && Input.GetKeyDown("space"))
        {
            pickupObject = other.gameObject;
            Invoke("Delay", 0.1f);
            other.gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    void Delay()
    {
        delay = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("LEGO") || other.gameObject.CompareTag("Dino"))
        {
            audioSource.PlayOneShot(deathSFX);
            Die();

            audioSource.PlayOneShot(hitByLegoSFX);
            FindObjectOfType<ScreenShakeManager>().CameraShake(GetComponent<CinemachineImpulseSource>());
        }
        if(other.gameObject.CompareTag("Finish"))
        {
            transition.SetActive(true);
            Invoke("loadscene", 2.1f);
            
            
        }
    }

    public void EnableMovementAfterCutscene(GameObject cutscenePlayer)
    {
        if (hasFlashlight)
        {
            flashlightLight2D.SetActive(true);
        }

        sp.enabled = true;
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
        GameObject hand = FindObjectOfType<ArmScript>().gameObject;

        hand.GetComponent<AudioSource>().loop = false;
        hand.transform.position = new Vector2(transform.position.x, transform.position.y);
        hand.GetComponentInChildren<Animator>().SetTrigger("start");
    }

    public void Die()
    {
        disableControls = true;
        transition.SetActive(true);

        flashlightLight2D.SetActive(false);

        sp.enabled = false;

        //Turns all of the audio sources off
        /*AudioSource[] everyAudioSource = FindObjectsOfType<AudioSource>();
        for (int i = 0; i < everyAudioSource.Length; i++)
        {
            everyAudioSource[i].Stop();
        }*/
    }

    private void babyd()
    {
        disableControls = true;
        baby_scary.SetActive(true);
        Invoke("Die", 2f);
    }

    void CaveShake()
    {
        FindObjectOfType<ScreenShakeManager>().CameraShake(GameObject.Find("Cave").GetComponent<CinemachineImpulseSource>());
    }
    void loadscene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
