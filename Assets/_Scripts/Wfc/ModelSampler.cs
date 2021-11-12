using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModelSampler {

    public bool ignoreEmptyTiles;

    public (List<TileRule>, List<int>) sample(TileGrid inputGrid) {
        Vector3Int dimensions = inputGrid.dimensions;
        Grid3D<int> tileIndices = inputGrid.tileIndices;
        Grid3D<int> tileRotations = inputGrid.tileRotations;

        var constraints = new HashSet<TileRule>();
        var tiles = new HashSet<int>();


        tileIndices.forEach((position, tileIndex) => {
            if (!ignoreEmptyTiles || (tileIndex != TileGrid.TILE_EMPTY)) {
                tiles.Add(TileUtils.tileIndexToModelIndex(
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

            foreach (var item in SamplerUtils.DirectionsToVectors) {
                Vector3Int offset = item.Value;
                Directions3D direction = item.Key;

                Vector3Int neighborPosition = centerTilePosition + offset;

                if (SamplerUtils.isInBounds(dimensions, neighborPosition)) {
                    //ignore constraints where some tile is an empty tile
                    if (ignoreEmptyTiles && (tileIndices.at(neighborPosition) == TileGrid.TILE_EMPTY || tileIndices.at(centerTilePosition) == TileGrid.TILE_EMPTY)) {
                        continue;
                    }

                    constraints.Add(new TileRule(
                        TileUtils.tileIndexToModelIndex(
                            tileIndices.at(centerTilePosition),
                            tileRotations.at(centerTilePosition)
                        ),
                        TileUtils.tileIndexToModelIndex(
                            tileIndices.at(neighborPosition),
                            tileRotations.at(neighborPosition)
                        ),
                        direction
                    ));
                }
            }
        }

        return (constraints.ToList(), tiles.ToList());
    }
}



