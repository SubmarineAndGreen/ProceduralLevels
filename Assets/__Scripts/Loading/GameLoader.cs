using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoader : MonoBehaviour
{
    [SerializeField] Game game;
    [SerializeField] LevelBuilder levelBuilder;
    GameObject loadingScreen;

    private void Awake() {
        loadingScreen = GameObject.Find("LoadingScreen");
        StartCoroutine(initialize());
    }

    IEnumerator initialize() {
        // loadingScreen.SetActive(true);
        yield return StartCoroutine(levelBuilder.generateLevel());
        var sceneLoading = SceneManager.LoadSceneAsync("SkyboxCamera", LoadSceneMode.Additive);
        yield return new WaitUntil(() => sceneLoading.isDone);
        loadingScreen.SetActive(false);
        game.enabled = true;
    }
}
