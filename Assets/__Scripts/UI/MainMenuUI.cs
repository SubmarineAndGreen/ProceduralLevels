using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Button startGameButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button quitButton;

    private void Awake() {
        startGameButton.onClick.AddListener(GameLoader.LoadNewLevel);
        optionsButton.onClick.AddListener(showOptionsUI);
        quitButton.onClick.AddListener(Application.Quit);
    }

    private void showOptionsUI() {

    }
}
