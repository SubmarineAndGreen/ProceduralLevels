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
    [SerializeField] int structurePlaceholderIndex;
    [HideInInspector] public string pipesSampleFileName;


    void Start() {
        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

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

        wfcRunner = GetComponent<WfcRunner>();
        SamplerResult pipesSample = SamplerResult.loadFromFile($"{Application.dataPath}/SamplerSaves/{pipesSampleFileName}");

        var wfcTiles = createWfcTiles(pipesSample.uniqueTiles);

        List<ITileConstraint> wfcConstraints = new List<ITileConstraint>();

        if (pathConstraint) {
            wfcConstraints.Add(new PathConstraint(tilesWithoutEmpty(pipesSample.uniqueTiles,
                                                                    tileSets[TILESET_PIPES],
                                                                    wfcTiles)));
        }

        addStructureConstraints(tileIndices, wfcConstraints, wfcTiles);


        int[,,] generatedTiles;
        bool generationSuccess = wfcRunner.runAdjacentModel(out generatedTiles,
                                                            generatedGridDimensions,
                                                            tileSets[TILESET_PIPES],
                                                            pipesSample.uniqueTiles,
                                                            pipesSample.rules,
                                                            wfcTiles,
                                                            wfcConstraints);

        if (!generationSuccess) {
            Debug.LogError("WFC failed!");
            // return;
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

        initializeNavigation(tileIndices, tileSetIndices);
        initializeEnemyManager(tileIndices, tileSetIndices);

        levelGrid.tileIndices = tileIndices;
        levelGrid.tileRotations = tileRotations;
        levelGrid.tileSetIndices = tileSetIndices;
        levelGrid.tileObjects = tileObjects;
        levelGrid.rebuildGrid();

        Debug.Log("Level created in: " + stopwatch.ElapsedMilliseconds + "ms");
        stopwatch.Stop();
    }

    private void capOffTileEnds(Grid3D<int> tiles, Grid3D<int> rotations, Grid3D<int> tileSetIndices, SamplerResult pipesSample) {

        ConnectionData[] connectionData = pipesSample.connections;

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

    private void initializeNavigation(Grid3D<int> tileIndices, Grid3D<int> tileSetIndices) {
        Vector3Int fullDimensions = tileIndices.dimensions;
        int[,,] inputDistanceField = new int[fullDimensions.x, fullDimensions.y, fullDimensions.z];
        int blockedTile = -1;

        tileIndices.forEach((x, y, z, tileIndex) => {
            if (tileIndex == TileGrid.TILE_EMPTY || tileSetIndices.at(x, y, z) != TILESET_PIPES || tileIndex == tileSets[TILESET_PIPES].emptyTileIndex) {
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

    private int addRoomConstraints(Grid3D<int> tileIndices,
                                   Vector3Int position,
                                   Vector3Int dimensions,
                                   List<ITileConstraint> wfcConstraints,
                                   Dictionary<int, DeBroglie.Tile> wfcTiles) {
        int constrainedTileCount = 0;
        int structureTile = TileUtils.tileIndexToModelIndex(structurePlaceholderIndex, TileGrid.NO_ROTATION);
        for (int xOffset = 0; xOffset < dimensions.x; xOffset++) {
            for (int yOffset = 0; yOffset < dimensions.y; yOffset++) {
                for (int zOffset = 0; zOffset < dimensions.z; zOffset++) {
                    wfcConstraints.Add(new FixedTileConstraint() {
                        Tiles = new DeBroglie.Tile[] { wfcTiles[structureTile] },
                        Point = new Point(position.x + xOffset, position.y + yOffset, position.z + zOffset)
                    });
                    constrainedTileCount++;
                }
            }
        }
        return constrainedTileCount;
    }

    private void addStructureConstraints(Grid3D<int> tileIndices, List<ITileConstraint> wfcConstraints, Dictionary<int, DeBroglie.Tile> wfcTiles) {
        int structureTileCount = 0;
        structureTileCount += addRoomConstraints(tileIndices, new Vector3Int(0, 0, 0), new Vector3Int(3, 3, 3), wfcConstraints, wfcTiles);

        wfcConstraints.Add(new CountConstraint() {
            Comparison = CountComparison.Exactly,
            Count = structureTileCount,
            Tiles = new HashSet<DeBroglie.Tile>() { wfcTiles[TileUtils.tileIndexToModelIndex(structurePlaceholderIndex, TileGrid.NO_ROTATION)] }
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

