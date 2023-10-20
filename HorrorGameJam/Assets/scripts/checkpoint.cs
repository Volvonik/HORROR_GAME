using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    public static Vector2 position;
    public static bool didsave;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Checkpoint");
            didsave = true;
            position = transform.position;
            GetComponent<Animator>().SetTrigger("checkpoint");
        }
    }
}
