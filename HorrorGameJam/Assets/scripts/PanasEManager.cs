using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

        float currentYPosition = FindObjectOfType<move_water>().transform.position.y + Random.Range(yPositionBorders[0], yPositionBorders[1]);
        Vector3 prefabPosition = new Vector3(FindObjectOfType<move_water>().transform.position.x + xPosition, currentYPosition, 0f);
        GameObject prefab = Instantiate(panasE, prefabPosition, Quaternion.identity);

        timer = 0;
    }
}
