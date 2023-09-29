using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float cameraSpeed;
    public float xCatchUpMultiplier;  //multiplier applied to cameraSpeed when player is outside the deadzone
    public float yOffset;   //the distance the camera is above the player
    [Range(0f, 10f)] public float xDeadZone;
    [Range(0f, 10f)] public float yDeadZone;

    private bool outOfBoundsX = false;
    private bool outOfBoundsY = false;

    // Start is called before the first frame update
    void Start()
    {   
        //set the position of the camera to be the same as the player's
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + yOffset, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        float desiredX = player.transform.position.x;

        float xDiff = Mathf.Abs(desiredX - transform.position.x);    //difference between the player and camera x position

        //if the player has moved out of the x deadzone bounds
        if(xDiff > xDeadZone || outOfBoundsX){
            outOfBoundsX = true;

            //if the camera is out of the deadzone, apply a multipler to catch up
            float speed = xDiff > xDeadZone ? cameraSpeed * xCatchUpMultiplier : cameraSpeed;

            //interpolate between the current x value and the desired x value quickly
            float smoothX = Mathf.Lerp(transform.position.x, desiredX, speed * Time.deltaTime);

            //set the position of the camera
            transform.position = new Vector3(smoothX, transform.position.y, transform.position.z);

            if(Mathf.Abs(transform.position.x - desiredX) < 0.1f){
                outOfBoundsX = false;
            }
        }

        float desiredY = player.transform.position.y + yOffset;

        float yDiff = Mathf.Abs(desiredY - transform.position.y);    //difference between the player and the camera y position

        //if the player has moved out of the y deadzone bounds
        if(yDiff > yDeadZone || outOfBoundsY){
            outOfBoundsY = true;

            //interpolate between the current y position and the player's position
            float smoothY = Mathf.Lerp(transform.position.y, desiredY, cameraSpeed * Time.deltaTime);

            //set the position of the camera
            transform.position = new Vector3(transform.position.x, smoothY, transform.position.z);

            if(Mathf.Abs(transform.position.y - desiredY) < 0.1f){
                outOfBoundsY = false;
            }
        }

    }
}
