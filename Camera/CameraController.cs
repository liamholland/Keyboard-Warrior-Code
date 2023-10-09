using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float cameraSpeed;
    public float shakeSpeed; //speed of the camera when shaking
    public float yOffset;   //the distance the camera is above the player
    [Range(0f, 10f)] public float xDeadZone;
    [Range(0f, 10f)] public float yDeadZone;

    [SerializeField] private bool ignoreDeadZone = false;  //should the camera use the deadzone
    [SerializeField] private float xCatchUpMultiplier;  //multiplier applied to cameraSpeed when player is outside the deadzone
    [SerializeField] private float yCatchUpMultiplier;  //multiplier applied to cameraSpeed when player is outside the deadzone
    [SerializeField] [Range(0f, 0.1f)] private float shakeTime;    //the longest time the camera can shake for at once

    private bool outOfBoundsX = false;  //has the player gone beyond the the horizontal bounds
    private bool outOfBoundsY = false;  //has the target gone beyond the vertical bounds
    private bool isShaking = false; //is the camera shaking
    private Vector2 target; //the target position that the camera tracks to

    // Start is called before the first frame update
    void Start()
    {
        //start off with the target as the player
        target = player.transform.position;

        //set the position of the camera to be the same as the player's
        transform.position = new Vector3(target.x, target.y + yOffset, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isShaking){
            //set the target to the player
            target = player.transform.position;
        }

        //track the target
        TrackTarget();

    }

    //track the target position of the camera
    private void TrackTarget(){
        float desiredX = target.x;

        float xDiff = Mathf.Abs(desiredX - transform.position.x);    //difference between the player and camera x position

        //if the player has moved out of the x deadzone bounds or the deadzone isn't being used
        if(xDiff > xDeadZone || outOfBoundsX || ignoreDeadZone){
            outOfBoundsX = true;
            
            float speed;

            if(isShaking){
                //if the camera is shaking, apply the shake speed
                speed = shakeSpeed;
            }
            else{
                //if the camera is out of the deadzone, apply a multiplier to catch up
                speed = xDiff > xDeadZone ? cameraSpeed * xCatchUpMultiplier : cameraSpeed;
            }

            //interpolate between the current x value and the desired x value quickly
            float smoothX = Mathf.Lerp(transform.position.x, desiredX, speed * Time.deltaTime);

            //set the position of the camera
            transform.position = new Vector3(smoothX, transform.position.y, transform.position.z);

            if(Mathf.Abs(transform.position.x - desiredX) < 0.1f){
                outOfBoundsX = false;
            }
        }

        float desiredY = target.y;

        float yDiff = Mathf.Abs(desiredY - transform.position.y);    //difference between the player and the camera y position

        //if the player has moved out of the y deadzone bounds
        if(yDiff > yDeadZone || outOfBoundsY || ignoreDeadZone){
            outOfBoundsY = true;

            float speed;

            if(isShaking){
                speed = shakeSpeed;
            }
            else{
                //if the camera is out of the deadzone, apply a multiplier to catch up
                speed = yDiff > yDeadZone ? cameraSpeed * yCatchUpMultiplier : cameraSpeed;
            }

            //interpolate between the current y position and the player's position
            float smoothY = Mathf.Lerp(transform.position.y, desiredY, speed * Time.deltaTime);

            //set the position of the camera
            transform.position = new Vector3(transform.position.x, smoothY, transform.position.z);

            if(Mathf.Abs(transform.position.y - desiredY) < 0.1f){
                outOfBoundsY = false;
            }
        }
    }

    /// <summary>
    /// Attempts to apply a shake effect to the camera. The duration of the shake is controlled by the camera itself
    /// </summary>
    /// <param name="negativeShake">The magnitude vector negatively applied to the current target of the camera - applied first</param>
    /// <param name="positiveShake">The magnitude vector positively applied to the current target of the camera - applied second</param>
    /// <param name="startAfter">The length of time to wait before starting the shake effect</param>
    public void ShakeCamera(Vector2 negativeShake, Vector2 positiveShake){
        if(!isShaking){
            StartCoroutine(CameraShake(negativeShake, positiveShake));   //start the coroutine
        }
    }

    //a private coroutine that handles the shake effect
    private IEnumerator CameraShake(Vector2 negativeShake, Vector2 positiveShake){
        isShaking = true;   //the camera is now shaking

        ignoreDeadZone = true;  //ignore the deadzone

        Vector2 targetBeforeShake = target; //record the current target

        //the first shake position uses the negative vector
        Vector2 firstShakePosition = new Vector2(transform.position.x - negativeShake.x, transform.position.y - negativeShake.y);

        //the second shake position uses the positive vector
        Vector2 secondShakePosition = new Vector2(transform.position.x + positiveShake.x, transform.position.y + positiveShake.y);

        float elapsedTime = 0f;

        while(elapsedTime < shakeTime){
            target = firstShakePosition;    //set the target of the camera to the first position

            yield return new WaitUntil(() => Vector2.Distance(target, transform.position) < 0.1f);

            target = secondShakePosition;   //set the target to the second shake position

            yield return new WaitUntil(() => Vector2.Distance(target, transform.position) < 0.1f);

            elapsedTime += Time.fixedDeltaTime;
        }

        ignoreDeadZone = false; //stop ignoring the deadzone

        isShaking = false;  //no longer shaking

        target = targetBeforeShake;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(xDeadZone, yDeadZone, 0));
    }
}
