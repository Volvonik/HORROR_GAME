using UnityEngine;

public class DinoScript : MonoBehaviour
{
    move_water playerScript;
    Animator animator;
    AudioSource audioSource;

    bool canFollowPlayer;
    bool followPlayer;

    GameObject target;

    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float eatingTime = 3f;

    bool isEating;
    [SerializeField] AudioClip eatingSFX;
    [SerializeField] AudioClip idleSFX;
    [SerializeField] AudioClip runningSFX;

    Vector2 direction;

    bool playerFacingRight;
    bool isDinoFacingLeft;

    void Awake()
    {
        playerScript = FindObjectOfType<move_water>();
        //animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        canFollowPlayer = playerScript.dinoIsAllowedToFollowPlayer;
        if (!canFollowPlayer)
        {
            return;
        }

        if(isEating)
        {
            followPlayer = false;
        }
        else
        {
            followPlayer = !playerScript.isPlayerLookingAtDino;
            //animator.SetBool("isRunning", followPlayer);

            direction = playerScript.gameObject.transform.position - transform.position;
        }

        playerFacingRight = playerScript.transform.rotation.y == 180;

        FollowPlayer();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") || other.CompareTag("Food"))
        {
            isEating = true;
            //animator.SetBool("isEating", true);
            //audioSource.PlayOneShot(eatingSFX);
            Invoke("StopEating", eatingTime);
        }
    }

    void FollowPlayer()
    {
        if(!followPlayer)
        {
            //if(!audioSource.isPlaying)
            //{
            //    audioSource.PlayOneShot(idleSFX);
            //}

            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        if (followPlayer)
        {
            //audioSource.PlayOneShot(runningSFX);
            target = playerScript.gameObject;
            Vector2 finalMoveSpeed = direction.normalized * moveSpeed;
            GetComponent<Rigidbody2D>().velocity = finalMoveSpeed;
        }
    }

    void StopEating()
    {
        isEating = false;
        //animator.SetBool("isEating", false);
    }
}
