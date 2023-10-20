using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private void Start(){
        DontDestroyOnLoad(gameObject);  //persist the menu manager
    }

    /// <summary>
    /// Load the first level of the game
    /// </summary>
    public void LoadLevelOne(){
        SceneManager.LoadScene("Level1");
    }

    /// <summary>
    /// Quit the application
    /// </summary>
    public void QuitGame(){
        Application.Quit();
    }
}
