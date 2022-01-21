using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameScene : MonoBehaviour
{
    [SerializeField] Camera loadingScreenCamera;
    private void Awake() {
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
    }
}
