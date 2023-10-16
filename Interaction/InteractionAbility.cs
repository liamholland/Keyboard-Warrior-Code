using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionAbility : MonoBehaviour
{
    public float interactionRange;  //the range that the entity can interact with interactables
    public LayerMask whatIsInteractable;    //reference to the interactable layer
    public Animator instructionsAnimator;   //the animator for the instructions box

    [SerializeField] private TextMeshProUGUI interactionInstructions;   //The text element of the box

    [Header("-- Camera Settings-- ")]
    [SerializeField] private float interactionShakeSpeed;   //speed the camera will shake
    [SerializeField] [Range(0f, 0.1f)] private float shakeTime; //the amount of time the shake lasts


    private GameObject closestInteractable; //the closest interactable to the entity
    private GameObject currInteractable;    //the current interactable to show
    private CameraController cameraController;  //reference to the camera controller

    void Start(){
        //get a reference to the camera controller
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    void Update()
    {
        //search for interactables each frame
        closestInteractable = GetClosestInteractable();

        //if the player is trying to interact, call the interact function on the nearest interactable
        if (closestInteractable != null && 
            (closestInteractable.GetComponent<IObject>().UseDefaultInteractButton ? 
                Input.GetButtonDown("Interact") : 
                Input.GetKeyDown(closestInteractable.GetComponent<IObject>().CustomKeyCode))){
            //remove the text
            interactionInstructions.text = "";
            
            //if the closest interactable is showing its instructions
            if(closestInteractable.GetComponent<IObject>().ShowInstructions){
                //animate the interaction
                instructionsAnimator.SetBool("interactionDone", true);
            }

            //apply a camera shake
            StartCoroutine(cameraController.ShakeCamera(interactionShakeSpeed, shakeTime, new Vector2(1, 0), new Vector2(1, 0)));

            //execute the interaction on the object
            closestInteractable.GetComponent<IObject>().Do();

        }

        //if the closest interactable has changed
        if(closestInteractable != currInteractable){
            //update the latest closest interactable
            currInteractable = closestInteractable;

            //register that there is an available interactable with the animator
            instructionsAnimator.SetBool("interactableAvailable", !instructionsAnimator.GetBool("interactableAvailable"));
        }

        //if the instructions box is hidden
        if(instructionsAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hiding")){
            //set the text of the interaction instructions on the HUD
            interactionInstructions.text = closestInteractable == null ? "" : closestInteractable.GetComponent<IObject>().Instructions;
        }

        //if the instructions box is doing the interaction
        if(instructionsAnimator.GetCurrentAnimatorStateInfo(0).IsName("InteractionDone")){
            //stop the animation from repeating
            instructionsAnimator.SetBool("interactionDone", false);

        }
        
        //update the show instructions
        instructionsAnimator.SetBool("showInstructions", closestInteractable == null ? false : closestInteractable.GetComponent<IObject>().ShowInstructions);
    }

    //goes through each of the interactables and returns the closest one
    private GameObject GetClosestInteractable()
    {
        //search for colliders in the area of the player
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange, whatIsInteractable);
        
        float currentSmallest = 1000f;

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
