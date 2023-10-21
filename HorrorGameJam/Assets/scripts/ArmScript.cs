using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ArmScript : MonoBehaviour
{
    [SerializeField] float attackDelay = 3f;
    float timer;
    bool isAttacking;
    Vector3 position;
    [SerializeField] float attackSpeed = 3f;

    private void FixedUpdate()
    {
        if(isAttacking)
        {
            Attack();
            return;
        }

        transform.position = new Vector3(FindObjectOfType<move_water>().transform.position.x, -15, 0);

        timer += Time.deltaTime;
        if (timer > attackDelay)
        {
            //timer = 0;
            position = transform.position;
            isAttacking = true;
        }
    }

    private void Attack()
    {
        transform.position = Vector3.Lerp(position, new Vector3(position.x, FindObjectOfType<move_water>().transform.position.y, 0), Time.deltaTime * attackSpeed);
        Debug.Log("IM ATTACKING!! YAAAA");
    }
}
