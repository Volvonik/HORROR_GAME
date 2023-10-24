using System;
using System.Collections;
using UnityEngine;

public class DinoScript : MonoBehaviour
{
    move_water playerScript;
    Animator animator;
    AudioSource audioSource;
    Rigidbody2D rb;

    bool canFollowPlayer;
    bool followPlayer;
    

    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float eatingTime = 3f;
    float eatingTimeCounter;

    bool stopMoving;
    [SerializeField] AudioClip eatingSFX;
    [SerializeField] AudioClip idleSFX;
    [SerializeField] AudioClip runningSFX;

    [SerializeField] Collider2D startEatTrigger;
    [SerializeField] Collider2D actualEatTrigger;

    Vector2 direction;

    bool playerFacingRight;
    bool isPlayerFacingDino;

    [SerializeField] float maxXDistanceToFollow = 6f;

    void Awake()
    {
        playerScript = FindObjectOfType<move_water>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        canFollowPlayer = playerScript.dinoIsAllowedToFollowPlayer;
        if (!canFollowPlayer)
        {
            return;
        }

        playerFacingRight = Mathf.Abs(playerScript.transform.rotation.y) != 0;
        float xDistanceBetweenDinoAndPlayer = playerScript.transform.position.x - transform.position.x; //if its minus the player is to the right of dino
        isPlayerFacingDino = xDistanceBetweenDinoAndPlayer < 0 && playerFacingRight || xDistanceBetweenDinoAndPlayer > 0 && !playerFacingRight;

        float yDistanceBetweenDinoAndPlayer = playerScript.transform.position.y - transform.position.y;

        if(stopMoving)
        {
            followPlayer = false;
        }

        else if(xDistanceBetweenDinoAndPlayer < 4f && Mathf.Abs(yDistanceBetweenDinoAndPlayer) > 3f)
        {
            followPlayer = true;
        }

        else
        {
            direction = playerScript.gameObject.transform.position - transform.position;

            followPlayer = !isPlayerFacingDino && Mathf.Abs(xDistanceBetweenDinoAndPlayer) < maxXDistanceToFollow;
        }

        FlipSprite();
    }

    private void FlipSprite()
    {
        if (Mathf.Abs(rb.velocity.x) > 0)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x) * -4f, transform.localScale.y);
        }
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
            //MakeASound(eatingSFX);

            Invoke("EnableEatCollider", 0.5f);
        }

        if (actualEatTrigger.IsTouchingLayers(LayerMask.GetMask("food")))
        {
            stopMoving = true;
            StartCoroutine(StopEating(other.gameObject, eatingTime));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(StopEating(other.gameObject, 0f));
            actualEatTrigger.gameObject.SetActive(false);
        }
    }
    void FollowPlayer()
    {
        if (!followPlayer)
        {
            //MakeASound(idleSFX);

            GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            if (!stopMoving)
            {
                animator.speed = 0f;
            }

        }

        if (followPlayer)
        {
            //MakeASound(runningSFX);

            Vector2 finalMoveSpeed = direction.normalized * moveSpeed;
            GetComponent<Rigidbody2D>().velocity = finalMoveSpeed;

            animator.speed = 1f;
        }
    }

    IEnumerator StopEating(GameObject other, float time)
    {
        yield return new WaitForSeconds(time);

        stopMoving = false;
        animator.SetBool("isEating", false);

        if(other.CompareTag("Food") || other.CompareTag("pickup"))
        {
            Destroy(other.transform.parent.gameObject);
        }

        yield return null;
    }

    void EnableEatCollider()
    {
        actualEatTrigger.gameObject.SetActive(true);
    }

    void MakeASound(AudioClip sfx)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(sfx);
        }
    }
}