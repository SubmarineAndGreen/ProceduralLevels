using UnityEngine;
using System.Collections.Generic;
using System.IO;

[CreateAssetMenu(fileName = "Structure", menuName = "wfc/Structure", order = 0)]

public class Structure : ScriptableObject {
    public GameObject structurePrefab;
    public string fileName;
    [HideInInspector] public List<StructureTile> tiles;
    [HideInInspector] public Vector3Int dimensions;
    string savePath;
    string saveFolder = "StructureSaves";

    public StructureTileCollection getTilesCollection() {
        savePath = $"{Application.dataPath}/{saveFolder}";
        StructureTileCollection collection = JsonUtility.FromJson<StructureTileCollection>(File.ReadAllText($"{savePath}/{fileName}.json"));
        return collection;
    }

    public void saveTilesToFile() {
        savePath = $"{Application.dataPath}/{saveFolder}";
        File.WriteAllText($"{savePath}/{fileName}.json", JsonUtility.ToJson(new StructureTileCollection(tiles, dimensions)));
    }
}


[System.Serializable]
public class StructureTileCollection {
    public Vector3Int dimensions;
    public List<StructureTile> tiles;
    public StructureTileCollection(List<StructureTile> tiles, Vector3Int dimensions) {
        this.tiles = tiles;
        this.dimensions = dimensions;
    }
}

