using UnityEngine;

public class Column : MonoBehaviour
{
    [SerializeField] private int damage;

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            other.gameObject.GetComponent<Damageable>().TakeDamage(damage);
        }
    }
}
