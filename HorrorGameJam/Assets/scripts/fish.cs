using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fish : MonoBehaviour
{
    float timer;
    [SerializeField]
    float speed;
    float num = 0;
    Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(num == 0)
        {
            num = Random.Range(1.5f, 3);
        }
        timer += Time.deltaTime;
        if (timer > num)
        {
            timer = 0;
            num = 0;
        }
    }
    void FixedUpdate()
    {
        
        if(timer < num / 2)
        {
            rb.velocity = new Vector2(0, -speed);
        }
        if (timer > num / 2)
        {
            rb.velocity = new Vector2(0, speed);
        }
    }
}
