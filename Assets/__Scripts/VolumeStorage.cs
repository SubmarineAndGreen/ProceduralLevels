using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeStorage
{
    static VolumeStorage volumeStorage;
    float volume;

    void Start()
    {
        volumeStorage = new VolumeStorage();
    }

    public void UpdateVolume(float i)
    {
        volume = i;
    }

    public float CheckVolume()
    {
        return volume;
    }
}
