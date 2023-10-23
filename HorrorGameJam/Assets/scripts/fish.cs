using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fish : MonoBehaviour
{
    float timer;
    [SerializeField]
    float speed;
    int num = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(num == 0)
        {
            num = Random.Range(3, 8);
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
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if(timer < num / 2)
        {
            rb.velocity = new Vector2(transform.position.x, -speed);
        }
        if (timer > num / 2)
        {
            rb.velocity = new Vector2(transform.position.x, speed);
        }
    }
}
