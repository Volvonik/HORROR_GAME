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

    [Header("Ball Pool")]
    public bool inBallsPool;
    [SerializeField] float yDistanceInBallsPool;
    [SerializeField] float[] spawnDelaysInBallsPool;
    [SerializeField] float[] xPositionBordersInBallPool;

    private void Start()
    {
        random = Random.Range(spawnDelays[0], spawnDelays[1]);
    }

    private void Update()
    {
        if (stopSpawning)
        {
            timer = 0;
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
        if(FindObjectOfType<move_water>() == null)
        {
            return;
        }

        float currentYPosition = /*FindObjectOfType<move_water>().transform.position.y + */Random.Range(yPositionBorders[0], yPositionBorders[1]);
        float currentXPosition = FindObjectOfType<move_water>().transform.position.x + xPosition;

        if(!inBallsPool)
        {
            random = Random.Range(spawnDelays[0], spawnDelays[1]);

            if (FindAnyObjectByType<move_water>().GetComponent<CapsuleCollider2D>().IsTouchingLayers(LayerMask.GetMask("Cave"))) //Checks to see if the player is within the cave's range
            {
                currentYPosition = Random.Range(caveYPositionBorders[0], caveYPositionBorders[1]);
            }
        }
        else
        {
            random = Random.Range(spawnDelaysInBallsPool[0], spawnDelaysInBallsPool[1]);

            currentYPosition = FindObjectOfType<move_water>().transform.position.y + yDistanceInBallsPool;
            currentXPosition = Random.Range(xPositionBordersInBallPool[0], xPositionBordersInBallPool[1]);
        }

        Vector3 prefabPosition = new Vector3(currentXPosition, currentYPosition, 0f);
        Instantiate(panasE, prefabPosition, Quaternion.identity);

        timer = 0;
    }
}
