using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject optionsMenu;
    public bool isAllowedToPause = true;
    bool isPaused;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isAllowedToPause || SceneManager.GetActiveScene().buildIndex == 1) //So u'll always be allowed to pause in the happy scene
            {
                Pause();
            }
            else
            {
                if(!FindObjectOfType<move_water>().GetComponent<SpriteRenderer>().enabled) //That means the player is dead and it will be aquared to show "u cant pause" in that situation
                {
                    return;
                }

                ShowPauseError(); //So it will basically trigger only when in cutscene
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

            GameObject.Find("AudioManager").GetComponent<AudioSource>().pitch = 0.14f;
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

        GameObject.Find("AudioManager").GetComponent<AudioSource>().pitch = 1f;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void GoToOptions()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void BackFromOptions()
    {
        optionsMenu.SetActive(false);
    }

    private void ShowPauseError()
    {
        Debug.Log("Cant Pause");
    }

}
