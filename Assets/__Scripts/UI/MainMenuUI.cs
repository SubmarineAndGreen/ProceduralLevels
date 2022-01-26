using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Button startGameButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button acceptButton;
    [SerializeField] Slider volumeSlider;
    [SerializeField] SaveSerial saveSerial;
    //GameStats saveData;

    private void Awake() {
        startGameButton.onClick.AddListener(GameLoader.LoadNewLevel);
        optionsButton.onClick.AddListener(showOptionsUI);
        quitButton.onClick.AddListener(Application.Quit);
        saveSerial.LoadGame();
        volumeSlider.value = saveSerial.volumeToSave;
        //volumeStorage = GameObject.Find("VolumeStorage").GetComponent<VolumeStorage>();
        /*saveData = DataSaver.loadData<GameStats>("saveData");
        if(!saveData.Equals(default(GameStats)))
        {
            volumeSlider.value = saveData.volume;
            Debug.Log("Found Data");
        }
        else
        {
            DataSaver.saveData(saveData, "saveData");
            Debug.Log("Not Found Data");
        }*/
        acceptButton.onClick.AddListener(hideOptionsUI);
    }

    private void showOptionsUI() {
        startGameButton.gameObject.SetActive(false);
        optionsButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        acceptButton.gameObject.SetActive(true);
        volumeSlider.gameObject.SetActive(true);
    }

    private void hideOptionsUI()
    {
        //volumeStorage.UpdateVolume(volumeSlider.value);
        //saveData.volume = volumeSlider.value;
        //DataSaver.saveData(saveData, "saveData");
        saveSerial.volumeToSave = volumeSlider.value;
        saveSerial.SaveGame();
        startGameButton.gameObject.SetActive(true);
        optionsButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        acceptButton.gameObject.SetActive(false);
        volumeSlider.gameObject.SetActive(false);
    }
}
