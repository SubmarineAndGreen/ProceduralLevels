using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModelSampler {



    public bool ignoreEmptyTiles;

    public Adjacencies sample(TileGrid inputGrid) {

        Vector3Int dimensions = inputGrid.dimensions;
        Grid3D<int> tileIndices = inputGrid.tileIndices;
        Grid3D<int> tileRotations = inputGrid.tileRotations;

        var rules = new HashSet<TileRule>();
        var tiles = new HashSet<int>();


        tileIndices.forEach((position, tileIndex) => {
            if (!ignoreEmptyTiles || (tileIndex != TileGrid.TILE_EMPTY)) {
                Tile tile = inputGrid.tileSet.tiles[tileIndex];
                int rotations = Tile.symmetryToNumberOfRotations[tile.symmetry];
                int currentRotation = tileRotations.at(position);
                for (int i = 0; i < rotations; i++) {
                    tiles.Add(TileUtils.tileIndexToModelIndex(
                        tileIndex,
                        currentRotation
                    ));
                    currentRotation = getNextRotation(tile, currentRotation);
                }

                scanNeighbors(position);
            }
        });

        void scanNeighbors(Vector3Int source) {
            Vector3Int dimensions = inputGrid.dimensions;
            Grid3D<int> tileIndices = inputGrid.tileIndices;
            Grid3D<int> tileRotations = inputGrid.tileRotations;

            foreach (var item in SamplerUtils.DirectionsToVectors) {
                Vector3Int offset = item.Value;
                Directions3D direction = item.Key;

                Vector3Int destination = source + offset;

                if (SamplerUtils.isInBounds(dimensions, destination)) {
                    //ignore constraints where some tile is an empty tile
                    if (ignoreEmptyTiles && (tileIndices.at(destination) == TileGrid.TILE_EMPTY || tileIndices.at(source) == TileGrid.TILE_EMPTY)) {
                        continue;
                    }

                    Tile sourceTile = inputGrid.tileSet.tiles[tileIndices.at(source)];
                    Tile destinationTile = inputGrid.tileSet.tiles[tileIndices.at(destination)];

                    int sourceRotation = tileRotations.at(source);
                    int destinationRotation = tileRotations.at(destination);
                    Directions3D currentDirection = direction;

                    // RotationalSymmetry sym = sourceTile.symmetry;
                    // registerRule(source, 0, destination, 0, direction);
                    // int nextRotation = sourceRotation;

                    const int sides = 4;
                    
                    // up and down
                    if (direction == Directions3D.UP || direction == Directions3D.DOWN) {
                        int destStartingRotation = destinationRotation;
                        for (int i = 0; i < sides; i++) {
                            destinationRotation = destStartingRotation;
                            for (int j = 0; j < sides; j++) {
                                registerRule(source, sourceRotation, destination, destinationRotation, direction);
                                getNextRotation(destinationTile, destinationRotation);
                            }
                            sourceRotation = getNextRotation(sourceTile, sourceRotation);
                        }
                    // xz plane
                    } else {
                        for (int i = 0; i < sides; i++) {
                            registerRule(source, sourceRotation, destination, destinationRotation, currentDirection);
                            sourceRotation = getNextRotation(sourceTile, sourceRotation);
                            destinationRotation = getNextRotation(destinationTile, destinationRotation);
                            currentDirection = SamplerUtils.nextDirectionClockwise[currentDirection];
                        }
                    }

                    // rules.Add(new TileRule(
                    //     TileUtils.tileIndexToModelIndex(
                    //         tileIndices.at(source),
                    //         tileRotations.at(source)
                    //     ),
                    //     TileUtils.tileIndexToModelIndex(
                    //         tileIndices.at(destination),
                    //         tileRotations.at(destination)
                    //     ),
                    //     direction
                    // ));
                }
            }
        }

        return new Adjacencies() {
            rules = rules.ToList(),
            uniqueTiles = tiles.ToList()
        };

        //returns int for new rotation going clockwise (0 -> 90 -> ...)
        int getNextRotation(Tile tile, int rotation) {
            rotation += 1;
            int maxRotations = Tile.symmetryToNumberOfRotations[tile.symmetry];
            if (rotation >= maxRotations) {
                rotation = 0;
            }
            return rotation;
        }

        void registerRule(Vector3Int source, int sourceRotation,
         Vector3Int destination, int destinationRotation, Directions3D direction) {

            int sourceIndex = tileIndices.at(source);
            int destIndex = tileIndices.at(destination);

            rules.Add(new TileRule(
                TileUtils.tileIndexToModelIndex(
                    sourceIndex,
                    sourceRotation
                ),
                TileUtils.tileIndexToModelIndex(
                    destIndex,
                    destinationRotation
                ),
                direction
            ));
        }
    }


}

public struct Adjacencies {
    public List<TileRule> rules;
    public List<int> uniqueTiles;
}



