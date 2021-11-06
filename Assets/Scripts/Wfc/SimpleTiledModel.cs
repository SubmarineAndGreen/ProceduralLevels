using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class SimpleTiledModel {
    const int nOfRotations = 4;
    public List<int> tileModelIndices;
    public List<int> frequencyHints;
    private Dictionary<int, int> frequencyHintsForUniqueTiles = new Dictionary<int, int>();

    public List<TileRule> rules;

    public SimpleTiledModel(List<int> tileModelIndices, List<TileRule> rules) {
        this.tileModelIndices = tileModelIndices;
        this.rules = rules;
    }

    public void saveToFile(string filename) {
        string jsonString = JsonUtility.ToJson(this);
        Debug.Log(jsonString);
        Directory.CreateDirectory($"{Application.dataPath}/{ModelSampler.SAVE_FOLDER}");
        File.WriteAllText($"{Application.dataPath}/{ModelSampler.SAVE_FOLDER}/{filename}.json", jsonString);
    }

    public static SimpleTiledModel loadFromFile(string filename) {
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

    public void updateFrequencyHints(Dictionary<int, int> frequencyHintsForUniqueTiles) {

        var uniqueTileCounts = new Dictionary<int, int>();
        var frequencyRotationWeights = new List<int>();
        var uniqueTileIndices = getUniqueTileIndices();

        foreach (int tile in uniqueTileIndices) {
            if (uniqueTileCounts.ContainsKey(tile)) {
                uniqueTileCounts[tile] += 1;
            } else {
                uniqueTileCounts[tile] = 1;
            }
        }

        foreach (int modelIndex in tileModelIndices) {
            int tile = SimpleTiledModel.modelIndexToTileIndex(modelIndex);
            //1 over 'n of rotations' so for tile that has 4 available rotations each variant will have weight 0.25 
            frequencyRotationWeights.Add(1 / uniqueTileCounts[tile]);
        }

        frequencyHints = new List<int>();
        for (int i = 0; i < tileModelIndices.Count; i++) {
            int tileModelIndex = tileModelIndices[i];
            int index = modelIndexToTileIndex(tileModelIndex);
            frequencyHints.Add(frequencyHintsForUniqueTiles[index] * frequencyRotationWeights[i]);
        }
    }

    public List<int> getUniqueTileIndices() {
        var uniqueTileIds = new HashSet<int>();
        foreach(int tile in tileModelIndices) {
            uniqueTileIds.Add(modelIndexToTileIndex(tile));
        }
        return uniqueTileIds.ToList();
    }

    public Dictionary<int, int> getFrequencyHintsForUniqueTiles() {
        if(frequencyHintsForUniqueTiles == null) {
            frequencyHintsForUniqueTiles = new Dictionary<int, int>();
        }
        return frequencyHintsForUniqueTiles;
    }
}