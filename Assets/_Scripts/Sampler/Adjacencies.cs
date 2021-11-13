using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public struct Adjacencies {
    public string tileSet;
    public List<TileRule> rules;
    public List<int> uniqueTiles;
    
    public void saveToFile(string path) {
        File.WriteAllText(path, JsonUtility.ToJson(this));
    }

    public static Adjacencies loadFromFile(string path) {
        return JsonUtility.FromJson<Adjacencies>(File.ReadAllText(path));
    }
}


