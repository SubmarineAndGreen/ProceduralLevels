using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSerial : MonoBehaviour
{
    public float volumeToSave;
    public int enemyDefeatedToSave;
    public int shotsFiredToSave;

    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
                     + "/MySaveData.dat");
        SaveData data = new SaveData();
        data.volume = volumeToSave;
        data.enemyDefeated = enemyDefeatedToSave;
        data.shotsFired = shotsFiredToSave;
        bf.Serialize(file, data);
        file.Close();
        //Debug.Log("Game data saved!");
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath
                       + "/MySaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
                       File.Open(Application.persistentDataPath
                       + "/MySaveData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            volumeToSave = data.volume;
            enemyDefeatedToSave = data.enemyDefeated;
            shotsFiredToSave = data.shotsFired;
            //Debug.Log("Game data loaded!");
        }
        else
        {
            //Debug.LogError("There is no save data!");
            volumeToSave = 1;
            enemyDefeatedToSave = 0;
            shotsFiredToSave = 0;
            SaveGame();
        }
    }
}

[System.Serializable]
public class SaveData
{
    public float volume;
    public int enemyDefeated;
    public int shotsFired;
}