using System;
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

    bool isEating;
    [SerializeField] AudioClip eatingSFX;
    [SerializeField] AudioClip idleSFX;
    [SerializeField] AudioClip runningSFX;

    Vector2 direction;

    bool playerFacingRight;
    bool isPlayerFacingDino;

    float maxXDistanceToFollow = 6f;

    void Awake()
    {
        playerScript = FindObjectOfType<move_water>();
        //animator = GetComponent<Animator>();
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

        if(isEating)
        {
            followPlayer = false;
        }
        else
        {
            followPlayer = !isPlayerFacingDino && Mathf.Abs(xDistanceBetweenDinoAndPlayer) < maxXDistanceToFollow;

            //animator.SetBool("isRunning", followPlayer);

            direction = playerScript.gameObject.transform.position - transform.position;
        }


        FlipSprite();
    }

    private void FlipSprite()
    {
        if (Mathf.Abs(rb.velocity.x) > 0)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x) * 0.8f, transform.localScale.y);
        }
    }

    void FixedUpdate()
    {
        FollowPlayer();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") || other.CompareTag("Food"))
        {
            isEating = true;
            //animator.SetBool("isEating", true);
            //MakeASound(eatingSFX);
            Invoke("StopEating", eatingTime);
        }
    }

    void FollowPlayer()
    {
        if(!followPlayer)
        {
            //MakeASound(idleSFX);

            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        if (followPlayer)
        {
            //MakeASound(runningSFX);

            Vector2 finalMoveSpeed = direction.normalized * moveSpeed;
            GetComponent<Rigidbody2D>().velocity = finalMoveSpeed;
        }
    }

    void StopEating()
    {
        isEating = false;
        //animator.SetBool("isEating", false);
    }

    void MakeASound(AudioClip sfx)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(sfx);
        }
    }
}
