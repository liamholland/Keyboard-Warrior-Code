using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public LayerMask whatIsEnemy;
    public GameObject keyBoard; //reference to the keyboard
    public CameraController cameraController;
    [SerializeField] private int health;
    [SerializeField] private float attackRange;
    [Range(0f, 0.1f)] [SerializeField] private float attackShakeTime;
    [SerializeField] private float attackShakeSpeed;

    private void Update()
    {
        //when the player attacks
        if (Input.GetButtonDown("Attack"))
        {
            Attack();
        }

        //when the player performs a ranged attack / grapple
        if(Input.GetButtonDown("Throw")){
            ThrowKeyBoard();
        }

    }

    //throw the keyboard towards the mouse
    private void ThrowKeyBoard()
    {
        throw new NotImplementedException();
    }

    private void Attack()
    {
        if(keyBoard.activeSelf){
            //get an enemy collider
            Collider2D collider = Physics2D.OverlapCircle(keyBoard.transform.position, attackRange, whatIsEnemy);
            
            //if there is a collider, do damage to it
            if(collider != null){
                StartCoroutine(cameraController.ShakeCamera(attackShakeSpeed, attackShakeTime, new Vector2(0f, 0.2f), new Vector2(transform.localScale.x * 0.8f, 0f)));
                Enemy i = collider.gameObject.GetComponent<Enemy>();
                i.TakeDamage(1, 1);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        
        //if the player loses all its health, kill the player
        if(health <= 0){
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
