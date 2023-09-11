using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public float health;
    public float baseDamage;
    public Player player;
    public Transform playerPosition;
    public bool isMoving;
    public float passiveMovespeed;
    public float hostileMovespeed;
    public float maxMoveDistance;
    public Rigidbody2D enemyRigid;
    public LayerMask whatIsPlayer;
    public float detectionRange;
    public float socialDistancing;
    public float alertTime;

    private Vector2 startPostion;
    private int direction = 1;
    private bool isAlert;
    private float timer;

    private void Awake()
    {
        startPostion = transform.position;
    }

    public bool DetectPlayer()
    {
        RaycastHit2D[] colliders = Physics2D.LinecastAll(transform.position, new Vector2(transform.position.x + (detectionRange * direction), transform.position.y), whatIsPlayer);
        if (colliders.Length > 0)
        {
            timer = Time.time + alertTime;
            return true;
        }
        else if (timer >= Time.time)
            return true;
        else
            return false;
    }

    private void FixedUpdate()
    {
        isAlert = DetectPlayer();
        DoStuff();
    }

    private void DoStuff()
    {
        if (isMoving && !isAlert)
        {
            if (Vector2.Distance(transform.position, startPostion) > maxMoveDistance + 1)
                SetStartPosition();
            PassiveMoveCycle();
        }
        else if (isAlert)
        {
            if (Vector2.Distance(transform.position, playerPosition.position) > socialDistancing)
                HostileMoveCycle();
        }
    }

    private void SetStartPosition()
    {
        startPostion = transform.position;
    }

    private void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void HostileMoveCycle()
    {
        if (playerPosition.position.x > transform.position.x)
            direction = 1;
        else
            direction = -1;

        enemyRigid.velocity = new Vector2(hostileMovespeed * direction, enemyRigid.velocity.y);
    }

    public void TakeDamage(float d)
    {
        health -= d;
    }

    public void DoDamage(float d)
    {
        player.TakeDamage(d);
    }

    private void PassiveMoveCycle()
    {
        if (Vector2.Distance(transform.position, startPostion) >= maxMoveDistance)
            direction *= -1;

        enemyRigid.velocity = new Vector2(passiveMovespeed * direction, enemyRigid.velocity.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (detectionRange * direction), transform.position.y));
    }
}
