using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEditor;
using DeBroglie;
using DeBroglie.Models;
using DeBroglie.Topo;
using DeBroglie.Constraints;
using Resolution = DeBroglie.Resolution;
using Debug = UnityEngine.Debug;
using System;

public class WfcRunner : MonoBehaviour {

    [HideInInspector] public string modelFile;
    public bool runTillSolved = false;
    public long maxRunTime = 30;
    public int numberOfTries = 1;
    // -1 means backtracking off
    public int backtrackDepth = 0;

    // [Header("Path")]
    // public bool path = true;

    // [HideInInspector] public SamplerResult samplerResult;

    public int[,,] runAdjacentModel(Vector3Int outputDimensions,
                                    TileSet tileSet,
                                    List<int> tiles,
                                    List<TileRule> rules,
                                    Dictionary<int, DeBroglie.Tile> tileObjects,
                                    List<ITileConstraint> constraints,
                                    int rngSeed = LevelBuilder.NO_SEED) {
        System.Random rng;
        if (rngSeed == LevelBuilder.NO_SEED) {
            rng = new System.Random((int)DateTime.Now.Ticks);
        } else {
            rng = new System.Random(rngSeed);
        }

        int[,,] tilesOut = new int[outputDimensions.x, outputDimensions.y, outputDimensions.z];

        GridTopology outputGridTopo = new GridTopology(outputDimensions.x, outputDimensions.y, outputDimensions.z, false);

        var model = new AdjacentModel(DirectionSet.Cartesian3d);
        foreach (TileRule rule in rules) {
            Directions3D direction = rule.directionAtoB;
            Vector3Int v = DirectionUtils.DirectionsToVectors[direction];

            model.AddAdjacency(tileObjects[rule.valueA], tileObjects[rule.valueB], v.x, v.y, v.z);

        }

        foreach (var item in tileObjects) {
            int tileIndex = TileUtils.modelIndexToTileIndex(item.Key);
            Tile tile = tileSet.tiles[tileIndex];
            int numberOfRotations = Tile.symmetryToNumberOfRotations[tile.symmetry];
            float frequency = (1f / numberOfRotations);
            if(tile.frequency != 0) {
                frequency *= tile.frequency;
            }
            model.SetFrequency(item.Value, frequency);
        }


        var propagator = new TilePropagator(model, outputGridTopo, new TilePropagatorOptions() {
            BackTrackDepth = backtrackDepth,
            Constraints = constraints.ToArray(),
            RandomDouble = rng.NextDouble
        });

        int tries = numberOfTries;

        Stopwatch stopwatch = Stopwatch.StartNew();
        const int millis = 1000;
        long maxRunTimeMillis = maxRunTime * millis;

        bool success = false;

        while (tries > 0 || runTillSolved) {
            tries--;
            if (stopwatch.ElapsedMilliseconds > maxRunTimeMillis) {
                Debug.Log("max wfc time reached");
                break;
            }

            var status = propagator.Run();

            if (status == Resolution.Decided) {
                success = true;
                break;
            }

            propagator.Clear();
        }

        Debug.Log($"{(success ? "sucess" : "contradiction")} tries:{numberOfTries - tries}");

        int[,,] result = propagator.ToValueArray<int>().ToArray3d<int>();
        return result;
    }
}




// [CustomEditor(typeof(WfcRunner))]
// public class WfcRunnerEditor : Editor {

//     WfcRunner wfc;
//     private void OnEnable() {
//         wfc = target as WfcRunner;
//     }

//     public override void OnInspectorGUI() {
//         base.OnInspectorGUI();
//     }
// }
