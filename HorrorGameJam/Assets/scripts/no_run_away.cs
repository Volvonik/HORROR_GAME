using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class no_run_away : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(move_water.DC)
        {
            Destroy(gameObject);
        }
    }
}
