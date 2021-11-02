using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class WfcModel
{
    public List<int> tileIds;
    public List<GridAdjacencyConstraint> constraints;

    public WfcModel(List<int> tileIds, List<GridAdjacencyConstraint> constraints)
    {
        this.tileIds = tileIds;
        this.constraints = constraints;
    }

    public void saveToFile(string filename)
    {
        string jsonString = JsonUtility.ToJson(this);
        Directory.CreateDirectory($"{Application.dataPath}/{ModelSampler.SAVE_FOLDER}");
        File.WriteAllText($"{Application.dataPath}/{ModelSampler.SAVE_FOLDER}/{filename}.json", jsonString);
    }

    public static WfcModel loadFromFile(string filename)
    {  
        string jsonString = File.ReadAllText($"{Application.dataPath}/{ModelSampler.SAVE_FOLDER}/{filename}");
        return JsonUtility.FromJson<WfcModel>(jsonString);
    }
}