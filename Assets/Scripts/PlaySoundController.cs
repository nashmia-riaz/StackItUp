using UnityEngine;

/// <summary>
/// This script is used to play sounds on triggers, usually for buttons
/// </summary>
public class PlaySoundController : MonoBehaviour
{
    //sound to play
    public AudioClip soundToPlay;

    //tag for sound source
    public string sourceTag = "Sound";
        
    //play as the object starts
    public bool playOnStart = true;
        
	void Start()
	{
		if( playOnStart == true )    PlaySound(soundToPlay);
	}
	
	public void PlaySound(AudioClip sound)
    {
        if (sourceTag != string.Empty && sound)
        {
            GameObject.FindGameObjectWithTag(sourceTag).GetComponent<AudioSource>().PlayOneShot(sound);
        }
    }
}
