using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonHand : MonoBehaviour
{
    GameObject hand;
    [SerializeField] GameObject arm;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            hand = Instantiate(arm, arm.transform.position, Quaternion.identity);
        }
        

        
    }
}
