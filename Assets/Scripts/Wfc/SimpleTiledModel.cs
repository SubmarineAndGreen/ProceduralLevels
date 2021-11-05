using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SimpleTiledModel
{
    const int nOfRotations = 4;
    public List<int> tileIds;
    public List<TileRule> rules;

    public SimpleTiledModel(List<int> tileIds, List<TileRule> rules)
    {
        this.tileIds = tileIds;
        this.rules = rules;
    }

    public void saveToFile(string filename)
    {
        string jsonString = JsonUtility.ToJson(this);
        Debug.Log(jsonString);
        Directory.CreateDirectory($"{Application.dataPath}/{ModelSampler.SAVE_FOLDER}");
        File.WriteAllText($"{Application.dataPath}/{ModelSampler.SAVE_FOLDER}/{filename}.json", jsonString);
    }

    public static SimpleTiledModel loadFromFile(string filename)
    {  
        string jsonString = File.ReadAllText($"{Application.dataPath}/{ModelSampler.SAVE_FOLDER}/{filename}");
        return JsonUtility.FromJson<SimpleTiledModel>(jsonString);
    }

    public static int tileIndexToModelIndex(int index, int rotation) {
        return index * nOfRotations + rotation;
    }

    public static int modelIndexToTileIndex(int index) {
        return (index - (index % nOfRotations)) / nOfRotations;
    }

    public static int modelIndexToRotation(int index) {
        return index % nOfRotations;
    }
}