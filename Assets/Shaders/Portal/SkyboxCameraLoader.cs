using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkyboxCameraLoader : MonoBehaviour
{
    private void Start() {
        SceneManager.LoadScene("SkyboxCamera", LoadSceneMode.Additive);
    }
}
