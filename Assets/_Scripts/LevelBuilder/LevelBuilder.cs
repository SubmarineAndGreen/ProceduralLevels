using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

[RequireComponent(typeof(WfcRunner))]
public class LevelBuilder : MonoBehaviour {
    private WfcRunner wfcRunner;
    [SerializeField] private TileGrid levelGrid;
    [SerializeField] Vector3Int generatedGridDimensions;
    Vector3Int fullDimensions;
    public TileSet[] tileSets;
    private const int TILESET_PIPES = 0;
    private const int TILESET_CAPS = 1;
    [SerializeField] int upTileCapIndex;
    [SerializeField] int downTileCapIndex;
    [SerializeField] int sideTileCapIndex;
    [HideInInspector] public string pipesSampleFileName;


    void Start() {
        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

        wfcRunner = GetComponent<WfcRunner>();

        levelGrid.tileSets = tileSets;

        //leave 1 tile of margin on each side for ending hole blocking tiles
        Vector3Int oneTileMargin = Vector3Int.one * 2;
        fullDimensions = generatedGridDimensions + oneTileMargin;

        int[,,] generatedTiles;
        bool generationSuccess = wfcRunner.runAdjacentModel(out generatedTiles, pipesSampleFileName, tileSets[TILESET_PIPES], generatedGridDimensions);

        if (!generationSuccess) {
            Debug.LogError("WFC failed!");
            return;
        }

        levelGrid.dimensions = fullDimensions;
        Grid3D<int> tileIndices = new Grid3D<int>(fullDimensions);
        tileIndices.updateEach((int index) => TileGrid.TILE_EMPTY);
        Grid3D<int> tileRotations = new Grid3D<int>(fullDimensions);
        Grid3D<int> tileSetIndices = new Grid3D<int>(fullDimensions);
        Grid3D<GameObject> tileObjects = new Grid3D<GameObject>(fullDimensions);

        //leave 1 empty tile border
        for (int x = 1; x < fullDimensions.x - 1; x++) {
            for (int y = 1; y < fullDimensions.y - 1; y++) {
                for (int z = 1; z < fullDimensions.z - 1; z++) {
                    tileIndices.set(x, y, z, TileUtils.modelIndexToTileIndex(generatedTiles[x - 1, y - 1, z - 1]));
                    tileRotations.set(x, y, z, TileUtils.modelIndexToRotation(generatedTiles[x - 1, y - 1, z - 1]));
                    tileSetIndices.set(x, y, z, TILESET_PIPES);
                }
            }
        }
        capOffTileEnds(tileIndices, tileRotations, tileSetIndices);

        int[,,] inputDistanceField = new int[fullDimensions.x, fullDimensions.y, fullDimensions.z];
        int blockedTile = -1;

        tileIndices.forEach((x, y, z, tileIndex) => {
            if(tileIndex == TileGrid.TILE_EMPTY || tileIndex == tileSets[TILESET_PIPES].emptyTileIndex) {
                inputDistanceField[x,y,z] = blockedTile;
            } else {
                inputDistanceField[x,y,z] = int.MaxValue / 2;
            }
        });

        NavigationManager navigation = NavigationManager.instance;
        navigation.calculateVectorFields(inputDistanceField, blockedTile);


        levelGrid.tileIndices = tileIndices;
        levelGrid.tileRotations = tileRotations;
        levelGrid.tileSetIndices = tileSetIndices;
        levelGrid.tileObjects = tileObjects;
        levelGrid.rebuildGrid();

        Debug.Log("Level created in: " + stopwatch.ElapsedMilliseconds + "ms");
        stopwatch.Stop();
    }

    private void capOffTileEnds(Grid3D<int> tiles, Grid3D<int> rotations, Grid3D<int> tileSetIndices) {

        ConnectionData[] connectionData = wfcRunner.samplerResult.connections;

        tiles.forEach((position, tile) => {
            if (tile != TileGrid.TILE_EMPTY && tileSetIndices.at(position) != TILESET_CAPS) {
                var possibleConnections = connectionData[tile];
                foreach (Directions3D direction in SamplerUtils.allDirections) {
                    bool isConnectionPossible = possibleConnections.canConnectFromDirection(direction, rotations.at(position));
                    if (isConnectionPossible) {
                        Vector3Int offset = SamplerUtils.DirectionsToVectors[direction];
                        // Debug.Log(tiles.at(position) + " " + offset);
                        if (tiles.at(position + offset) == TileGrid.TILE_EMPTY) {
                            switch (direction) {
                                case Directions3D.UP:
                                    tileSetIndices.set(position + offset, TILESET_CAPS);
                                    tiles.set(position + offset, upTileCapIndex);
                                    rotations.set(position + offset, TileGrid.NO_ROTATION);
                                    // levelGrid.placeTileDontInstantiate(upTileCapIndex, TILESET_CAPS, position + offset, TileGrid.NO_ROTATION);
                                    break;
                                case Directions3D.DOWN:
                                    tileSetIndices.set(position + offset, TILESET_CAPS);
                                    tiles.set(position + offset, downTileCapIndex);
                                    rotations.set(position + offset, TileGrid.NO_ROTATION);
                                    break;
                                default:
                                    tileSetIndices.set(position + offset, TILESET_CAPS);
                                    tiles.set(position + offset, sideTileCapIndex);
                                    rotations.set(position + offset, (int)direction - 2);
                                    //direction - 2 gives correct rotation where FORWARD/Z+ is rotation 0
                                    // levelGrid.placeTileDontInstantiate(sideTileCapIndex, TILESET_CAPS, position + offset, (int)direction - 2);
                                    break;
                            }
                        }
                    }
                }
            }
        });
    }
}



[CustomEditor(typeof(LevelBuilder))]
public class LevelBuilderEditor : Editor {
    LevelBuilder builder;

    private void OnEnable() {
        builder = target as LevelBuilder;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorUtils.filePicker("Pipes model", builder.pipesSampleFileName, $"{Application.dataPath}/SamplerSaves", (fileName) => {
            builder.pipesSampleFileName = fileName as string;
        });
    }
}

