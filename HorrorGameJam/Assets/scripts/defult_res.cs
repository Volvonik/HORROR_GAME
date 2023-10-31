using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class defult_res : MonoBehaviour
{
    private static bool once;
    // Start is called before the first frame update
    void Start()
    {
        if (!once) { Screen.SetResolution(1920, 1080, true); once = true; }
      
    }

    // Update is called once per frame
    
}
