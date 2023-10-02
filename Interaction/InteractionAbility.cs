using UnityEngine;
using UnityEngine.UI;

public class InteractionAbility : MonoBehaviour
{
    public float interactionRange;
    public LayerMask whatIsInteractable;
    public Text interactionInstructions;

    void Update()
    {
        //search for interactables each frame
        GameObject g = GetClosestInteractable();

        //set the text of the interaction instructions on the HUD
        interactionInstructions.text = g == null ? "" : g.GetComponent<IObject>().Instructions;

        //if the player is trying to interact, call the interact function on the nearest interactable
        if (g != null && Input.GetButtonDown("Interact")){
            g.GetComponent<IObject>().Do();
        }
    }

    //goes through each of the interactables and returns the closest one
    private GameObject GetClosestInteractable()
    {
        //search for colliders in the area of the player
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange, whatIsInteractable);
        
        float currentSmallest = interactionRange;

        GameObject closest = null;

        //cycle through each collider found and update the closes object if there is one closer than it
        foreach(Collider2D interactable in colliders)
        {
            float distance = Vector2.Distance(transform.position, interactable.gameObject.transform.position);
            if(distance < currentSmallest)
            {
                currentSmallest = distance;
                closest = interactable.gameObject;
            }
        }
        return closest; //return the result
    }

    //visualisation of interaction range
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
