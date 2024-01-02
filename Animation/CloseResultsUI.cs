using TMPro;
using UnityEngine;

public class CloseResultsUI : MonoBehaviour
{
    public Animator UIAnimator;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI collectiblesText;
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI numDeathsText;
    public Controller player;
    public KeyboardController keyboard;

    public void Close(){
        UIAnimator.SetBool("computing", false);
    }

    private void Update(){
        if(UIAnimator.GetCurrentAnimatorStateInfo(0).IsName("show")){
            timeText.text = "Time: " + (Time.time - PlayerContext.startTime);
            collectiblesText.text = "Found: " + PlayerContext.numCollectiblesFound + " / 6";
            enemiesKilledText.text = "Enemies Killed: " + PlayerContext.enemiesKilled;
            numDeathsText.text = "No. of Deaths: " + PlayerContext.numDeaths;
        }
    }

    /// <summary>
    /// Load the main menu
    /// </summary>
    public void LoadMainMenu(){
        Controller.context = PlayerContext.GenerateNewContext(player, keyboard);
        StartCoroutine(player.LoadSceneAnimation("MainMenu"));
    }
}
