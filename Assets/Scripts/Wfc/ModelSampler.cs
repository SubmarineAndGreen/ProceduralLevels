using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModelSampler : MonoBehaviour {
    public const string SAVE_FOLDER = "WfcModels";
    [SerializeField] TileGrid inputGrid;
    [SerializeField] bool ignoreEmptyTiles;

    public SimpleTiledModel run() {
        return simpleTiledModel();
    }

    private SimpleTiledModel simpleTiledModel() {
        Vector3Int dimensions = inputGrid.dimensions;
        Grid3D<int> tileIndices = inputGrid.tileIndices;
        Grid3D<int> tileRotations = inputGrid.tileRotations;

        var constraints = new HashSet<TileRule>();
        var tileIds = new HashSet<int>();

        tileIndices.forEach((position, tileIndex) => {
            if (tileIndex != TileGrid.TILE_EMPTY) {
                tileIds.Add(SimpleTiledModel.tileIndexToModelIndex(
                    tileIndex,
                    tileRotations.at(position)
                ));
                scanNeighbors(position);
            }
        });

        void scanNeighbors(Vector3Int centerTilePosition) {
            Vector3Int dimensions = inputGrid.dimensions;
            Grid3D<int> tileIndices = inputGrid.tileIndices;
            Grid3D<int> tileRotations = inputGrid.tileRotations;

            foreach (var item in SolverUtils.DirectionsToVectors) {
                Vector3Int offset = item.Value;
                Directions3D direction = item.Key;

                Vector3Int neighborPosition = centerTilePosition + offset;

                if (SolverUtils.isInBounds(dimensions, neighborPosition)) {
                    //ignore constraints where some tile is an empty tile
                    if (ignoreEmptyTiles && (tileIndices.at(neighborPosition) == TileGrid.TILE_EMPTY || tileIndices.at(centerTilePosition) == TileGrid.TILE_EMPTY)) {
                        continue;
                    }

                    constraints.Add(new TileRule(
                        SimpleTiledModel.tileIndexToModelIndex(
                            tileIndices.at(centerTilePosition),
                            tileRotations.at(centerTilePosition)
                        ),
                        SimpleTiledModel.tileIndexToModelIndex(
                            tileIndices.at(neighborPosition),
                            tileRotations.at(neighborPosition)
                        ),
                        direction
                    ));
                }
            }
        }

        return new SimpleTiledModel(tileIds.ToList(), constraints.ToList());
    }
}



