using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;  //the damage the projectile does
    public float destroyTime = 1000f; //the time after which the projectile will be destroyed

    private void Update(){
        destroyTime -= Time.deltaTime;
        if(destroyTime <= 0f){
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            other.gameObject.GetComponent<Damageable>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
