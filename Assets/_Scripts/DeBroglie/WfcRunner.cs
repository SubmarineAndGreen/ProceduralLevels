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

public class WfcRunner : MonoBehaviour {
    public TileGrid input;
    public TileGrid output;
    [HideInInspector] public string modelFile;
    public bool runTillSolved = false;
    public long maxRunTime = 30;
    public int numberOfTries = 1;
    // -1 means backtracking off
    public int backtrackDepth = 0;
    // [Header("Overlapping Model")]
    // [Range(1, 5)]
    // public int tileSize = 2;
    // [Range(1, 4)]
    // public int rotations = 1;
    // public bool reflections = false;
    [Header("Path")]
    public bool path = true;
    [Header("Debug")]
    public float stepTime = 0.1f;
    [HideInInspector] public SamplerResult samplerResult;

    public bool runAdjacentModel(out int[,,] result, Vector3Int outputDimensions) {

        List<TileRule> rules;
        List<int> tiles;

        samplerResult = SamplerResult.loadFromFile($"{ModelSampler.savePath}/{modelFile}");
        rules = samplerResult.rules;
        tiles = samplerResult.uniqueTiles;

        Dictionary<int, DeBroglie.Tile> tileObjects = new Dictionary<int, DeBroglie.Tile>();

        foreach (int tile in tiles) {
            tileObjects[tile] = new DeBroglie.Tile(tile);
        }



        Vector3Int inputDimensions = input.dimensions;

        int[,,] tilesOut = new int[outputDimensions.x, outputDimensions.y, outputDimensions.z];

        GridTopology outputGridTopo = new GridTopology(outputDimensions.x, outputDimensions.y, outputDimensions.z, false);

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
            BackTrackDepth = backtrackDepth,
            Constraints = path ? new ITileConstraint[] {
                new PathConstraint(tilesWithoutEmpty(tiles, tileObjects))
            } : null
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

        result = propagator.ToValueArray<int>().ToArray3d<int>();
        return success;
    }

    public void runAndBuild() {
        Vector3Int outputDimensions = output.dimensions;
        int[,,] result;
        bool success = runAdjacentModel(out result, output.dimensions);
        if (success) {
            for (int x = 0; x < outputDimensions.x; x++) {
                for (int y = 0; y < outputDimensions.y; y++) {
                    for (int z = 0; z < outputDimensions.z; z++) {
                        output.tileIndices.set(new Vector3Int(x, y, z), TileUtils.modelIndexToTileIndex(result[x, y, z]));
                        output.tileRotations.set(new Vector3Int(x, y, z), TileUtils.modelIndexToRotation(result[x, y, z]));
                    }
                }
            }

            output.rebuildGrid();
        }
    }

    // public void runOverlapModel() {
    //     Vector3Int inputDimensions = input.dimensions;
    //     Vector3Int outputDimensions = output.dimensions;

    //     int[,,] tilesIn = new int[inputDimensions.x, inputDimensions.y, inputDimensions.z];
    //     int[,,] tilesOut = new int[outputDimensions.x, outputDimensions.y, outputDimensions.z];

    //     input.tileIndices.forEach((x, y, z, index) => {
    //         tilesIn[x, y, z] = TileUtils.tileIndexToModelIndex(index, input.tileRotations.at(x, y, z));
    //     });

    //     GridTopology outputGridTopo = new GridTopology(outputDimensions.x, outputDimensions.y, outputDimensions.z, false);
    //     ITopoArray<int> topoArray = TopoArray.Create(tilesIn, false);

    //     // var model = new OverlappingModel(topoArray.ToTiles(), tileSize, rotations, reflections);
    //     var model = new OverlappingModel(tileSize, 1, tileSize);
    //     model.AddSample(topoArray.ToTiles(), new DeBroglie.Rot.TileRotation(rotations, reflections));

    //     var propagator = new TilePropagator(model, outputGridTopo, new TilePropagatorOptions() {
    //         BackTrackDepth = backtrackDepth
    //     });

    //     while (numberOfTries > 0) {
    //         var status = propagator.Run();
    //         if (status != Resolution.Decided) {
    //             numberOfTries--;
    //             Debug.Log("Undecided!");
    //         } else {
    //             break;
    //         }
    //     }

    //     tilesOut = propagator.ToValueArray<int>().ToArray3d<int>();

    //     for (int x = 0; x < outputDimensions.x; x++) {
    //         for (int y = 0; y < outputDimensions.y; y++) {
    //             for (int z = 0; z < outputDimensions.z; z++) {
    //                 output.tileIndices.set(new Vector3Int(x, y, z), TileUtils.modelIndexToTileIndex(tilesOut[x, y, z]));
    //                 output.tileRotations.set(new Vector3Int(x, y, z), TileUtils.modelIndexToRotation(tilesOut[x, y, z]));
    //             }
    //         }
    //     }

    //     output.rebuildGrid();
    // }

    public HashSet<DeBroglie.Tile> tilesWithoutEmpty(List<int> uniqueTiles, Dictionary<int, DeBroglie.Tile> tileObjects) {
        var result = new HashSet<DeBroglie.Tile>();
        int emptyTile = TileUtils.tileIndexToModelIndex(output.tileSet.emptyTileIndex, TileGrid.NO_ROTATION);
        foreach (int tileIndex in uniqueTiles)
        {
            if(tileIndex != emptyTile) {
                result.Add(tileObjects[tileIndex]);
            }
        }
        return result;
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
        EditorGUILayout.Space(20);
        EditorUtils.filePicker("Model File", wfc.modelFile, ModelSampler.savePath,
         fileName => wfc.modelFile = fileName as string);

        EditorUtils.guiButton("Run Adjacent Model", () => {
            wfc.runAndBuild();
        });

        // EditorUtils.guiButton("Run Overlap Model", () => {
        //     wfc.runOverlapModel();
        // });
    }
}
