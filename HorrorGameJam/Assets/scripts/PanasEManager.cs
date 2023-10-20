using UnityEngine;

public class PanasEManager : MonoBehaviour
{
    [SerializeField] GameObject panasE;
    [SerializeField] float xPosition;
    [SerializeField] float[] yPositionBorders;
    [SerializeField] float[] caveYPositionBorders;

    [SerializeField] float[] spawnDelays;
    float timer;
    float random;

    public bool stopSpawning;

    private void Start()
    {
        random = Random.Range(spawnDelays[0], spawnDelays[1]);
    }

    private void Update()
    {
        if (stopSpawning)
        {
            return;
        }

        timer += Time.deltaTime;
        if(timer > random)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        random = Random.Range(spawnDelays[0], spawnDelays[1]);

        if(FindObjectOfType<move_water>() == null)
        {
            return;
        }

        float currentYPosition = /*FindObjectOfType<move_water>().transform.position.y + */Random.Range(yPositionBorders[0], yPositionBorders[1]);

        if(FindAnyObjectByType<move_water>().GetComponent<CapsuleCollider2D>().IsTouchingLayers(LayerMask.GetMask("Cave"))) //Checks to see if the player is within the cave's range
        {
            currentYPosition = Random.Range(caveYPositionBorders[0], caveYPositionBorders[1]);
        }

        Vector3 prefabPosition = new Vector3(FindObjectOfType<move_water>().transform.position.x + xPosition, currentYPosition, 0f);
        GameObject prefab = Instantiate(panasE, prefabPosition, Quaternion.identity);

        timer = 0;
    }
}
