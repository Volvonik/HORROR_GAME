using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ArmScript : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] float attackDelay = 3f;
    float timer;
    bool isAttacking;
    [SerializeField] float attackSpeed = 3f;
    [SerializeField] float upPushForce = 30;

    [SerializeField] int lifeTotal = 3;
    [SerializeField] float timeToGoUpAfterPunch = 1.5f;
    [SerializeField] float goingUpTime = 3f;

    [SerializeField] AudioClip attackSFX;
    [SerializeField] AudioClip shakeSFX;
    [SerializeField] AudioClip hitSFX;
    [SerializeField] AudioClip dieSFX;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //audioSource.clip = null;
        audioSource.loop = false;
    }

    private void FixedUpdate()
    {
        if(isAttacking)
        {
            return;
        }

        transform.position = new Vector3(FindObjectOfType<move_water>().transform.position.x, -15, 0);

        timer += Time.deltaTime;
        if (timer > attackDelay)
        {
            timer = 0;
            Attack();
        }
    }

    private void Attack()
    {
        isAttacking = true;

        GetComponent<Rigidbody2D>().AddForce(-transform.up * attackSpeed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!isAttacking)
        {
            return;
        }

        if (other.CompareTag("Cave"))
        {
            HitAnObject(attackSFX);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!isAttacking)
        {
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<move_water>().Die();
        }
        else if(other.gameObject.CompareTag("LEGO"))
        {
            GetHit();
            HitAnObject(hitSFX);
            audioSource.PlayOneShot(attackSFX);
        }
    }

    IEnumerator goUpAfterAttack()
    {
        yield return new WaitForSeconds(timeToGoUpAfterPunch);
        GetComponent<Rigidbody2D>().AddForce(transform.up * upPushForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(goingUpTime);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        isAttacking = false;
        //transform.Translate(-3.1f, 0, 0);
        yield return null;
    }

    private void GetHit()
    {
        StartCoroutine(HitVisualEffect());

        lifeTotal--;

        if(lifeTotal <= 0 )
        {
            AudioSource globalAudioSource = GameObject.Find("AudioManager").GetComponent<AudioSource>();
            globalAudioSource.PlayOneShot(dieSFX);
            globalAudioSource.PlayOneShot(hitSFX);
            globalAudioSource.PlayOneShot(shakeSFX);

            FindObjectOfType<ScreenShakeManager>().CameraShake(GetComponent<CinemachineImpulseSource>());

            Destroy(gameObject);
        }
    }

    IEnumerator HitVisualEffect()
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);

        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.2f);

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);

        spriteRenderer.color = Color.white;

        yield return null;
    }

    void HitAnObject(AudioClip sfx)
    {
        if(audioSource == null)
        {
            return;
        }

        audioSource.pitch = 1f;
        audioSource.volume = 0.6f;

        audioSource.PlayOneShot(sfx);
        audioSource.PlayOneShot(shakeSFX);

        FindObjectOfType<ScreenShakeManager>().CameraShake(GetComponent<CinemachineImpulseSource>());
        StartCoroutine(goUpAfterAttack());
    }
}
