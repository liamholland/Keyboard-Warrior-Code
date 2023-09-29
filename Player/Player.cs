using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float health;
    public float attackRange;
    public LayerMask whatIsEnemy;
    private void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            Attack();
        }
    }

    private void Attack()
    {
        DoDamage(1);
        //ApplyDebuffs
    }

    public void DoDamage(float d)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange, whatIsEnemy);
        foreach(Collider2D collider in colliders)
        {
            BaseEnemy i = collider.gameObject.GetComponent<BaseEnemy>();
            i.TakeDamage(!i.DetectPlayer() ? d*10 : d);
        }
    }

    public void TakeDamage(float d)
    {
        float s = 1;
        health -= s > d ? d : (d - 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
