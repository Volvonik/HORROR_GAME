using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject transition;
    [SerializeField]
    GameObject duck;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && optionsMenu.activeInHierarchy)
        {
            GameObject.Find("GlobalLight").GetComponent<AudioSource>().Play();
            Back();
        }
    }

    public void Play()
    {
        transition.SetActive(true);
        Invoke("play2", 2.1f);
    }

    public void Options()
    {
        mainMenu.SetActive(false);
        duck.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void Back()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
        duck.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
    private void play2()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
