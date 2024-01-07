using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class END : MonoBehaviour
{
    [SerializeField] GameObject transition;
    [SerializeField] GameObject transition2;
    [SerializeField] GameObject story1;
    [SerializeField] GameObject story2;

    public bool inStory;
    bool inSecondPage;

    void Update()
    {

        if (Input.GetKeyDown (KeyCode.Space) && inStory)
        {
            if(!inSecondPage) //swap pages
            {
                story1.SetActive(false);
                story2.SetActive(true);
                inSecondPage = true;
            }
            
            else
            {
                transition2.SetActive(true);
                Invoke("end",2.1f);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Finish"))
        {
            transition.SetActive(true);
            Invoke("Story", 2.3f);
        }
    }
    void Story()
    {
        inStory = true;
        story1.SetActive(true);
    }
    private void end()
    {
        // reset all
        FindObjectOfType<PauseMenuScript>().GoToMainMenu();
        SceneManager.LoadScene(0);
    }
}
