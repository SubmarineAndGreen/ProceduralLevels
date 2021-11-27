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
        wfcRunner = GetComponent<WfcRunner>();

        levelGrid.tileSets = tileSets;

        //leave 1 tile of margin on each side for ending hole blocking tiles
        Vector3Int oneTileMargin = Vector3Int.one * 2;
        fullDimensions = generatedGridDimensions + oneTileMargin;
        levelGrid.resize(fullDimensions);

        int[,,] generatedTiles;
        bool generationSuccess = wfcRunner.runAdjacentModel(out generatedTiles, pipesSampleFileName, tileSets[TILESET_PIPES], generatedGridDimensions);

        if (!generationSuccess) {
            Debug.LogError("WFC failed!");
            return;
        }

        Grid3D<int> tileIndices = levelGrid.tileIndices;
        Grid3D<int> tileRotations = levelGrid.tileRotations;

        //leave 1 empty tile border
        for (int x = 1; x < fullDimensions.x - 1; x++) {
            for (int y = 1; y < fullDimensions.y - 1; y++) {
                for (int z = 1; z < fullDimensions.z - 1; z++) {
                    tileIndices.set(x, y, z, TileUtils.modelIndexToTileIndex(generatedTiles[x - 1, y - 1, z - 1]));
                    tileRotations.set(x, y, z, TileUtils.modelIndexToRotation(generatedTiles[x - 1, y - 1, z - 1]));
                }
            }
        }
        capOffTileEnds();
        levelGrid.rebuildGrid();
    }

    private void capOffTileEnds() {
        int[] capIndices = {
            upTileCapIndex,
            downTileCapIndex,
            sideTileCapIndex
        };

        Grid3D<int> tiles = levelGrid.tileIndices;
        Grid3D<int> rotations = levelGrid.tileRotations;
        Grid3D<int> tileSetIndices = levelGrid.tileSetIndices;
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
                                    levelGrid.placeTileDontInstantiate(upTileCapIndex, TILESET_CAPS, position + offset, TileGrid.NO_ROTATION);
                                    break;
                                case Directions3D.DOWN:
                                    levelGrid.placeTileDontInstantiate(downTileCapIndex, TILESET_CAPS, position + offset, TileGrid.NO_ROTATION);
                                    break;
                                default:
                                //direction - 2 gives correct rotation where FORWARD/Z+ is rotation 0
                                    levelGrid.placeTileDontInstantiate(sideTileCapIndex, TILESET_CAPS, position + offset, (int)direction - 2);
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
        EditorUtils.filePicker("Pipes model", builder.pipesSampleFileName, TileSampler.savePath, (fileName) => {
            builder.pipesSampleFileName = fileName as string;
        });
    }
}

