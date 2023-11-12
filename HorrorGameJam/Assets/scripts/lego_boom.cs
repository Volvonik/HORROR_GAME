using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lego_boom : MonoBehaviour
{
    // Start is called before the first frame update
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("hand"))
        {
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
