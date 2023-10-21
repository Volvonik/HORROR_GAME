using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    public static Vector2 position;
    public static bool didsave;
    public AudioClip musicAfterCheckpoint;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            didsave = true;
            position = transform.position;
            GetComponent<Animator>().SetTrigger("checkpoint");
            GetComponent<AudioSource>().Play();
        }
    }
}
