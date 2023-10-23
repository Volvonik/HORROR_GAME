using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fish : MonoBehaviour
{
    float timer;
    [SerializeField]
    float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 6)
        {
            timer = 0;
        }
    }
    void FixedUpdate()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if(timer < 3)
        {
            rb.velocity = new Vector2(transform.position.x, -speed);
        }
        if (timer > 3)
        {
            rb.velocity = new Vector2(transform.position.x, speed);
        }
    }
}
