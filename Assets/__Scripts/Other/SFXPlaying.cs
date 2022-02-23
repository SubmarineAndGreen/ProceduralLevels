using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlaying : MonoBehaviour
{
    public AudioSource dash;
    public AudioSource walk;
    [SerializeField] SaveSerial saveSerial;
    private float volume;

    private void Awake()
    {
        saveSerial.LoadGame();
        volume = saveSerial.volumeToSave;
        dash.volume = volume;
        walk.volume = volume;
    }

    public void PlayDash()
    {
        //Debug.Log("DASH SFX");
        dash.Play();
    }
    public void PlayWalk()
    {
        if(!walk.isPlaying)
        {
            //Debug.Log("WALK SFX");
            walk.Play();
        }
    }
    public void StopWalk()
    {
        walk.Stop();
    }
}
