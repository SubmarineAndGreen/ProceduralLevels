using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public struct SamplerResult {
    public string tileSet;
    public List<TileRule> rules;
    public List<int> uniqueTiles;
    public connectionData[] connections;
    public void saveToFile(string path) {
        File.WriteAllText(path, JsonUtility.ToJson(this));
    }

    public static SamplerResult loadFromFile(string path) {
        return JsonUtility.FromJson<SamplerResult>(File.ReadAllText(path));
    }
}




