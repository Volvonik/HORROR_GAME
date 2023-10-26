using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex != 2)
        {
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}
