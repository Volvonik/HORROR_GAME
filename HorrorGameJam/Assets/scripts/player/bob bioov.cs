using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bobbioov : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer SR;
    private bool once = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!once && Input.GetKeyDown("a") || !once && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rb.velocity = new Vector2(-3, 8);
            once = true;
            
        }
        else if (!once && Input.GetKeyDown("d") || !once && Input.GetKeyDown(KeyCode.RightArrow))
        {
            rb.velocity = new Vector2(3,8);
            once = true;
        }
        if (Input.GetKeyDown("a") || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SR.flipX = true;
        }
        else if (Input.GetKeyDown("d") ||  Input.GetKeyDown(KeyCode.RightArrow))
        {
            SR.flipX = false;
        }
        
       
    }
    private void FixedUpdate()
    {
        
       
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("ground"))
        {
            once = false;
            rb.velocity = new Vector2(0, 0);
        }
    }
}
