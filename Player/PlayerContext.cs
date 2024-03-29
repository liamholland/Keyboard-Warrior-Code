using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerContext : ScriptableObject
{
    [Header("-- Player --")]
    public bool airControl;
    public bool canDash;

    [Header("-- Keyboard --")]
    public bool available;
    public bool longCableUnlocked;
    public int level;
    public List<Key> keys;

    [Header("-- Position --")]
    public string sceneName;
    public static Vector2 spawnPosition;

    //static context
    //lists of game object names to find
    public static float startTime = 0f;
    public static int numCollectiblesFound = 0;
    public static int enemiesKilled = 0;
    public static int numDeaths = 0;
    public static List<string> openDoors = new List<string>();
    public static List<string> decodedComputers = new List<string>();
    public static List<string> npcsFinishedInScenes = new List<string>();
    public static List<string> conversationsToMakeAvailable = new List<string>();
    public static List<string> conversationsToMakeUnavailable = new List<string>();
    public static List<string> levelUpsCollected = new List<string>();
    public static List<string> collectiblesFound = new List<string>();

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
    /// Add a level up which the player has collected to the player context
    /// </summary>
    /// <param name="levelUp">The level up game object to add</param>
    public static void AddLevelUpToContext(GameObject levelUp){
        if(levelUp.GetComponent<LevelUp>() == null) return; //game object is not a level up

        //add the level up to the context
        if(!levelUpsCollected.Contains(levelUp.name)){
            levelUpsCollected.Add(levelUp.name);
        }
    }

    public static void AddCollectibleToContext(GameObject collectible){
        if(collectible.GetComponent<Collectible>() == null) return; //game object is not a collectible
    
        //add the collectible to the context
        if(!collectiblesFound.Contains(collectible.name)){
            collectiblesFound.Add(collectible.name);
        }
    }

    public static void AddNpcDoneInSceneToContext(GameObject npc){
        Npc npcComponent = npc.GetComponent<Npc>();
        
        if(npcComponent == null) return; //object is not an npc

        //add the npc to the context
        if(!npcsFinishedInScenes.Contains(npc.name)){
            npcsFinishedInScenes.Add(npc.name);
        }
        else{
            return; //avoid removing conversations that are not there
        }

        //remove the npc's conversations from both lists on the context
        foreach(Conversation c in npcComponent.conversations){
            if(conversationsToMakeAvailable.Contains(c.name)){
                conversationsToMakeAvailable.Remove(c.name);
            }
            else if(conversationsToMakeUnavailable.Contains(c.name)){
                conversationsToMakeUnavailable.Remove(c.name);
            }
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

        //for each npc in the scene
        foreach(GameObject npc in npcs){
            //if the npc is done in this scene
            if(npcsFinishedInScenes.Contains(npc.name)){
                npc.SetActive(false);  //remove it from the scene
            }
            else{
                //otherwise, for each conversation it has, apply the required availability
                foreach(Conversation c in npc.GetComponent<Npc>().conversations){
                    if(conversationsToMakeAvailable.Contains(c.name)){
                        c.IsAvailable = true;
                    }
                    else if(conversationsToMakeUnavailable.Contains(c.name)){
                        c.IsAvailable = false;
                    }
                }
            }
        }

        foreach(string levelUpName in levelUpsCollected){
            GameObject levelUp = GameObject.Find(levelUpName);
            if(levelUp != null){
                levelUp.SetActive(false);   //remove it from the scene
            }
        }

        foreach(string collectible in collectiblesFound){
            GameObject collectibleObject = GameObject.Find(collectible);

            if(collectibleObject != null){
                collectibleObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Generate a new context object
    /// </summary>
    /// <param name="player">Instance of the player controller</param>
    /// <param name="keyboard">Instance of the player keyboard controller</param>
    /// <returns></returns>
    public static PlayerContext GenerateNewContext(Controller player, KeyboardController keyboard){
        //create a context
        PlayerContext context = (PlayerContext)CreateInstance("PlayerContext");
        context.airControl = player.airControl;
        context.canDash = player.canDash;
        context.available = keyboard.KeyboardAvailable;
        context.longCableUnlocked = keyboard.longCableUnlocked;
        context.level = keyboard.Level;
        context.keys = keyboard.keys;
        context.sceneName = SceneManager.GetActiveScene().name;

        return context;
    }

    /// <summary>
    /// Resets the player context for new games
    /// </summary>
    public static void Reset(){
        Controller.context = null;

        //reset all static variables
        startTime = 0f;
        numCollectiblesFound = 0;
        enemiesKilled = 0;
        numDeaths = 0;
        openDoors.Clear();
        decodedComputers.Clear();
        npcsFinishedInScenes.Clear();
        conversationsToMakeAvailable.Clear();
        conversationsToMakeUnavailable.Clear();
        levelUpsCollected.Clear();
        collectiblesFound.Clear();
    }
}
