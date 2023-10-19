using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panasye : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    void Update()
    {
        transform.Translate(-transform.right * moveSpeed);

        print(transform.position.x - FindObjectOfType<move_water>().gameObject.transform.position.x);

        if(Mathf.Abs(transform.position.x - FindObjectOfType<move_water>().gameObject.transform.position.x) > 20 && transform.position.x < FindObjectOfType<move_water>().gameObject.transform.position.x)
        {
            Destroy(gameObject);
        }
    }
}
