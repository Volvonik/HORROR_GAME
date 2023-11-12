using System;
using System.Collections;
using UnityEngine;

public class DinoScript : MonoBehaviour
{
    move_water playerScript;
    Animator animator;
    AudioSource audioSource;

    bool canFollowPlayer;
    bool followPlayer;

    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float eatingTime = 3f;

    bool stopMoving;
    [SerializeField] AudioClip eatingSFX;
    [SerializeField] AudioClip runningSFX;

    [SerializeField] Collider2D startEatTrigger;
    [SerializeField] Collider2D actualEatTrigger;

    Vector2 direction;
    float angle;

    bool playerFacingRight;
    bool isPlayerFacingDino;


    [SerializeField] AudioClip duckDieSFX;

    void Awake()
    {
        playerScript = FindObjectOfType<move_water>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        canFollowPlayer = playerScript.dinoIsAllowedToFollowPlayer;
        if (!canFollowPlayer)
        {
            return;
        }

        direction = playerScript.gameObject.transform.position - transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
        playerFacingRight = Mathf.Abs(playerScript.transform.rotation.y) != 0;
        float xDistanceBetweenDinoAndPlayer = playerScript.transform.position.x - transform.position.x; //if its minus the player is to the right of dino
        isPlayerFacingDino = xDistanceBetweenDinoAndPlayer < 0 && playerFacingRight || xDistanceBetweenDinoAndPlayer > 0 && !playerFacingRight;

        float yDistanceBetweenDinoAndPlayer = playerScript.transform.position.y - transform.position.y;

        if(stopMoving)
        {
            followPlayer = false;
            return;
        }
        else
        {
            followPlayer = true;
        }
        /*else if(Mathf.Abs(xDistanceBetweenDinoAndPlayer) < 4f && Mathf.Abs(yDistanceBetweenDinoAndPlayer) > 3f)
        {
            followPlayer = true;
        }
        else
        {
            followPlayer = !isPlayerFacingDino && Mathf.Abs(xDistanceBetweenDinoAndPlayer) < maxXDistanceToFollow;
        }*/

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.time / 100);

        if (!audioSource.isPlaying)
        {
            audioSource.pitch = 1f;
            audioSource.Play();
        }

        FlipSprite(xDistanceBetweenDinoAndPlayer);
    }

    private void FlipSprite(float xDistance)
    {
        if(stopMoving) { return; }

        transform.localScale = new Vector2(3, -Mathf.Sign(direction.x) * 4);
    }

    void FixedUpdate()
    {
        FollowPlayer();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") ||  other.CompareTag("pickup"))
        {
            animator.SetBool("isEating", true);

            //dontCareAboutPlayer = true;

            //Invoke("EnableEatCollider", 0.5f);
        }

        if (other.CompareTag("Food"))
        {
            stopMoving = true;
            StartCoroutine(StopEating(other.gameObject, eatingTime));
        }

        if (startEatTrigger.IsTouchingLayers(LayerMask.GetMask("food")))
        {
            stopMoving = true;
            StartCoroutine(StopEating(other.gameObject, eatingTime));
        }
        if(other.CompareTag("stopshark"))
        {
            stopMoving = true;
            animator.SetBool("isEating", false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(StopEating(other.gameObject, 0f));
            //actualEatTrigger.gameObject.SetActive(false);
        }
        if(other.CompareTag("pickup"))
        {
            animator.SetBool("isEating", false);
        }
    }
    void FollowPlayer()
    {
        if (!followPlayer)
        {
            
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            if (!stopMoving && !animator.GetBool("isEating"))
            {
                animator.speed = 0f;
            }

        }
        else
        {
            Vector2 finalMoveSpeed = direction.normalized * moveSpeed;
            GetComponent<Rigidbody2D>().velocity = finalMoveSpeed;

            animator.speed = 1f;
        }
    }

    IEnumerator StopEating(GameObject other, float time)
    {
        yield return new WaitForSeconds(time);

        //dontCareAboutPlayer = false;

        stopMoving = false;
        animator.SetBool("isEating", false);

        if(other.CompareTag("Food") || other.CompareTag("pickup"))
        {
            Destroy(other.GetComponentInParent<SpriteRenderer>().gameObject);
            FindObjectOfType<move_water>().GetComponent<AudioSource>().PlayOneShot(duckDieSFX);
        }

        yield return null;
    }

    void EnableEatCollider()
    {
        actualEatTrigger.gameObject.SetActive(true);
    }

    public void SetStats(float speed, float eatTime)
    {
        moveSpeed = speed;
        eatingTime = eatTime;
    }
}