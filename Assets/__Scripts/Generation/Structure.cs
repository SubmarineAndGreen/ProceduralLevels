using UnityEngine;
using System.Collections.Generic;
using System.IO;

[CreateAssetMenu(fileName = "Structure", menuName = "wfc/Structure", order = 0)]
public class Structure : ScriptableObject {
    public GameObject structurePrefab;
    public string fileName;
    [HideInInspector] public List<StructureTile> tiles;
    string savePath;
    string saveFolder = "StructureSaves";

    private void Awake() {
        savePath = $"{Application.dataPath}/{saveFolder}";
    }

    public void loadTilesFromFile() {
        tiles = JsonUtility.FromJson<List<StructureTile>>(File.ReadAllText($"{savePath}/{fileName}.json"));
    }

    public void saveTilesToFile() {
        File.WriteAllText($"{savePath}/{fileName}.json", JsonUtility.ToJson(tiles));
    }
}