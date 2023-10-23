using UnityEngine;

public class DinoScript : MonoBehaviour
{
    move_water playerScript;

    void Awake()
    {
        playerScript = FindObjectOfType<move_water>();
    }
}
