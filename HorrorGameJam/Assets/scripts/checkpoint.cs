using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    public static Vector2 position;
    public static bool didsave;
    public AudioClip musicAfterCheckpoint;
    private Animator animator;
    AudioSource ao;

    private void Start()
    {
        animator = GetComponent<Animator>();
        ao = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            didsave = true;
            position = transform.position;
            GetComponent<Animator>().SetTrigger("checkpoint");
            ao.Play();
            move_water.lastCheckpointMusic = musicAfterCheckpoint;
        }
    }
}
