using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public float health;
    public float attackRange;
    public LayerMask whatIsEnemy;
    public GameObject keyBoard; //reference to the keyboard

    private void Update()
    {
        //when the player attacks
        if (Input.GetButtonDown("Attack"))
        {
            Attack();
        }
    }

    private void Attack()
    {
        if(keyBoard.activeSelf){
            //get an enemy collider
            Collider2D collider = Physics2D.OverlapCircle(keyBoard.transform.position, attackRange, whatIsEnemy);
            
            //if there is a collider, do damage to it
            if(collider != null){
                Enemy i = collider.gameObject.GetComponent<Enemy>();
                i.TakeDamage(1, 1);
            }
        }
    }

    public void TakeDamage(float d)
    {
        float s = 1;
        health -= s > d ? d : (d - 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
