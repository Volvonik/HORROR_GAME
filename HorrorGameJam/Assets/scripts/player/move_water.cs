using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class move_water : MonoBehaviour
{
    [Header("Joystick")]
    [SerializeField] FixedJoystick joystick;
    bool pickedUp = false;
    [SerializeField] GameObject duckButton;
    bool duckPickup;
    [SerializeField] GameObject dropButton;
    bool dropDuck;

    [SerializeField] GameObject transition2;

    [Header("Components")]
    AudioSource audioSource;
    AudioSource audioManager;
    Rigidbody2D rb;
    SpriteRenderer sp;
    PanasEManager fishSpawner;
    Animator animator;

    [Header("Movement")]
    public bool disableControls;
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
    public static AudioClip lastCheckpointMusic;
    [SerializeField] AudioClip dinoMusic;
    bool isPlayingDefaultMusic;
    bool openLegsOnce;

    [SerializeField] AudioSource defaultAudio;
    [SerializeField] AudioSource babyAudio;
    [SerializeField] AudioSource dinoTheSharkAudio;

    [Header("Baby Chase")]
    [SerializeField] GameObject tinok;

    [Header("Flashlight")]
    private static bool hasFlashlight;
    [SerializeField] LayerMask flashlightLayer;
    [SerializeField] float raycastLength;
    [SerializeField] GameObject flashlightCheck;
    [SerializeField] GameObject flashlightButton;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite hasFlashlightSprite;
    [SerializeField] GameObject flashlightLight2D;
    [SerializeField] AudioClip flashLightSFX;  

    [Header("Pickup")]
    GameObject pickupObject;
    [SerializeField] GameObject pickupPosition;
    bool delay;
    public bool dinoIsAllowedToFollowPlayer; //After that we will create a trigger for dino arena

    [Header("Jumpscares")]
    [SerializeField] GameObject baby_scary;
    [SerializeField] Sprite babyImage;
    [SerializeField] Sprite dinoImage;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip babyDeathSFX;
    [SerializeField] AudioClip dinoDeathSFX;

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

        FindObjectOfType<PauseMenuScript>().isAllowedToPause = true;

        //hasFlashlight = true;
        //hasFlashlight = false;

        

        //checkpoint.didsave = false;
        if(checkpoint.didsave)
        {
            transform.position = checkpoint.position;

            audioManager.Stop();
            audioManager.clip = lastCheckpointMusic;
            audioManager.Play();

            //StopAllMusic();
            //lastCheckpointMusic.gameObject.GetComponent<AudioSource>().Play();

            if (lastCheckpointMusic == defaultTheme)
            {
                isPlayingDefaultMusic = true;
            }
        }
        else
        {
            //StopAllMusic();
            //defaultAudio.Play();

            audioManager.Stop();
            audioManager.clip = defaultTheme;
            audioManager.Play();

            isPlayingDefaultMusic = true;

            hasFlashlight = false;
        }

        if (GameObject.Find("Flashlight") != null && hasFlashlight) //So if you have the flashlight and you die the flashlight at the start still exists
        {
            Destroy(GameObject.Find("Flashlight"));
        }
    }

    private void StopAllMusic()
    {
        if(defaultAudio.isPlaying)
        {
            defaultAudio.Stop();
        }
        if(babyAudio.isPlaying)
        {
            babyAudio.Stop();
        }
        if(dinoTheSharkAudio.isPlaying)
        {
            dinoTheSharkAudio.Stop();
        }

    }

    void Update()
    {
        moveInput = new Vector2(joystick.Horizontal * speed + force.x, joystick.Vertical * speed + force.y);
        
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

        if(FindObjectOfType<BabyScript>() != null && FindObjectOfType<BabyScript>().GetComponent<Animator>().GetBool("playBalls") == true)
        {
            if (isPlayingDefaultMusic || disableControls) { return; }

            audioManager.Stop();
            audioManager.clip = defaultTheme;
            audioManager.Play();

            //StopAllMusic();
            //defaultAudio.Play();

            isPlayingDefaultMusic = true;
        }


        if(pickupObject != null)
        {
            pickupObject.transform.position = pickupPosition.transform.position;
            dropButton.SetActive(true);
        }
        else
        {
            dropButton.SetActive(false);
        }

        if(dropDuck && delay)
        {
            dropDuck = false;
            pickupObject.GetComponent<Collider2D>().enabled = true;
            pickupObject = null;
            delay = false;
        }

        if (FindObjectOfType<ArmScript>() == null && !openLegsOnce)
        {
            openLegsOnce = true;

            audioSource.PlayOneShot(openLegsSFX);
            GameObject.Find("Left_Leg").GetComponent<Animator>().SetTrigger("up");
            //GameObject.Find("Right_Leg").GetComponent<Animator>().SetTrigger("up"); //so you will not be able to come back after this

            fishSpawner.inLegoArena = false;
            fishSpawner.stopSpawning = true;

            if (FindObjectOfType<BabyScript>() != null)
            {
                Destroy(FindObjectOfType<BabyScript>().gameObject);
            }

            if (isPlayingDefaultMusic || disableControls) { return; }

            audioManager.Stop();
            audioManager.clip = defaultTheme;
            audioManager.Play();

            //StopAllMusic();
            //defaultAudio.Play();

            isPlayingDefaultMusic = true;
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

    public void DropDuckButton()
    {
        dropDuck = true;
    }

    public void Pickup()
    {
        pickedUp = true;
    }

    void PickFlashLight()
    {
        animator.SetBool("hasFlashLight", hasFlashlight);

        if (flashlightCheck == null) return;

        RaycastHit2D flashlightRaycast = Physics2D.Raycast(flashlightCheck.transform.position, -transform.right, raycastLength, flashlightLayer);
        Debug.DrawRay(flashlightCheck.transform.position, -transform.right * raycastLength, Color.green);

        if (flashlightRaycast)
        {
            flashlightButton.SetActive(true);

            if (pickedUp)
            {
                hasFlashlight = true;

                audioSource.PlayOneShot(flashLightSFX);

                Destroy(flashlightButton);
                Destroy(flashlightCheck);
                Destroy(flashlightRaycast.transform.gameObject);
            }
        }
        else
        {
            if (flashlightButton == null) return;

            flashlightButton.SetActive(false);
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

        else if(other.CompareTag("Respawn"))
        {
            Jumpscare(babyImage, babyDeathSFX);
        }
        else if(other.CompareTag("Dino"))
        {
            Jumpscare(dinoImage, dinoDeathSFX);
        }

        else if(other.CompareTag("LegoArena"))
        {
            other.enabled = false;

            fishSpawner.stopSpawning = true;

            GameObject.Find("Left_Leg").GetComponent<Animator>().SetTrigger("fall");
            GameObject.Find("Right_Leg").GetComponent<Animator>().SetTrigger("fall");

            GameObject.Find("arm_shpitz").GetComponent<AudioSource>().Play();

            Invoke("starthand", 3.2f);

            FindObjectOfType<ScreenShakeManager>().CameraShake(GameObject.Find("Right_Leg").GetComponent<CinemachineImpulseSource>());
            audioSource.PlayOneShot(legDropSFX);

            if (!isPlayingDefaultMusic) { return; }

            if(FindObjectOfType<BabyScript>() != null)
            {
                Destroy(FindObjectOfType<BabyScript>().gameObject);
            }

            audioManager.Stop();
            audioManager.clip = BabyRunAway;
            audioManager.Play();

            //StopAllMusic();
            //babyAudio.Play();

            isPlayingDefaultMusic = false;
        }

        else if(other.CompareTag("DinoArena"))
        {
            dinoIsAllowedToFollowPlayer = true;

            if(audioManager.isPlaying && audioManager.clip == dinoMusic)
            {
                return;
            }
            audioManager.Stop();
            audioManager.clip = dinoMusic;
            audioManager.Play();

            //StopAllMusic();
            //dinoTheSharkAudio.Play();

            isPlayingDefaultMusic = false;
        }

        else if (other.CompareTag("pickup") && pickupObject == null)
        {
            duckButton.SetActive(true);
            //pickupObject = other.gameObject;
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

        
        else if (other.CompareTag("pickup"))
        {
            duckButton.SetActive(false);
            //pickupObject = null;
        }
    }

    public void DuckPickupPressed()
    {
        duckPickup = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("LegoArena") && FindObjectOfType<ArmScript>() == null)
        {
            fishSpawner.inLegoArena = true;
            fishSpawner.stopSpawning = false;
        }

        if (other.CompareTag("pickup") && duckPickup)
        {
            pickupObject = other.gameObject;
            Invoke("Delay", 0.1f);
            other.gameObject.GetComponent<Collider2D>().enabled = false;
            duckPickup = false;
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
            transition2.SetActive(true);
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

        if (!isPlayingDefaultMusic) { return; }

        audioManager.Stop();
        audioManager.clip = BabyRunAway;
        audioManager.Play();

        //StopAllMusic();
        //babyAudio.Play();

        isPlayingDefaultMusic = false;
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

        dinoIsAllowedToFollowPlayer = false;

        //Turns all of the audio sources off
        /*AudioSource[] everyAudioSource = FindObjectsOfType<AudioSource>();
        for (int i = 0; i < everyAudioSource.Length; i++)
        {
            everyAudioSource[i].Stop();
        }*/
    }

    private void Jumpscare(Sprite jumpscareImage, AudioClip sfx)
    {
        disableControls = true;
        baby_scary.GetComponent<AudioSource>().clip = sfx;
        baby_scary.GetComponent<Image>().sprite = jumpscareImage;
        baby_scary.SetActive(true);
        Invoke("Die", 2f);
    }

    void CaveShake()
    {
        audioSource.PlayOneShot(caveShakeSFX);
        FindObjectOfType<ScreenShakeManager>().CameraShake(GameObject.Find("Cave").GetComponent<CinemachineImpulseSource>());
    }
    void loadscene()
    {
        SceneManager.LoadScene(5);
        disableControls = true;
        checkpoint.didsave = false;
        hasFlashlight = false;
        GetComponent<Collider2D>().enabled = false;
    }

    /*public void ResetStats()
    {
        checkpoint.didsave = false;
        hasFlashlight = false;
    }*/
}
