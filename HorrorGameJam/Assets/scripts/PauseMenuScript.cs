using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject transition;
    [SerializeField] GameObject AudioManager;
    public bool isAllowedToPause = true;
    bool isPaused;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isAllowedToPause || SceneManager.GetActiveScene().buildIndex == 1) //So u'll always be allowed to pause in the happy scene
            {
                if(optionsMenu.activeInHierarchy)
                {
                    BackFromOptions();
                }
                else
                {
                    Pause();
                }
            }
        }
    }

    private void Pause()
    {
        if(!isPaused)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
            isPaused = true;

            if(SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 3) { return; }
            AudioManager.GetComponent<AudioSource>().pitch = 0.5f;

            if(FindObjectsOfType<checkpoint>() == null) { return; }
            checkpoint[] checkpointAudioSources = FindObjectsOfType<checkpoint>();
            foreach(checkpoint audioSource in checkpointAudioSources)
            {
                audioSource.GetComponent<AudioSource>().pitch = 0f;
            }

            if(FindObjectOfType<BabyScript>() == null) { return; }
            FindObjectOfType<BabyScript>().GetComponent<AudioSource>().pitch = 0f;

            if(FindObjectOfType<ArmScript>() == null) { return; }
            FindObjectOfType<ArmScript>().GetComponent<AudioSource>().pitch = 0f;
        }
        else
        {
            Continue();
        }
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        isPaused = false;

        if (SceneManager.GetActiveScene().buildIndex == 1) { return; }
        AudioManager.GetComponent<AudioSource>().pitch = 1f;

        if (FindObjectsOfType<checkpoint>() == null) { return; }
        AudioSource[] checkpointAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in checkpointAudioSources)
        {
            audioSource.pitch = 1f;
        }

        if (FindObjectOfType<BabyScript>() == null) { return; }
        FindObjectOfType<BabyScript>().GetComponent<AudioSource>().pitch = 1f;
        
        if (FindObjectOfType<ArmScript>() == null) { return; }
        FindObjectOfType<ArmScript>().GetComponent<AudioSource>().pitch = 1f;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        transition.SetActive(true);
        Invoke("menu2", 2.1f);
    }
    private void menu2()
    { 
        SceneManager.LoadScene(0);
        checkpoint.didsave = false;
    }

    public void GoToOptions()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void BackFromOptions()
    {
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
}
