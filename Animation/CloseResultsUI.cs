using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CloseResultsUI : MonoBehaviour
{
    public Animator UIAnimator;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI enemiesKilledText;

    public void Close(){
        UIAnimator.SetBool("computing", false);
    }

    private void Update(){
        if(UIAnimator.GetCurrentAnimatorStateInfo(0).IsName("show")){
            timeText.text = "Time: " + (Time.time - PlayerContext.startTime);
            enemiesKilledText.text = "Enemies Killed: " + PlayerContext.enemiesKilled;
        }
    }
}
