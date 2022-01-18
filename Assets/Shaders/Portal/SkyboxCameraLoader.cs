using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkyboxCameraLoader : MonoBehaviour
{
    private void Start() {
        var op = SceneManager.LoadSceneAsync("SkyboxCamera", LoadSceneMode.Additive);
    }
}
