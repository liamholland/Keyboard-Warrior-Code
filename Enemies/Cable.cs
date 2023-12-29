using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour
{
    public Vector2 target { get; set; }
    public LayerMask whatDestroysCable; //physics layers that destroy the cable
    [SerializeField] private LineRenderer cableRenderer;
    [SerializeField] private int numDashesToFree;   //the number of times the player has to dash to free themselves

    private bool hitTarget = false; //did the cable hit the target

    private void Update(){
        if(Input.GetButtonDown("Dash")){
            numDashesToFree--;
        }

        if(hitTarget) return;
        else if(Vector2.Distance(transform.position, target) < 0.2f) Destroy(gameObject);
        
        transform.position = Vector2.MoveTowards(transform.position, target, 10f * Time.deltaTime);

        cableRenderer.SetPosition(0, (Vector2)transform.position + new Vector2(Random.Range(0f, 0.5f), Random.Range(0f, 0.5f)));
        cableRenderer.SetPosition(1, (Vector2)transform.position - new Vector2(Random.Range(0f, 0.5f), Random.Range(0f, 0.5f)));
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            //restrict the player
            hitTarget = true;
            StartCoroutine(RestrictPlayer());
        }
        else if(((1 << other.gameObject.layer) & whatDestroysCable) != 0){  //if something that destroys the cable hits it; faster way of checking every value of the layermask against each layer
            //cable missed
            Destroy(gameObject);
        }
    }

    private IEnumerator RestrictPlayer(){
        Controller.isInteracting = true;    //easy way to stop the player from moving

        transform.position = target;
        cableRenderer.SetPosition(0, (Vector2)transform.position + new Vector2(0.5f, 0.5f));
        cableRenderer.SetPosition(1, (Vector2)transform.position - new Vector2(0.5f, 0.5f));
        
        yield return new WaitUntil(() => numDashesToFree <= 0);
        
        Controller.isInteracting = false;   //free the player

        Destroy(gameObject);
    }
}
