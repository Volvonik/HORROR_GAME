using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanasEManager : MonoBehaviour
{
    [SerializeField] GameObject panasE;
    [SerializeField] float xPosition;
    [SerializeField] float[] yPositionBorders;

    [SerializeField] float[] spawnDelays;
    float timer;
    float random;

    private void Start()
    {
        random = Random.Range(spawnDelays[0], spawnDelays[1]);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > random)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        random = Random.Range(spawnDelays[0], spawnDelays[1]);

        float currentYPosition = Random.Range(yPositionBorders[0], yPositionBorders[1]);
        Vector3 prefabPosition = new Vector3(xPosition, currentYPosition, 0f);
        GameObject prefab = Instantiate(panasE, prefabPosition, Quaternion.identity);

        timer = 0;
    }
}
