using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource music;  //the game music audio
    private float defaultVolume;    //the default volume of the music

    private void Awake(){
        //assign the value of the component
        music = GetComponent<AudioSource>();

        //set the volume
        defaultVolume = music.volume;
    }

    private void Update(){
        music.volume = Controller.isInteracting ? defaultVolume / 2 : defaultVolume;
    }
}
