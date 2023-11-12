using Cinemachine;
using UnityEngine;

public class HazardScript : MonoBehaviour
{
    [SerializeField] Sprite[] hazardSprites;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float rotateSpeed = 3f;

    float xPositionWhenDestroyed = -130f;

    Rigidbody2D rb;

    [SerializeField] AudioClip explosionSFX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        moveSpeed = FindObjectOfType<DifficultyManager>().hazardMoveSpeed[MainMenuScript.difficulty];
    }

    private void Start()
    {
        rb.velocity = new(moveSpeed, 0f);

        int random = Random.Range(0, hazardSprites.Length);
        GetComponent<SpriteRenderer>().sprite = hazardSprites[random];
    }

    private void FixedUpdate()
    {
        transform.Rotate(0f, 0f, rotateSpeed);

        if(transform.position.x > xPositionWhenDestroyed)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            GameObject.Find("AudioManager").GetComponent<AudioSource>().PlayOneShot(explosionSFX, 2f);
            FindObjectOfType<ScreenShakeManager>().CameraShake(GetComponent<CinemachineImpulseSource>());

            other.GetComponent<move_water>().Die();
        }
    }
}
