using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(WfcRunner))]
public class LevelBuilder : MonoBehaviour {
    private WfcRunner wfcRunner;
    [SerializeField] private TileGrid levelGrid;
    [SerializeField] Vector3Int generatedGridDimensions;
    Vector3Int fullDimensions;
    [SerializeField] int upTileCapIndex;
    [SerializeField] int downTileCapIndex;
    [SerializeField] int sideTileCapIndex;


    void Start() {
        var a = new List<int>();

        wfcRunner = GetComponent<WfcRunner>();
        wfcRunner.output = levelGrid;
        //levelGrid.clear();
        //leave 1 tile of margin on each side for ending hole blocking tiles
        Vector3Int oneTileMargin = Vector3Int.one * 2;
        fullDimensions = generatedGridDimensions + oneTileMargin;
        levelGrid.resize(fullDimensions);

        int[,,] generatedTiles;
        bool generationSuccess = wfcRunner.runAdjacentModel(out generatedTiles, generatedGridDimensions);

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
        ConnectionData[] connectionData = wfcRunner.samplerResult.connections;

        tiles.forEach((position, tile) => {
            if (tile != TileGrid.TILE_EMPTY && !capIndices.Contains(tile)) {
                var possibleConnections = connectionData[tile];
                foreach (Directions3D direction in SamplerUtils.allDirections) {
                    bool isConnectionPossible = possibleConnections.canConnectFromDirection(direction, rotations.at(position));
                    if (isConnectionPossible) {
                        Vector3Int offset = SamplerUtils.DirectionsToVectors[direction];
                        // Debug.Log(tiles.at(position) + " " + offset);
                        if (tiles.at(position + offset) == TileGrid.TILE_EMPTY) {
                            switch (direction) {
                                case Directions3D.UP:
                                    levelGrid.placeTile(upTileCapIndex, position + offset, TileGrid.NO_ROTATION);
                                    break;
                                case Directions3D.DOWN:
                                    levelGrid.placeTile(downTileCapIndex, position + offset, TileGrid.NO_ROTATION);
                                    break;
                                default:
                                    levelGrid.placeTile(sideTileCapIndex, position + offset, (int)direction - 2);
                                    break;
                            }
                        }
                    }
                }
            }
        });
    }
}

