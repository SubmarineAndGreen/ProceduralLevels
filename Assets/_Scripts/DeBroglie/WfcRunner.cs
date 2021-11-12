using System.Collections;
using System.Collections.Generic;
using DeBroglie;
using DeBroglie.Models;
using DeBroglie.Topo;
using UnityEngine;
using UnityEditor;
using Resolution = DeBroglie.Resolution;
using DeBroglie.Constraints;

public class WfcRunner : MonoBehaviour {
    public TileGrid input;
    public TileGrid output;
    public int numberOfTries = 1;
    public bool backtracking = true;
    [Header("Overlapping Model")]
    [Range(1, 5)]
    public int tileSize = 2;
    [Range(1, 4)]
    public int rotations = 1;
    public bool reflections = false;

    public void runAdjacentModel() {
        ModelSampler sampler = new ModelSampler();
        sampler.ignoreEmptyTiles = true;
        List<TileRule> rules;
        List<int> tiles;

        (rules, tiles) = sampler.sample(input);

        Dictionary<int, DeBroglie.Tile> tileObjects = new Dictionary<int, DeBroglie.Tile>();

        foreach (int tile in tiles) {
            tileObjects[tile] = new DeBroglie.Tile(tile);
        }



        Vector3Int inputDimensions = input.dimensions;
        Vector3Int outputDimensions = output.dimensions;

        // int[,,] tilesIn = new int[inputDimensions.x, inputDimensions.y, inputDimensions.z];
        int[,,] tilesOut = new int[outputDimensions.x, outputDimensions.y, outputDimensions.z];

        // input.tileIndices.forEach((x, y, z, index) => {
        //     tilesIn[x, y, z] = SimpleTiledModel.tileIndexToModelIndex(index, input.tileRotations.at(x, y, z));
        // });

        GridTopology outputGridTopo = new GridTopology(outputDimensions.x, outputDimensions.y, outputDimensions.z, false);

        // ITopoArray<int> topoArray = TopoArray.Create(tilesIn, false);

        var model = new AdjacentModel(DirectionSet.Cartesian3d);
        foreach (TileRule rule in rules) {
            Directions3D direction = rule.directionAtoB;
            Vector3Int v = SamplerUtils.DirectionsToVectors[direction];
            model.AddAdjacency(tileObjects[rule.valueA], tileObjects[rule.valueB], v.x, v.y, v.z);
        }

        foreach (var item in tileObjects) {
            int tileIndex = TileUtils.modelIndexToTileIndex(item.Key);
            int numberOfRotations = Tile.symmetryToNumberOfRotations[input.tileSet.tiles[tileIndex].symmetry];
            model.SetFrequency(item.Value, 1.0 / numberOfRotations);
        }

        var propagator = new TilePropagator(model, outputGridTopo, new TilePropagatorOptions() {
            BackTrackDepth = backtracking ? -1 : 0
        });

        while (numberOfTries > 0) {
            var status = propagator.Run();
            if (status != Resolution.Decided) {
                numberOfTries--;
                Debug.Log("Undecided!");
            } else {
                break;
            }
        }



        tilesOut = propagator.ToValueArray<int>().ToArray3d<int>();

        for (int x = 0; x < outputDimensions.x; x++) {
            for (int y = 0; y < outputDimensions.y; y++) {
                for (int z = 0; z < outputDimensions.z; z++) {
                    output.tileIndices.set(new Vector3Int(x, y, z), TileUtils.modelIndexToTileIndex(tilesOut[x, y, z]));
                    output.tileRotations.set(new Vector3Int(x, y, z), TileUtils.modelIndexToRotation(tilesOut[x, y, z]));
                }
            }
        }

        output.rebuildGrid();

    }

    public void runOverlapModel() {
        Vector3Int inputDimensions = input.dimensions;
        Vector3Int outputDimensions = output.dimensions;

        int[,,] tilesIn = new int[inputDimensions.x, inputDimensions.y, inputDimensions.z];
        int[,,] tilesOut = new int[outputDimensions.x, outputDimensions.y, outputDimensions.z];

        input.tileIndices.forEach((x, y, z, index) => {
            tilesIn[x, y, z] = TileUtils.tileIndexToModelIndex(index, input.tileRotations.at(x, y, z));
        });

        GridTopology outputGridTopo = new GridTopology(outputDimensions.x, outputDimensions.y, outputDimensions.z, false);
        ITopoArray<int> topoArray = TopoArray.Create(tilesIn, false);

        // var model = new OverlappingModel(topoArray.ToTiles(), tileSize, rotations, reflections);
        var model = new OverlappingModel(tileSize, 1, tileSize);
        model.AddSample(topoArray.ToTiles(), new DeBroglie.Rot.TileRotation(rotations, reflections));

        var propagator = new TilePropagator(model, outputGridTopo, new TilePropagatorOptions() {
            BackTrackDepth = backtracking ? -1 : 0
        });

        while (numberOfTries > 0) {
            var status = propagator.Run();
            if (status != Resolution.Decided) {
                numberOfTries--;
                Debug.Log("Undecided!");
            } else {
                break;
            }
        }

        tilesOut = propagator.ToValueArray<int>().ToArray3d<int>();

        for (int x = 0; x < outputDimensions.x; x++) {
            for (int y = 0; y < outputDimensions.y; y++) {
                for (int z = 0; z < outputDimensions.z; z++) {
                    output.tileIndices.set(new Vector3Int(x, y, z), TileUtils.modelIndexToTileIndex(tilesOut[x, y, z]));
                    output.tileRotations.set(new Vector3Int(x, y, z), TileUtils.modelIndexToRotation(tilesOut[x, y, z]));
                }
            }
        }

        output.rebuildGrid();
    }
}




[CustomEditor(typeof(WfcRunner))]
public class WfcRunnerEditor : Editor {

    WfcRunner wfc;
    private void OnEnable() {
        wfc = target as WfcRunner;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorUtils.guiButton("Run Adjacent Model", () => {
            wfc.runAdjacentModel();
        });

        EditorUtils.guiButton("Run Overlap Model", () => {
            wfc.runOverlapModel();
        });
    }
}
