using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BabyScript : MonoBehaviour
{
    [SerializeField] float moveSpeed = .3f;

    Vector3 direction;

    private void Update()
    {
        direction = (FindObjectOfType<move_water>().gameObject.transform.position - transform.position).normalized;
    }

    void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, direction.y) * moveSpeed;
    }
}
