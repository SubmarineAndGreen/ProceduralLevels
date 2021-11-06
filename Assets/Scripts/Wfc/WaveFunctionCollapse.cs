using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour {
    [HideInInspector] public string modelFile;
    public TileGrid outputGrid;
    public TileSet tileSet;
    [HideInInspector] public SimpleTiledModel model;
    //number of tile rotations included in model for each  unique tile <index, rotations>
    [HideInInspector] public Dictionary<int, int> uniqueTileCounts;
    //additional frequency wieghts based on number of rotations of a tile in a model
    private List<int> frequencyRotationWeights;
    //frequencyHints per tile/rotation pair in model taking rotationWeights in account
    private List<int> frequencyHints;

    public Grid3D<int> run(TileRule[] rules, int[] frequencyHints) {
        return null;
    }

    class cell {
        public List<bool> possibleTiles;

        // public int totalPossibleTileFrequency(List<int> tilesInModel) {
        //     int total = 0;
        //     for(int i = 0; i < possibleTiles.Count; i++) {
        //         int tileIndex = tilesInModel[i];
        //         bool possible = possibleTiles[i];
        //         if(possible) {
        //             total += 
        //         }
        //     }
        // }
    }

    class wfcState {
        public Grid3D<cell> grid;
        public int remainingUncollapsedCells;
        public TileRule[] TileRules;
        public int[] frequencyHints;

        public Vector3Int chooseNextCell() {
            return Vector3Int.zero;
        }

        public void collapseCellAt(Vector3Int position) {

        }

        public void propagate() {

        }

        public void run() {

        }
    }
}




