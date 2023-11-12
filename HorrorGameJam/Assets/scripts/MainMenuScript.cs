using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject transition;
    [SerializeField]
    GameObject duck;


    [SerializeField] GameObject Diff;

    GameObject globalL;

    private void Start()
    {
        globalL = GameObject.Find("GlobalLight");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && optionsMenu.activeInHierarchy)
        {
            globalL.GetComponent<AudioSource>().Play();
            Back();
        }
    }

    public void Play()
    {
        mainMenu.SetActive(false);
        duck.SetActive(false);
        Diff.SetActive(true);
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
        Diff.SetActive(false);
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
        checkpoint.didsave = false;
    }
    public void RealyPlay()
    {
        transition.SetActive(true);
        Invoke("play2", 2.1f);
    }
}
