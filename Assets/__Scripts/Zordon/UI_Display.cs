using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UI_Display : MonoBehaviour
{
    [SerializeField] public Slider healthSlider;
    [SerializeField] public Slider progressSlider;
    [SerializeField] public Slider energySlider;
    //[SerializeField] public TextMeshProUGUI healthText;
    [SerializeField] public TextMeshProUGUI weaponText;
    [SerializeField] public GameObject deathScreen;
    [SerializeField] public GameObject victoryScreen;
    [SerializeField] public GameObject pauseMenu;
    public bool godMode;

    private bool isPaused;
    private bool isSceneChanging;
    public int hp;
    public int energy;
    public int progress;

    private void Start()
    {
        isPaused = false;
        isSceneChanging = false;
        godMode = false;
    }

    public void UpdateHealth(int hp)
    {
        //healthText.text = "Health: " + hp;
        if(!godMode)
            healthSlider.value = hp;
    }
    public void UpdateWeapon(int weapon)
    {
        switch (weapon)
        {
            case 0:
                weaponText.text = "Magic Missiles";
                break;
            case 1:
                weaponText.text = "Claymore";
                break;
        }
    }
    public void AddProgress(int progress)
    {
        progressSlider.value += progress;
        if(progressSlider.value==100&&!isSceneChanging)
        {
            isSceneChanging = true;
            NewLevel();
        }
    }
    public void AddEnergy(int energy)
    {
        energySlider.value += energy;
    }
    public void DeathScreen()
    {
        deathScreen.SetActive(true);
        StartCoroutine(Death());
    }
    public void NewLevel()
    {
        victoryScreen.SetActive(true);
        StartCoroutine(Win());
    }
    public void PauseMenu()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        if(isPaused)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Time.timeScale = 1.0f;
            Cursor.visible = false;
            Cursor.lockState=CursorLockMode.Locked;
        }
    }
    public void ExitButton()
    {
        StartCoroutine(Pause());
    }
    IEnumerator Win()
    {
        yield return new WaitForSeconds(2);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("mvp");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    IEnumerator Death()
    {
        yield return new WaitForSeconds(4);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Menu");
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    IEnumerator Pause()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Menu");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
