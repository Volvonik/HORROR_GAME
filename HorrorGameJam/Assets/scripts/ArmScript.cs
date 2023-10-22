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
    [SerializeField] float attackSpeed = 3f;
    [SerializeField] float upPushForce = 30;

    [SerializeField] int lifeTotal = 3;
    [SerializeField] float timeToGoUpAfterPunch = 1.5f;
    [SerializeField] float goingUpTime = 3f;

    private void Start()
    {
        Attack();
    }

    private void FixedUpdate()
    {
        if(isAttacking)
        {
            return;
        }

        transform.position = new Vector3(FindObjectOfType<move_water>().transform.position.x, -15, 0);

        timer += Time.deltaTime;
        if (timer > attackDelay)
        {
            timer = 0;
            Attack();
        }
    }

    private void Attack()
    {
        isAttacking = true;
        Debug.Log("IM ATTACKING!! YAAAA");
        //GetComponentInChildren<Animator>().SetBool("punch", true);
        GetComponent<Rigidbody2D>().AddForce(-transform.up * attackSpeed, ForceMode2D.Impulse);
    }

    public void SetPunchToFalse()
    {
        GetComponentInChildren<Animator>().SetBool("punch", false);
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!isAttacking)
        {
            return;
        }
        if (other.CompareTag("Cave"))
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            StartCoroutine(goUpAfterAttack());
        }
    }

    IEnumerator goUpAfterAttack()
    {
        new WaitForSeconds(timeToGoUpAfterPunch);
        GetComponent<Rigidbody2D>().AddForce(transform.up * upPushForce, ForceMode2D.Impulse);

        new WaitForSeconds(goingUpTime);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        isAttacking = false;
        yield return null;
    }

    private void Die()
    {
        Debug.Log("WHEN U DIE");
    }
}
