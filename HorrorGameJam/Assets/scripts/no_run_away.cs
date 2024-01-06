using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class no_run_away : MonoBehaviour
{
    [SerializeField] EdgeCollider2D edge;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(move_water.DC)
        {
            edge.enabled = false;
        }
    }
}
