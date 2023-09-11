using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionAbility : MonoBehaviour
{
    public float interactionRange;
    public LayerMask whatIsInteractable;
    public Text interactionInstructions;

    void Update()
    {
        GameObject g = GetClosestInteractable();

        if (g == null)
            interactionInstructions.text = "";
        else
            interactionInstructions.text = g.GetComponent<IObject>().Instructions;

        if (g != null && Input.GetButtonDown("Interact"))
            g.GetComponent<IObject>().Do();
    }

    private GameObject GetClosestInteractable()
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange, whatIsInteractable);
        float currentSmallest = 5;
        GameObject closest = null;
        foreach(Collider2D interactable in colliders)
        {
            float distance = Vector2.Distance(transform.position, interactable.gameObject.transform.position);
            if(distance < currentSmallest)
            {
                currentSmallest = distance;
                closest = interactable.gameObject;
            }
        }
        return closest;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
