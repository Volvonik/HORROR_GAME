using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class options_menu : MonoBehaviour
{
    [SerializeField] AudioMixer masterAudioMixer;

    public void changegraphics(int value)
    {
        QualitySettings.SetQualityLevel(value);

        print(QualitySettings.GetQualityLevel());
    }

    public void changeresolution(int value)
    {
        if (value == 0)
        {
            Screen.SetResolution(3840, 2160, true);
        }
        else if (value == 1)
        {
            Screen.SetResolution(2560, 1440, true);
        }
        else if(value == 2)
        {
            Screen.SetResolution(1920, 1080, true);
        }
        else if (value == 3)
        {
            Screen.SetResolution(1366, 768, true);
        }

        print(Screen.currentResolution);
    }

    public void changefps(TextMeshProUGUI value)
    {
        if (int.Parse(value.ToString()) > 5)
        {
            Application.targetFrameRate = int.Parse(value.ToString());
        }
        else
        {
            Application.targetFrameRate = 10;
        }

        print(Application.targetFrameRate);
    }

    public void ChangeMasterVolume(float volume)
    {
        masterAudioMixer.SetFloat("masterVolume", volume);
    }
}
