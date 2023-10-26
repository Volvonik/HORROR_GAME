using Cinemachine;
using System.Collections;
using UnityEngine;

public class ArmScript : MonoBehaviour
{
    [Header("Components")]
    AudioSource audioSource;

    [Header("Hand Attack")]
    [SerializeField] float attackDelay = 3f;
    float attackTimer;
    bool isAttacking;
    [SerializeField] float attackSpeed = 3f;
    [SerializeField] float upPushForce = 30;
    [SerializeField] float timeToGoUpAfterPunch = 1.5f;
    [SerializeField] float goingUpTime = 3f;

    [Header("Life")]
    [SerializeField] int lifeTotal = 3;

    [Header("Audio")]
    [SerializeField] AudioClip attackSFX;
    [SerializeField] AudioClip shakeSFX;
    [SerializeField] AudioClip hitSFX;
    [SerializeField] AudioClip dieSFX;
    [SerializeField] AudioClip[] throwHazardSFX;

    [Header("Hazard Attack")]
    [SerializeField] float throwHazardDelay = 2f;
    float throwHazardTimer;
    [SerializeField] GameObject hazardPrefab;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
    }

    private void FixedUpdate()
    {

        throwHazardTimer += Time.deltaTime;
        if (throwHazardTimer > throwHazardDelay)
        {
            ThrowHazard();
        }
        if(isAttacking)
        {
            return;
        }

        transform.position = new Vector3(FindObjectOfType<move_water>().transform.position.x, -15, 0);

        attackTimer += Time.deltaTime;
        if (attackTimer > attackDelay)
        {
            Attack();
        }

    }

    private void ThrowHazard()
    {
        throwHazardTimer = 0f;

        int randomSFX = Random.Range(0, throwHazardSFX.Length);
        audioSource.PlayOneShot(throwHazardSFX[randomSFX], 6f);

        Vector3 hazardPosition = new Vector3(FindObjectOfType<move_water>().transform.position.x - 10f, FindObjectOfType<move_water>().transform.position.y, 0f);
        Instantiate(hazardPrefab, hazardPosition, Quaternion.identity);
    }

    private void Attack()
    {
        isAttacking = true;
        attackTimer = 0f;

        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
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

        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        isAttacking = false;

        yield return null;
    }

    private void GetHit()
    {
        StartCoroutine(HitVisualEffect());

        lifeTotal--;

        if(lifeTotal <= 0 )
        {
            AudioSource globalAudioSource = FindObjectOfType<move_water>().GetComponent<AudioSource>();
            globalAudioSource.PlayOneShot(dieSFX);
            globalAudioSource.PlayOneShot(hitSFX);
            globalAudioSource.PlayOneShot(shakeSFX);

            FindObjectOfType<ScreenShakeManager>().CameraShake(GetComponent<CinemachineImpulseSource>());

            Destroy(gameObject);

            if(FindObjectOfType<BabyScript>() != null )
            {
                Destroy(FindObjectOfType<BabyScript>().gameObject);
            }
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
