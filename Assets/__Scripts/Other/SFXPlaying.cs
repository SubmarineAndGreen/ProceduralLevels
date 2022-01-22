using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlaying : MonoBehaviour
{
    public AudioSource dash;
    public AudioSource walk;
    public void PlayDash()
    {
        Debug.Log("DASH SFX");
        dash.Play();
    }
    public void PlayWalk()
    {
        if(!walk.isPlaying)
        {
            Debug.Log("WALK SFX");
            walk.Play();
        }
    }
    public void StopWalk()
    {
        walk.Stop();
    }
}
