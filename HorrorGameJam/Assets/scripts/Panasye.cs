using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panasye : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    int random;
    bool goDown;
    private void Start()
    {
        if (FindObjectOfType<PanasEManager>().inBallsPool)
        {
            goDown = true;
            random = Random.Range(0, 2);
        }
    }

    void FixedUpdate()
    {
        if(goDown)
        {
            transform.Translate(transform.up * moveSpeed * 1.5f);

            if(random == 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, -90);
                transform.localScale = new Vector2(-1, 1);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 90);
                transform.localScale = new Vector2(1, 1);
            }

            if(Mathf.Abs(transform.position.y - FindObjectOfType<move_water>().transform.position.y) > 20 && transform.position.y < FindObjectOfType<move_water>().transform.position.y)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.Translate(-transform.right * moveSpeed);
            if (Mathf.Abs(transform.position.x - FindObjectOfType<move_water>().transform.position.x) > 20 && transform.position.x < FindObjectOfType<move_water>().gameObject.transform.position.x)
            {
                Destroy(gameObject);
            }
        }
    }
}
