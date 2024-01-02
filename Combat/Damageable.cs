using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int health;  //the amount of health the entity has

    //what to do when the entity takes damage
    public virtual void TakeDamage(int damage){
        health -= damage;

        if(health <= 0){
            Destroy(gameObject);
        }
    }
}
