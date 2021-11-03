using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModelSampler : MonoBehaviour {
    public const string SAVE_FOLDER = "WfcModels";
    const int nOfRotations = 4;
    [SerializeField] TileGrid inputGrid;
    [SerializeField] bool ignoreEmptyTiles;

    public WfcModel run() {
        return simpleTiledModel();
    }

    private WfcModel simpleTiledModel() {
        Vector3Int dimensions = inputGrid.dimensions;
        Array3D<int> tileIndices = inputGrid.tileIndices;
        Array3D<int> tileRotations = inputGrid.tileRotations;

        var constraints = new HashSet<GridAdjacencyConstraint>();
        var tileIds = new HashSet<int>();

        tileIndices.forEach((position, tileIndex) => {
            if (tileIndex != TileGrid.TILE_EMPTY) {
                tileIds.Add(tileIndexToModelIndex(
                    tileIndex,
                    tileRotations.at(position)
                ));
                scanNeighbors(position);
            }
        });

        void scanNeighbors(Vector3Int centerTilePosition) {
            Vector3Int dimensions = inputGrid.dimensions;
            Array3D<int> tileIndices = inputGrid.tileIndices;
            Array3D<int> tileRotations = inputGrid.tileRotations;

            foreach (var item in SolverUtils.DirectionsToVectors) {
                Vector3Int offset = item.Value;
                Directions3D direction = item.Key;

                Vector3Int neighborPosition = centerTilePosition + offset;

                if (SolverUtils.isInBounds(dimensions, neighborPosition)) {
                    //ignore constraints where some tile is an empty tile
                    if (ignoreEmptyTiles && (tileIndices.at(neighborPosition) == TileGrid.TILE_EMPTY || tileIndices.at(centerTilePosition) == TileGrid.TILE_EMPTY)) {
                        continue;
                    }

                    constraints.Add(new GridAdjacencyConstraint(
                        tileIndexToModelIndex(
                            tileIndices.at(centerTilePosition),
                            tileRotations.at(centerTilePosition)
                        ),
                        tileIndexToModelIndex(
                            tileIndices.at(neighborPosition),
                            tileRotations.at(neighborPosition)
                        ),
                        direction
                    ));
                }
            }
        }

        return new WfcModel(tileIds.ToList(), constraints.ToList());
    }

    int tileIndexToModelIndex(int index, int rotation) {
        return index * nOfRotations + rotation;
    }

    int modelIndexToTileIndex(int index) {
        return (index - (index % nOfRotations)) / nOfRotations;
    }

    int modelIndexToRotation(int index) {
        return index % nOfRotations;
    }


}



