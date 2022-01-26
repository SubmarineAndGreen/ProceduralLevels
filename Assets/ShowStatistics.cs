using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowStatistics : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI shots;
    [SerializeField] TextMeshProUGUI enemies;
    private void Awake()
    {
        SaveSerial saveSerial = GameObject.FindObjectOfType<SaveSerial>();
        saveSerial.LoadGame();
        shots.SetText("Shots Fired: " + saveSerial.shotsFiredToSave);
        enemies.SetText("Enemies Killed: " + saveSerial.enemyDefeatedToSave);
    }
}
