using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class move_water : MonoBehaviour
{
    [SerializeField] GameObject transition2;

    public static bool beatHand;

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
    public static AudioClip lastCheckpointMusic;
    [SerializeField] AudioClip dinoMusic;
    bool isPlayingDefaultMusic;
    public static bool openLegsOnce;

    [SerializeField] AudioSource defaultAudio;
    [SerializeField] AudioSource babyAudio;
    [SerializeField] AudioSource dinoTheSharkAudio;

    [Header("Baby Chase")]
    [SerializeField] GameObject tinok;
    private static bool babySpawned;

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

    [SerializeField] int startDifficulty;

    public static bool DC;

    private void Awake()
    {
        if (startDifficulty >= 0 && startDifficulty < 3)
        {
            MainMenuScript.difficulty = startDifficulty;
        } 
    }

    void Start()
    {
        QualitySettings.SetQualityLevel(options_menu.graphicsValue);

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
        
        if(hasFlashlight)
        {
            flashlightLight2D.SetActive(true);
        }

        if(beatHand)
        {
            Destroy(FindObjectOfType<ArmScript>().gameObject);
            GameObject.Find("Left_Leg").GetComponent<Animator>().SetTrigger("fall");
        }

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
            beatHand = false;
            babySpawned = false;

            checkpoint.checkpointTimes = 0;
        }

        if (GameObject.Find("Flashlight") != null && hasFlashlight) //So if you have the flashlight and you die the flashlight at the start still exists
        {
            Destroy(GameObject.Find("Flashlight"));
        }

        if(openLegsOnce)
        {
            GameObject.Find("Left_Leg").GetComponent<Animator>().SetTrigger("up");
        }

        Time.timeScale = 1;
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

        if(FindObjectOfType<BabyScript>() != null && FindObjectOfType<BabyScript>().GetComponent<Animator>().GetBool("playBalls") == true || FindObjectOfType<BabyScript>() != null && FindObjectOfType<BabyScript>().GetComponent<Animator>().GetBool("playBalls2") == true)
        {
            if (isPlayingDefaultMusic || disableControls) { return; }

            audioManager.Stop();
            audioManager.clip = defaultTheme;
            audioManager.Play();

            //StopAllMusic();
            //defaultAudio.Play();

            isPlayingDefaultMusic = true;

            DC = true;
        }


        if(pickupObject != null)
        {
            pickupObject.transform.position = pickupPosition.transform.position;
        }

        if(Input.GetKeyDown("space") && delay)
        {
            pickupObject.GetComponent<Collider2D>().enabled = true;
            pickupObject = null;
            delay = false;
        }

        if (FindObjectOfType<ArmScript>() == null && !openLegsOnce)
        {
            openLegsOnce = true;

            
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
            if(babySpawned) { return; }

            other.enabled = false;

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

        else if(other.CompareTag("LegoArena") && GameObject.Find("arm_shpitz#2") != null)
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

        
        else if(other.transform.parent != null)
        { 
            if(other.transform.parent.name == "BreadCheckpoint_2")
            {
                babySpawned = true;
            }
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
        if(other.gameObject.CompareTag("LEGO") || other.gameObject.CompareTag("Dino") || other.gameObject.CompareTag("hand"))
        {
            audioSource.PlayOneShot(deathSFX);
            Die();

            audioSource.PlayOneShot(hitByLegoSFX);
            FindObjectOfType<ScreenShakeManager>().CameraShake(GetComponent<CinemachineImpulseSource>());
        }
        if(other.gameObject.CompareTag("Finish"))
        {
            transition2.SetActive(true);
            disableControls = true;
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
        //stop time
        Invoke("StopTime", 0.6f);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        disableControls = true;
        checkpoint.didsave = false;
        hasFlashlight = false;
        GetComponent<Collider2D>().enabled = false;
    }
    private void StopTime()
    {
        Time.timeScale = 0;

    }
}
