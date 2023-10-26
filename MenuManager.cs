using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject changeResMenu;    //the menu to change the resolution of the game
    public TextMeshProUGUI versionText; //the text displaying the version of the build

    public Animator sceneTransitionAnimator;    //reference to the scene transition

    private void Start(){
        versionText.text = "V" + Application.version;   //set the version text
        SetRes720();    //set the resolution to 720p
    }

    /// <summary>
    /// Load the first level of the game
    /// </summary>
    public void LoadLevelOne(){
        StartCoroutine(LoadLevelOneTransition());
    }

    private IEnumerator LoadLevelOneTransition(){
        sceneTransitionAnimator.SetBool("LoadingScene", true);
        
        yield return new WaitUntil(() => sceneTransitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("OnScreen"));

        SceneManager.LoadScene("Level1");
    }

    /// <summary>
    /// Quit the application
    /// </summary>
    public void QuitGame(){
        Application.Quit();
    }

    /// <summary>
    /// Set the resolution of the window to 1280x720
    /// </summary>
    public void SetRes720(){
        Screen.SetResolution(1280, 720, false);
        HideResChangeMenu();
    }

    /// <summary>
    /// Set the resolution of the window to 1920x1080
    /// </summary>
    public void SetRes1080(){
        Screen.SetResolution(1920, 1080, false);
        HideResChangeMenu();
    }

    public void ShowResChangeMenu(){
        changeResMenu.SetActive(true);
    }

    public void HideResChangeMenu(){
        changeResMenu.SetActive(false);
    }
}
