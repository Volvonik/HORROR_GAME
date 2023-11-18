using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class options_menu : MonoBehaviour
{
    [SerializeField] AudioMixer masterAudioMixer;

    public static int graphicsValue = 2;
    private static int resolutionValue = 2;
    private static int fpsValue = 144;
    private static float masterVolumeValue = 0; //max volume
    private static bool once = false;

    [SerializeField] TMP_Dropdown graphicsDropdown;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown difficultyDropdown;
    [SerializeField] TMP_InputField fpsInputField;
    [SerializeField] Slider masterVolumeSlider;

    void OnEnable()
    {
        graphicsDropdown.value = graphicsValue;
        resolutionDropdown.value = resolutionValue;
        fpsInputField.text = fpsValue.ToString();
        masterVolumeSlider.value = masterVolumeValue;
        if(difficultyDropdown != null)
        {
            difficultyDropdown.value = MainMenuScript.difficulty;
        }
    }
    void Start()
    {
        if(!once)
        {
            QualitySettings.SetQualityLevel(2);
            once = true;
        }

        
    }

    public void changegraphics(int value)
    {
        graphicsValue = value;

        QualitySettings.SetQualityLevel(value);
    }

    public void ChangeDifficulty(int value)
    {
        MainMenuScript.difficulty = value;
    }

    public void changeresolution(int value)
    {
        resolutionValue = value;

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
    }

    public void changefps(string value)
    {
        if(!value.All(char.IsDigit)) //So if there's a character in the number it reverts to the most recent digit number that was entered
        {
            fpsInputField.text = fpsValue.ToString();
            return;
        }

        fpsValue = int.Parse(value.ToString());

        if (int.Parse(value.ToString()) >= 10)
        {
            Application.targetFrameRate = int.Parse(value.ToString());
        }
        else
        {
            fpsValue = 10;
            fpsInputField.text = fpsValue.ToString();
            Application.targetFrameRate = 10;
        }

        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            GameObject.Find("GlobalLight").GetComponent<AudioSource>().Play();
        }
        else
        {
            GameObject.Find("QuackSFX").GetComponent<AudioSource>().Play();
        }
    }

    public void ChangeMasterVolume(float volume)
    {
        masterVolumeValue = volume;

        masterAudioMixer.SetFloat("masterVolume", volume);
    }
}
