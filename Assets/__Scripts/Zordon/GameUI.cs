using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {
    public Slider healthSlider;
    [SerializeField] private GameObject dashSlider;
    [HideInInspector] public Image dashSliderImage;
    public GameObject[] dashImage;
    public GameObject deathScreen;
    public GameObject victoryScreen;
    public GameObject pauseMenu;
    public GameObject dashEffect;
    public GameObject dashPosition;
    public Image heartImageBar;
    public TextMeshProUGUI hpText;

    public bool godMode;

    private bool isPaused;
    // private bool isSceneChanging;
    public int hp;

    private void Awake() {
        dashSliderImage = dashSlider.GetComponent<Image>();
    }

    private void Start() {
        isPaused = false;
        // isSceneChanging = false;
        godMode = false;
        // dashSlider = dashSliderGO.GetComponent<Image>();


        // dashSlider.material.SetFloat("_Progress", 0);
    }

    // public void UpdateDashes(int dashes) {
    //     const int maxDashes = 4;
    //     for(int i = 0; i < maxDashes; i++) {
    //         dashImage[i].SetActive(i < dashes);
    //     }
    // }

    // public void UpdateDashesCooldown(float cd) {
    //     dashSlider.material.SetFloat("_Progress", cd / 2);
    // }

    public void UpdateHealth(int hp) {
        //healthText.text = "Health: " + hp;
        if (!godMode)
            healthSlider.value = hp;
    }

    public void DeathScreen() {
        deathScreen.SetActive(true);
        StartCoroutine(Death());
    }
    public void NewLevel() {
        victoryScreen.SetActive(true);
        StartCoroutine(Win());
    }
    public void PauseMenu() {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        if (isPaused) {
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        } else {
            Time.timeScale = 1.0f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void ExitButton() {
        StartCoroutine(Pause());
    }

    public void DashEffect() {
        if (dashPosition.Equals(null)) {
            dashPosition = GameObject.Find("Dash VFX Position");
        }

        GameObject temp = Instantiate(dashEffect, dashPosition.transform);
        Destroy(temp, 2);
    }
    IEnumerator Win() {
        yield return new WaitForSeconds(2);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("mvp");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
    IEnumerator Death() {
        yield return new WaitForSeconds(4);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Menu");

        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
    IEnumerator Pause() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Menu");

        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}
