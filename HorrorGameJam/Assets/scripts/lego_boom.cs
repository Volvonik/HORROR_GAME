using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lego_boom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("hand"))
        {
            print("ouch im a lego ");
            gameObject.GetComponentInChildren<ParticleSystem>().Play();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            Invoke("destroy", 3);
        }
    }
    private void destroy()
    {
        Destroy(this.gameObject);
    }
}