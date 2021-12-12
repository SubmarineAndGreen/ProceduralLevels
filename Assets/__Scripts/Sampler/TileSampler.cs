using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class TileSampler : MonoBehaviour {

    public static string savePath;
    const string saveFolder = "SamplerSaves";
    [SerializeField] private TileGrid inputGrid;
    public bool ignoreEmptyTiles;
    [SerializeField] private string saveFileName;

    private void OnValidate() {
        savePath = $"{Application.dataPath}/{saveFolder}";
    }


    public void sample() {

        Vector3Int dimensions = inputGrid.dimensions;
        Grid3D<int> tileIndices = inputGrid.tileIndices;
        Grid3D<int> tileRotations = inputGrid.tileRotations;

        var rules = new HashSet<TileRule>();
        var tiles = new HashSet<int>();
        var connections = new ConnectionData[inputGrid.tileSets[inputGrid.currentTilesetIndex].tiles.Count];

        for (int i = 0; i < connections.Length; i++) {
            connections[i] = new ConnectionData();
            connections[i].setAllTrue();
        }


        tileIndices.forEach((position, tileIndex) => {
            if (!ignoreEmptyTiles || (tileIndex != TileGrid.TILE_EMPTY)) {
                Tile tile = inputGrid.tileSets[inputGrid.currentTilesetIndex].tiles[tileIndex];
                int rotations = Tile.symmetryToNumberOfRotations[tile.symmetry];
                int currentRotation = tileRotations.at(position);
                //save all unique tile indices (model indices with rotation coded in)
                for (int i = 0; i < rotations; i++) {
                    tiles.Add(TileUtils.tileIndexToModelIndex(
                        tileIndex,
                        currentRotation
                    ));
                    currentRotation = getNextRotation(tile, currentRotation);
                }
            }

            scanNeighbors(position);
        });

        void scanNeighbors(Vector3Int source) {
            Vector3Int dimensions = inputGrid.dimensions;
            Grid3D<int> tileIndices = inputGrid.tileIndices;
            Grid3D<int> tileRotations = inputGrid.tileRotations;

            foreach (var item in DirectionUtils.DirectionsToVectors) {
                Vector3Int offset = item.Value;
                Directions3D direction = item.Key;

                Vector3Int destination = source + offset;

                if (inBounds(dimensions, destination)) {
                    //skip creating rules where some tile is an empty tile
                    if (ignoreEmptyTiles) {
                        if (tileIndices.at(source) == TileGrid.TILE_EMPTY || tileIndices.at(destination) == TileGrid.TILE_EMPTY) {
                            continue;
                        }
                    }

                    Tile sourceTile = inputGrid.tileSets[inputGrid.currentTilesetIndex].tiles[tileIndices.at(source)];
                    Tile destinationTile = inputGrid.tileSets[inputGrid.currentTilesetIndex].tiles[tileIndices.at(destination)];

                    int sourceRotation = tileRotations.at(source);
                    int destinationRotation = tileRotations.at(destination);
                    Directions3D currentDirection = direction;

                    const int sides = 4;

                    // up and down, rotates both tiles and samples up to 16 combinations
                    // example rotations Source Tile, Destination Tile: 0, 0; 0, 1; 1, 0; 1, 1; 2, 0; etc.
                    if (direction == Directions3D.UP || direction == Directions3D.DOWN) {

                        if (tileIndices.at(destination) == inputGrid.tileSets[inputGrid.currentTilesetIndex].emptyTileIndex) {
                            banConnection(connections, tileIndices.at(source), tileRotations.at(source), direction);
                        }

                        int destStartingRotation = destinationRotation;
                        for (int i = 0; i < sides; i++) {
                            destinationRotation = destStartingRotation;
                            for (int j = 0; j < sides; j++) {
                                registerRule(source, sourceRotation, destination, destinationRotation, direction);
                                getNextRotation(destinationTile, destinationRotation);
                            }
                            sourceRotation = getNextRotation(sourceTile, sourceRotation);
                        }

                    } else {
                        // xz plane,
                        // rotate source tile and keep destination tile "glued" to the same side of source tile
                        // imagine dest tile is parented to src tile
                        // that means dest tile is also rotated
                        //XDX  X0X  XXX  XXX  XXX
                        //XSX  X0X  X11  X2X  33X
                        //XXX  XXX  XXX  X2X  XXX
                        // ^ all resulting rules if both tiles have no symmetry, view from top
                        // 0, 1, 2... are rotations
                        // if some tile has symmetry rotations are looped or tile is not rotated at all
                        // for example src has only 1 rotation (0) and dest 2 (0, 1):
                        //XDX  X0X  XXX  XXX  XXX
                        //XSX  X0X  X01  X0X  10X
                        //XXX  XXX  XXX  X0X  XXX
                        for (int i = 0; i < sides; i++) {
                            registerRule(source, sourceRotation, destination, destinationRotation, currentDirection);

                            if (tileIndices.at(destination) == inputGrid.tileSets[inputGrid.currentTilesetIndex].emptyTileIndex) {
                                banConnection(connections, tileIndices.at(source), sourceRotation, currentDirection);
                            }

                            sourceRotation = getNextRotation(sourceTile, sourceRotation);
                            destinationRotation = getNextRotation(destinationTile, destinationRotation);
                            currentDirection = DirectionUtils.nextDirectionClockwise[currentDirection];
                        }
                    }
                }
            }
        }

        var save = new SamplerResult() {
            tileSet = inputGrid.tileSets[inputGrid.currentTilesetIndex].name,
            rules = rules.ToList(),
            uniqueTiles = tiles.ToList(),
            connections = connections
        };

        save.saveToFile($"{savePath}/{saveFileName}.json");

        //returns int for given rotation + 90deg
        //takes into account tile symmetries
        //because symmetric tiles have less meaningful rotations
        int getNextRotation(Tile tile, int rotation) {
            rotation += 1;
            int maxRotations = Tile.symmetryToNumberOfRotations[tile.symmetry];
            if (rotation >= maxRotations) {
                rotation = 0;
            }
            return rotation;
        }

        //create tile adjacency rule
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

        void banConnection(ConnectionData[] connections, int tileIndex, int tileRotation, Directions3D direction) {
            connections[tileIndex].banConnection(direction, tileRotation);
        }
    }

    public bool inBounds(Vector3Int dimensions, Vector3Int position) {
        if (position.x >= dimensions.x || position.y >= dimensions.y || position.z >= dimensions.z) {
            return false;
        }
        if (position.x < 0 || position.y < 0 || position.z < 0) {
            return false;
        }
        return true;
    }
}



[CustomEditor(typeof(TileSampler))]
public class ModelSamplerEditor : Editor {

    TileSampler modelSampler;
    private void OnEnable() {
        modelSampler = target as TileSampler;
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorUtils.guiButton("Sample", () => modelSampler.sample());
    }
}


