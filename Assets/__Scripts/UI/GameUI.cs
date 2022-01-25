using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameUI : MonoBehaviour {
    [SerializeField] private GameObject dashSlider;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject gameOverMenu;
    [HideInInspector] public Image dashSliderImage;
    public GameObject[] dashImage;
    public Image heartImageBar;
    public TextMeshProUGUI hpText;
    [SerializeField] Button resumeButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button playAgainButton;
    [SerializeField] Button lossQuitButton;

    [HideInInspector] public bool isGameOver;

    private void Awake() {
        dashSliderImage = dashSlider.GetComponent<Image>();
        resumeButton.onClick.AddListener(togglePauseMenu);

        quitButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu", LoadSceneMode.Single));
        lossQuitButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu", LoadSceneMode.Single));

        playAgainButton.onClick.AddListener(GameLoader.LoadNewLevel);
    }

    private void Update() {
        if(!isGameOver && Keyboard.current.escapeKey.wasPressedThisFrame) {
            togglePauseMenu();
        }
    }

    private void togglePauseMenu() {
        Game game = Game.instance;
        game.isGamePaused = !game.isGamePaused;
        game.setPaused(game.isGamePaused);
        pauseMenu.SetActive(game.isGamePaused);
        Cursor.lockState = game.isGamePaused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void showLossScreen() {
        isGameOver = true;
        Cursor.lockState = CursorLockMode.None;
        gameOverMenu.SetActive(true);
    }
}
