using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using DeBroglie;
using DeBroglie.Constraints;
using System;

[RequireComponent(typeof(WfcRunner))]
public class LevelBuilder : MonoBehaviour {
    private WfcRunner wfcRunner;
    [SerializeField] private TileGrid levelGrid;
    [SerializeField] Vector3Int generatedGridDimensions;
    Vector3Int fullDimensions;
    public TileSet[] tileSets;
    [SerializeField] private bool pathConstraint = true;
    private const int TILESET_PIPES = 0;
    private const int TILESET_CAPS = 1;
    [Header("Tile Caps")]
    [SerializeField] int upTileCapIndex;
    [SerializeField] int downTileCapIndex;
    [SerializeField] int sideTileCapIndex;
    [Space(10)]
    [SerializeField] int roomTileIndex;
    [SerializeField] private CuboidStructure[] fixedRooms;
    [SerializeField] private FullLengthTunnel[] fixedTunnels;
    [HideInInspector] public string pipesSampleFileName;


    void Start() {
        System.Diagnostics.Stopwatch allStopwatch = System.Diagnostics.Stopwatch.StartNew();

        levelGrid.tileSets = tileSets;

        //leave 1 tile of margin on each side for ending hole blocking tiles
        Vector3Int oneTileMargin = Vector3Int.one * 2;
        fullDimensions = generatedGridDimensions + oneTileMargin;

        levelGrid.dimensions = fullDimensions;

        Grid3D<int> tileIndices = new Grid3D<int>(fullDimensions);
        tileIndices.updateEach(_ => TileGrid.TILE_EMPTY);

        Grid3D<int> tileRotations = new Grid3D<int>(fullDimensions);
        Grid3D<int> tileSetIndices = new Grid3D<int>(fullDimensions);
        Grid3D<GameObject> tileObjects = new Grid3D<GameObject>(fullDimensions);

        System.Diagnostics.Stopwatch wfcStopwatch = System.Diagnostics.Stopwatch.StartNew();
        wfcRunner = GetComponent<WfcRunner>();
        SamplerResult pipesSample = SamplerResult.loadFromFile($"{Application.dataPath}/SamplerSaves/{pipesSampleFileName}");

        var wfcTiles = createWfcTiles(pipesSample.uniqueTiles);

        List<ITileConstraint> wfcConstraints = new List<ITileConstraint>();

        if (pathConstraint) {
            wfcConstraints.Add(new PathConstraint(tilesWithoutEmpty(pipesSample.uniqueTiles,
                                                                    tileSets[TILESET_PIPES],
                                                                    wfcTiles)));
        }

        Vector3Int[] roomTilePositions = addStructureConstraints(tileIndices, wfcConstraints, wfcTiles);



        int[,,] generatedTiles;
        bool generationSuccess = wfcRunner.runAdjacentModel(out generatedTiles,
                                                            generatedGridDimensions,
                                                            tileSets[TILESET_PIPES],
                                                            pipesSample.uniqueTiles,
                                                            pipesSample.rules,
                                                            wfcTiles,
                                                            wfcConstraints);

        wfcStopwatch.Stop();

        if (!generationSuccess) {
            Debug.LogError("WFC failed!");
        }

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


        capOffTileEnds(tileIndices, tileRotations, tileSetIndices, pipesSample);

        levelGrid.tileIndices = tileIndices;
        levelGrid.tileRotations = tileRotations;
        levelGrid.tileSetIndices = tileSetIndices;
        levelGrid.tileObjects = tileObjects;
        levelGrid.rebuildGrid();

        createRoomTileWalls(roomTilePositions, tileIndices, tileObjects);

        System.Diagnostics.Stopwatch navigationStopwatch = System.Diagnostics.Stopwatch.StartNew();
        initializeNavigation(tileIndices, tileSetIndices);
        navigationStopwatch.Stop();

        initializeEnemyManager(tileIndices, tileSetIndices);


        allStopwatch.Stop();
        Debug.Log($"Level created in: {allStopwatch.ElapsedMilliseconds}ms, wfc/constraints:{wfcStopwatch.ElapsedMilliseconds}ms, navigation:{navigationStopwatch.ElapsedMilliseconds}ms");
    }

    private void capOffTileEnds(Grid3D<int> tiles, Grid3D<int> rotations, Grid3D<int> tileSetIndices, SamplerResult pipesSample) {

        ConnectionData[] connectionData = pipesSample.connections;

        tiles.forEach((position, tile) => {
            if (tile != TileGrid.TILE_EMPTY && tileSetIndices.at(position) != TILESET_CAPS) {
                var possibleConnections = connectionData[tile];
                foreach (Directions3D direction in DirectionUtils.allDirections) {
                    bool isConnectionPossible = possibleConnections.canConnectFromDirection(direction, rotations.at(position));
                    if (isConnectionPossible) {
                        Vector3Int offset = DirectionUtils.DirectionsToVectors[direction];
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

    private void initializeNavigation(Grid3D<int> tileIndices, Grid3D<int> tileSetIndices) {
        Vector3Int fullDimensions = tileIndices.dimensions;
        int[,,] inputDistanceField = new int[fullDimensions.x, fullDimensions.y, fullDimensions.z];
        int blockedTile = -1;

        tileIndices.forEach((x, y, z, tileIndex) => {
            if (tileIndex == TileGrid.TILE_EMPTY
                || tileSetIndices.at(x, y, z) != TILESET_PIPES
                || tileIndex == tileSets[TILESET_PIPES].emptyTileIndex) {
                inputDistanceField[x, y, z] = blockedTile;
            } else {
                inputDistanceField[x, y, z] = int.MaxValue / 2;
            }
        });

        NavigationManager navigation = NavigationManager.instance;
        navigation.calculateVectorFields(inputDistanceField, blockedTile);
    }

    private void initializeEnemyManager(Grid3D<int> tileIndices, Grid3D<int> tileSetIndices) {

        EnemyManager enemyManager = EnemyManager.instance;
        List<Vector3Int> validSpawningTiles = new List<Vector3Int>();

        tileIndices.forEach((x, y, z, tileIndex) => {
            if (tileIndex != TileGrid.TILE_EMPTY
            && tileSetIndices.at(x, y, z) == TILESET_PIPES
            && tileIndex != tileSets[TILESET_PIPES].emptyTileIndex) {
                validSpawningTiles.Add(new Vector3Int(x, y, z));
            }
        });

        enemyManager.validSpawningTiles = validSpawningTiles;
    }

    private Dictionary<int, DeBroglie.Tile> createWfcTiles(List<int> uniqueTiles) {
        Dictionary<int, DeBroglie.Tile> tileObjects = new Dictionary<int, DeBroglie.Tile>();

        foreach (int tileIndex in uniqueTiles) {
            tileObjects[tileIndex] = new DeBroglie.Tile(tileIndex);
        }

        return tileObjects;
    }

    public HashSet<DeBroglie.Tile> tilesWithoutEmpty(List<int> uniqueTiles, TileSet tileSet, Dictionary<int, DeBroglie.Tile> tileObjects) {
        var result = new HashSet<DeBroglie.Tile>();
        int emptyTile = TileUtils.tileIndexToModelIndex(tileSet.emptyTileIndex, TileGrid.NO_ROTATION);
        foreach (int tileIndex in uniqueTiles) {
            if (tileIndex != emptyTile) {
                result.Add(tileObjects[tileIndex]);
            }
        }
        return result;
    }

    private int addCuboidStructureConstraints(Grid3D<int> tileIndices,
                                   CuboidStructure structure,
                                   List<ITileConstraint> wfcConstraints,
                                   Dictionary<int, DeBroglie.Tile> wfcTiles,
                                   HashSet<Vector3Int> fixedTiles) {
        int constrainedTileCount = 0;
        int structureTile = TileUtils.tileIndexToModelIndex(roomTileIndex, TileGrid.NO_ROTATION);
        for (int xOffset = 0; xOffset < structure.dimensions.x; xOffset++) {
            for (int yOffset = 0; yOffset < structure.dimensions.y; yOffset++) {
                for (int zOffset = 0; zOffset < structure.dimensions.z; zOffset++) {
                    Vector3Int currentTile = new Vector3Int(structure.position.x + xOffset,
                                                            structure.position.y + yOffset,
                                                            structure.position.z + zOffset);
                    if (!fixedTiles.Contains(currentTile)) {
                        fixedTiles.Add(currentTile);

                        wfcConstraints.Add(new FixedTileConstraint() {
                            Tiles = new DeBroglie.Tile[] { wfcTiles[structureTile] },
                            Point = new Point(currentTile.x, currentTile.y, currentTile.z)
                        });

                        constrainedTileCount++;
                    }
                }
            }
        }
        return constrainedTileCount;
    }

    private Vector3Int[] addStructureConstraints(Grid3D<int> tileIndices,
                                         List<ITileConstraint> wfcConstraints,
                                         Dictionary<int, DeBroglie.Tile> wfcTiles) {
        //data structure to check for existing fixed tile constraints and avoid duplicates
        HashSet<Vector3Int> fixedTiles = new HashSet<Vector3Int>();
        int structureTileCount = 0;

        foreach (CuboidStructure structure in fixedRooms) {
            structureTileCount += addCuboidStructureConstraints(tileIndices,
                                                                structure,
                                                                wfcConstraints,
                                                                wfcTiles,
                                                                fixedTiles);
        }

        foreach (FullLengthTunnel tunnel in fixedTunnels) {
            structureTileCount += addCuboidStructureConstraints(tileIndices,
                                                                tunnel.toCuboidStructure(tileIndices.dimensions),
                                                                wfcConstraints,
                                                                wfcTiles,
                                                                fixedTiles);
        }

        wfcConstraints.Add(new CountConstraint() {
            Comparison = CountComparison.Exactly,
            Count = structureTileCount,
            Tiles = new HashSet<DeBroglie.Tile>() {
                 wfcTiles[TileUtils.tileIndexToModelIndex(roomTileIndex, TileGrid.NO_ROTATION)]
            }
        });

        return fixedTiles.ToArray();
    }

    private void createRoomTileWalls(Vector3Int[] roomTilePositions, Grid3D<int> tileIndices, Grid3D<GameObject> tileObjects) {
        //room tile positions are offset due to 1 tile border containing tile caps
        Vector3Int borderOffset = Vector3Int.one;
        int emptyPipeTileIndex = tileSets[TILESET_PIPES].emptyTileIndex;
        foreach (Vector3Int position in roomTilePositions) {
            RoomTile roomTile = tileObjects.at(position + borderOffset).GetComponent<RoomTile>();
            foreach (Directions3D direction in DirectionUtils.allDirections) {
                Vector3Int neighbourOffset = DirectionUtils.DirectionsToVectors[direction];
                int neighbourTileIndex = tileIndices.at(position + neighbourOffset + borderOffset);
                if (neighbourTileIndex != roomTileIndex) {
                    if (neighbourTileIndex == TileGrid.TILE_EMPTY || neighbourTileIndex == emptyPipeTileIndex) {
                        roomTile.instantiateWall(direction, RoomTile.WallType.SOLID);
                    } else {
                        roomTile.instantiateWall(direction, RoomTile.WallType.OPEN);
                    }
                }
            }
        }
    }
}

[Serializable]
public struct CuboidStructure {
    public Vector3Int dimensions;
    public Vector3Int position;
}


[Serializable]
public struct FullLengthTunnel {
    public Vector3Int pointInTunnel;
    public Vector2Int dimensions;
    public TunnelDirection direction;
    public CuboidStructure toCuboidStructure(Vector3Int levelDimensions) {
        if (direction == TunnelDirection.Vertical) {
            return new CuboidStructure() {
                dimensions = new Vector3Int(dimensions.x, levelDimensions.y - 2, dimensions.y),
                position = new Vector3Int(pointInTunnel.x, 0, pointInTunnel.z)
            };
        } else if (direction == TunnelDirection.HorizontalX) {
            return new CuboidStructure() {
                dimensions = new Vector3Int(levelDimensions.x - 2, dimensions.y, dimensions.x),
                position = new Vector3Int(0, pointInTunnel.y, pointInTunnel.z)
            };
        } else {
            return new CuboidStructure() {
                dimensions = new Vector3Int(dimensions.x, dimensions.y, levelDimensions.z - 2),
                position = new Vector3Int(pointInTunnel.x, pointInTunnel.y, 0)
            };
        }
    }
    public enum TunnelDirection {
        Vertical,
        HorizontalX,
        HorizontalZ
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

