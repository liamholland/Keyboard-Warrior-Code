using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContext : ScriptableObject
{
    [Header("-- Player --")]
    public bool airControl;
    public bool canDash;

    [Header("-- Keyboard --")]
    public bool available;
    public bool longCableUnlocked;
    public int level;

    [Header("-- Position --")]
    public Vector2 position;

    //static context
    public static List<string> openDoors = new List<string>();
    public static List<string> decodedComputers = new List<string>();
    public static List<string> conversationsToMakeAvailable = new List<string>();
    public static List<string> conversationsToMakeUnavailable = new List<string>();

    /// <summary>
    /// Add a door which should remain open after the player has opened it
    /// </summary>
    /// <param name="door">The game object that the IDoor script belongs to</param>
    public static void AddDoorToContext(GameObject door){
        if(door.GetComponent<IDoor>() == null) return;  //game object is not a door

        //add the door to the list if it is not already there
        if(!openDoors.Contains(door.name)){
            openDoors.Add(door.name);
        }
    }

    /// <summary>
    /// Add a conversation which should be available to the player context
    /// </summary>
    /// <param name="c"></param>
    public static void AddAvailableConversationToContext(Conversation c){
        if(!conversationsToMakeAvailable.Contains(c.name)){
            conversationsToMakeAvailable.Add(c.name);
        }

        if(conversationsToMakeUnavailable.Contains(c.name)){
            conversationsToMakeUnavailable.Remove(c.name);
        }
    }

    /// <summary>
    /// Add a conversation which should be unavailable to the player context
    /// </summary>
    /// <param name="c">The conversation to make unavailable</param>
    public static void AddUnavailableConversationToContext(Conversation c){
        if(!conversationsToMakeUnavailable.Contains(c.name)){
            conversationsToMakeUnavailable.Add(c.name);
        }

        if(conversationsToMakeAvailable.Contains(c.name)){
            conversationsToMakeAvailable.Remove(c.name);
        }
    }

    /// <summary>
    /// Add a computer which has been decoded by the player and should remain decoded
    /// </summary>
    /// <param name="computer">The game object the computer script is on</param>
    public static void AddComputerToContext(GameObject computer){
        if(computer.GetComponent<Computer>() == null) return;   //game object is not a computer

        //add the computer to the list if it is not already there
        if(!decodedComputers.Contains(computer.name)){
            decodedComputers.Add(computer.name);
        }
    }



    /// <summary>
    /// Apply the player context to objects recorded in lists
    /// </summary>
    public static void ApplyContextToObjects(){
        //open doors
        foreach(string doorName in openDoors){
            GameObject door = GameObject.Find(doorName);
            if(door != null){
                door.GetComponent<IDoor>().Open();
            }
        }

        //decode computers
        foreach(string cName in decodedComputers){
            GameObject computer = GameObject.Find(cName);
            if(computer != null){
                computer.GetComponent<Computer>().decoded = true;
            }
        }
        
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("Npc");

        foreach(GameObject npc in npcs){
            Npc npcComponent = npc.GetComponent<Npc>();
            
            foreach(Conversation c in npcComponent.conversations){
                if(conversationsToMakeAvailable.Contains(c.name)){
                    Debug.Log("Match found");
                    c.IsAvailable = true;
                }
                else if(conversationsToMakeUnavailable.Contains(c.name)){
                    Debug.Log("Match found");
                    c.IsAvailable = false;
                }
            }
        }
    }
}
