using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBall : MonoBehaviour
{
    [SerializeField] Sprite[] randomSprites;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = randomSprites[Random.Range(0, randomSprites.Length)];
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
    }
}
