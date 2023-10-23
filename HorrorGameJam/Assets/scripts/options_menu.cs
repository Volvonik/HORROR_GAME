using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class options_menu : MonoBehaviour
{
    int fps;
    [SerializeField]

    string input;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void changegraphics(int value)
    {
        QualitySettings.SetQualityLevel(value);
        print(value);
    }
    public void changeresolution(int value)
    {
        if(value == 0)
        {
            Screen.SetResolution(1920,1080, true);
        }
        if (value == 1)
        {
            Screen.SetResolution(3840,2160, true);
        }
        if (value == 2)
        {
            Screen.SetResolution(2560 , 1440, true);
        }
        if (value == 3)
        {
            Screen.SetResolution(1366 , 768, true);
        }
    }
    public void changefps(string value)
    {
        if (int.Parse(value) > 5)
        {
            Application.targetFrameRate = int.Parse(value);
            print(value);
        }
        else
        {
            Application.targetFrameRate = 10;
        }

        
        
    }
}
