using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    public static Vector2 position;
    public static bool didsave;
    public AudioClip musicAfterCheckpoint;
    AudioSource ao;

    [SerializeField] int checkpointNumber;
    public static int checkpointTimes;

    private void Start()
    {
        if(checkpointNumber < checkpointTimes)
        {
            Debug.Log(checkpointNumber + "Was Destroyed");
            if(gameObject.transform.parent != null)
            {
                Destroy(gameObject.transform.parent.gameObject);
                return;
            }

            Destroy(gameObject);
        }

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
            checkpointTimes = checkpointNumber;
        }
    }
}
