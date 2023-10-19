using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_water : MonoBehaviour
{
    [SerializeField]
    float speed;

    public static Vector2 force;

    public static Vector2 movement;
    
    // Start is called before the first frame update
    void Start()
    {
        force = new Vector2(1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();


        float MoveHorizontal = Input.GetAxis("Horizontal");

        float MoveVertical = Input.GetAxis("Vertical");



        rb.velocity = new Vector2(speed * MoveHorizontal * force.x, MoveVertical * speed * force.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("FlashLight"))
        {
            print("got flashlight!");
        }
    }
}
