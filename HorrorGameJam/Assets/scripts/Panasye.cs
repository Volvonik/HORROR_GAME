using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panasye : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    int random;
    private void Start()
    {
        random = Random.Range(0, 2);
    }

    void Update()
    {
        if(FindObjectOfType<PanasEManager>().inBallsPool)
        {
            transform.Translate(transform.up * moveSpeed);

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
        }
        else
        {
            transform.Translate(-transform.right * moveSpeed);
        }

        if(Mathf.Abs(transform.position.x - FindObjectOfType<move_water>().gameObject.transform.position.x) > 20 && transform.position.x < FindObjectOfType<move_water>().gameObject.transform.position.x)
        {
            Destroy(gameObject);
        }
    }
}
