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

    [Header("Movement")]
    bool disableControls;
    [SerializeField] float speed;
    public static Vector2 force;
    Vector2 moveInput;
    [SerializeField] GameObject transition;

    [Header("Cutscenes")]
    [SerializeField] GameObject creepyBobCutscene;
    float defaultGravityScale;

    [Header("Audio")]
    [SerializeField] AudioClip BabyRunAway;
    [SerializeField] AudioClip defaultTheme;
    [SerializeField] AudioClip legDropSFX;
    [SerializeField] AudioClip openLegsSFX;
    [SerializeField] AudioClip hitByLegoSFX;
    public AudioClip lastCheckpointMusic;
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

    [Header("Dino Raycast")]
    public bool dinoIsAllowedToFollowPlayer;
    [SerializeField] LayerMask dinoLayer;
    public bool isPlayerLookingAtDino;
    [SerializeField] float visionLength;
    [SerializeField] Transform[] visionRotations;

    [SerializeField]
    GameObject baby_scary;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        sp = GetComponent<SpriteRenderer>();
        fishSpawner = FindObjectOfType<PanasEManager>();

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

        PickFlashLight();
        DinoCheck();
        FlipSprite();

        if (FindObjectOfType<ArmScript>() == null && !openLegsOnce)
        {
            openLegsOnce = true;

            audioManager.PlayOneShot(openLegsSFX);
            GameObject.Find("Left_Leg").GetComponent<Animator>().SetTrigger("up");
            GameObject.Find("Right_Leg").GetComponent<Animator>().SetTrigger("up");
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

        if (hasFlashlight)
        {
            sp.sprite = hasFlashlightSprite;
            return;
        }

        sp.sprite = defaultSprite;
    }

    void DinoCheck()
    {
        if(!dinoIsAllowedToFollowPlayer)
        {
            return;
        }

        List<RaycastHit2D> visionRays = new List<RaycastHit2D>();
        visionRays.Clear();

        isPlayerLookingAtDino = false;

        for (int i = 0; i < visionRotations.Length; i++)
        {
            RaycastHit2D visionRay = Physics2D.Raycast(transform.position, visionRotations[i].up, visionLength, dinoLayer);
            visionRays.Add(visionRay);

            Debug.DrawRay(transform.position, visionRotations[i].up * visionLength, Color.red);

            if (visionRay)
            {
                isPlayerLookingAtDino = true;
                Debug.Log("I see dino!");
            }
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
        }

        else if(other.CompareTag("BallPoolSpawn"))
        {
            fishSpawner.stopSpawning = false;
            fishSpawner.inBallsPool = true;
        }

        else if(other.CompareTag("Respawn"))
        {
            babyd();
        }

        else if(other.CompareTag("BabyRanaway"))
        {
            if(!isPlayingDefaultMusic) { return; }

            audioManager.Stop();
            audioManager.Play();

            isPlayingDefaultMusic = false;
        }

        else if(other.CompareTag("LegoArena"))
        {
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
            fishSpawner.stopSpawning = true;
            fishSpawner.inBallsPool = false;
        }

        else if(other.CompareTag("BabyRanaway"))
        {
            if(isPlayingDefaultMusic || disableControls) { return; }

            audioManager.Stop();
            audioManager.PlayOneShot(defaultTheme);
            isPlayingDefaultMusic = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("LEGO"))
        {
            Die();

            audioSource.PlayOneShot(hitByLegoSFX);
            FindObjectOfType<ScreenShakeManager>().CameraShake(GetComponent<CinemachineImpulseSource>());
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
        Time.timeScale = 1f;
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
        Time.timeScale = 0f;
        disableControls = true;
        transition.SetActive(true);

        AudioSource[] everyAudioSource = FindObjectsOfType<AudioSource>();
        for (int i = 0; i < everyAudioSource.Length; i++)
        {
            everyAudioSource[i].Stop();
        }
    }
    private void babyd()
    {
        disableControls = true;
        baby_scary.SetActive(true);
        Invoke("Die", 2f);
    }
}
