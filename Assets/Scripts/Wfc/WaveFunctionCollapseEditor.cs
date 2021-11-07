using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using UnityEditor;

[CustomEditor(typeof(WaveFunctionCollapse))]
public class WaveFunctionCollapseEditor : Editor {
    WaveFunctionCollapse wfc;

    private void OnEnable() {
        wfc = target as WaveFunctionCollapse;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();


        EditorUtils.filePicker("Model", wfc.modelFile, $"{Application.dataPath}/{ModelSampler.SAVE_FOLDER}", (object parameter) => {
            wfc.modelFile = parameter as string;
            wfc.model = SimpleTiledModel.loadFromFile(wfc.modelFile);
        });

        EditorUtils.guiButton("Run", () => {
            wfc.run(wfc.tries);
        });


            var tiles = wfc.model.getUniqueTileIndices();
            var frequencyHintsForUniqueTiles = wfc.model.getFrequencyHintsForUniqueTiles();

            EditorGUILayout.LabelField("Tile Weights", EditorStyles.boldLabel);
            foreach (int tile in tiles) {
                if (!frequencyHintsForUniqueTiles.ContainsKey(tile)) {
                    frequencyHintsForUniqueTiles[tile] = 1;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(wfc.tileSet.tiles[tile].tilePrefab.name);
                frequencyHintsForUniqueTiles[tile] =
                    EditorGUILayout.IntSlider(frequencyHintsForUniqueTiles[tile], 0, 100);
                EditorGUILayout.EndHorizontal();
            }

            EditorUtils.guiButton("Save frequencies to model", () => {
                wfc.model.updateFrequencyHints(frequencyHintsForUniqueTiles);
                //TODO: remove replace
                wfc.model.saveToFile(wfc.modelFile.Replace(".json", ""));
            });
        }

}
