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
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 2)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 2)
        {
            Destroy(gameObject);
        }
    }
}
