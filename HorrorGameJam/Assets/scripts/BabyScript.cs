using UnityEngine;

public class BabyScript : MonoBehaviour
{
    [SerializeField] float moveSpeed = .3f;

    Vector2 direction;
    Vector2 finalMoveSpeed;
    float angle;
    bool disableMovement;

    private void Update()
    {
        if (disableMovement)
        {
            transform.localScale = Vector3.one * 3;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            return;
        }

        direction = (FindObjectOfType<move_water>().gameObject.transform.position - transform.position);
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;

        if(Mathf.Abs(direction.x) > .6f)
        {
            transform.localScale = new Vector2(3, -Mathf.Sign(direction.x) * 3);
        }

    }

    void FixedUpdate()
    {
        if (disableMovement) return;

        float distance = Vector3.Distance(transform.position, FindObjectOfType<move_water>().gameObject.transform.position);
        if(distance < .6f)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            return;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.time / 100);
        finalMoveSpeed = direction.normalized * moveSpeed;
        GetComponent<Rigidbody2D>().velocity = finalMoveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("BallPool"))
        {
            GetComponent<Animator>().SetTrigger("playWithBalls");
            disableMovement = true;

            GameObject.Find("AudioManager").GetComponent<AudioSource>().Stop();
            GameObject.Find("AudioManager").GetComponent<AudioSource>().Play();
        }
    }
}
