using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class WfcModel
{
    public List<GridAdjacencyConstraint> constraints;

    public WfcModel(List<GridAdjacencyConstraint> constraints)
    {
        this.constraints = constraints;
    }

    public void saveToFile(string filename)
    {
        string jsonString = JsonUtility.ToJson(this);
        Directory.CreateDirectory($"{Application.dataPath}/{ModelSampler.SAVE_FOLDER}");
        File.WriteAllText($"{Application.dataPath}/{ModelSampler.SAVE_FOLDER}/{filename}.json", jsonString);
    }
}