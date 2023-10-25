using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class END : MonoBehaviour
{
    [SerializeField] GameObject transition;
    [SerializeField] GameObject story1;
    [SerializeField] GameObject story2;
    private bool story;
    private bool storyt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(story && storyt)
        {
            story1.SetActive(true);
            storyt = false;
            
            //why no button works?
            if (Input.GetKeyDown(KeyCode.Space) && !storyt)
            {
                print("story2");
                story1.SetActive(false);
                story2.SetActive(true);
                Invoke("Story", 0.2f);
                if(storyt && Input.GetButtonDown("space"))
                {
                    SceneManager.LoadScene(0);
                    print("finish!");
                }
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
        story = true;
        storyt = true;
        print("story");
    }
}
