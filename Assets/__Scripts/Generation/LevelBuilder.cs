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
    public StructureSet structureSet;
    public int seed;
    public const int NO_SEED = -1;
    private WfcRunner wfcRunner;
    [SerializeField] private TileGrid levelGrid;
    [SerializeField] Vector3Int levelDimensions;
    [HideInInspector] public Vector3Int fullDimensions;
    [SerializeField] bool scaleAfterGeneration;
    public float scaleFactor;
    public TileSet[] tileSets;
    [SerializeField] private bool pathConstraint = true;
    private const int TILESET_MAIN = 0;

    [Header("Structures")]
    [SerializeField] bool generateStructures;
    [SerializeField] Tile structurePlaceholderTile;
    [SerializeField] Tile verticalPipeTile;
    [SerializeField] Tile horizontalPipeTile;
    [SerializeField] Tile emptyTile;
    [Header("Decorations")]
    [SerializeField] GameObject stairsPrefab;
    [SerializeField] GameObject walkwayPrefab;
    [SerializeField] GameObject windowPrefab;


    [HideInInspector] public string pipesSampleFileName;

    public void generateLevel() {
        System.Diagnostics.Stopwatch allStopwatch = System.Diagnostics.Stopwatch.StartNew();

        if (seed != NO_SEED) {
            UnityEngine.Random.InitState(seed);
        }

        levelGrid.tileSets = tileSets;

        //leave 1 tile of margin on each side for ending hole blocking tiles
        Vector3Int oneTileMargin = Vector3Int.one * 2;
        fullDimensions = levelDimensions + oneTileMargin;

        levelGrid.dimensions = fullDimensions;

        Grid3D<int> tileIndices = new Grid3D<int>(fullDimensions);
        tileIndices.updateEach(_ => TileGrid.TILE_EMPTY);

        Grid3D<int> tileRotations = new Grid3D<int>(fullDimensions);
        Grid3D<int> tileSetIndices = new Grid3D<int>(fullDimensions);
        Grid3D<GameObject> tileObjects = new Grid3D<GameObject>(fullDimensions);

        System.Diagnostics.Stopwatch wfcStopwatch = System.Diagnostics.Stopwatch.StartNew();
        wfcRunner = GetComponent<WfcRunner>();
        SamplerResult samplerResult = SamplerResult.loadFromFile($"{Application.dataPath}/SamplerSaves/{pipesSampleFileName}");

        var wfcTiles = createWfcTiles(samplerResult.uniqueTiles);

        List<ITileConstraint> wfcConstraints = new List<ITileConstraint>();

        Vector3Int testPos = Vector3Int.one * 3;
        int testStructure = 0;

        if (generateStructures) {
            createStructureConstraints(testPos, testStructure, wfcConstraints, wfcTiles);
            instantiateStructure(testPos, testStructure);
        } else {
            int index = tileSets[TILESET_MAIN].getTileIndexFromTileObject(structurePlaceholderTile);
            // Debug.Log(index);
            wfcConstraints.Add(new CountConstraint() {
                Comparison = CountComparison.Exactly,
                Count = 0,
                Tiles = new HashSet<DeBroglie.Tile>() { wfcTiles[TileUtils.tileIndexToModelIndex(index, 0)] }
            });
        }


        if (pathConstraint) {
            wfcConstraints.Add(new PathConstraint(tilesWithoutEmpty(samplerResult.uniqueTiles,
                                                                    tileSets[TILESET_MAIN],
                                                                    wfcTiles)));
        }

        createEmptyBorderConstraint(wfcConstraints, wfcTiles);

        int[,,] generatedTiles;
        bool generationSuccess = wfcRunner.runAdjacentModel(out generatedTiles,
                                                            fullDimensions,
                                                            tileSets[TILESET_MAIN],
                                                            samplerResult.uniqueTiles,
                                                            samplerResult.rules,
                                                            wfcTiles,
                                                            wfcConstraints,
                                                            seed);

        wfcStopwatch.Stop();

        if (!generationSuccess) {
            Debug.LogError("WFC failed!");
        }

        Dictionary<int, List<Vector3Int>> generatedTilePositionsByIndex = new Dictionary<int, List<Vector3Int>>();

        setGeneratedTiles(generatedTiles, generatedTilePositionsByIndex, tileIndices, tileRotations, tileSetIndices);

        levelGrid.tileIndices = tileIndices;
        levelGrid.tileRotations = tileRotations;
        levelGrid.tileSetIndices = tileSetIndices;
        levelGrid.tileObjects = tileObjects;
        levelGrid.rebuildGrid();

        placeStairs(levelGrid, samplerResult);
        placeWindows(levelGrid, samplerResult);

        System.Diagnostics.Stopwatch navigationStopwatch = System.Diagnostics.Stopwatch.StartNew();
        initializeNavigation(tileIndices, tileSetIndices);
        navigationStopwatch.Stop();

        List<Vector3Int> pipeTilePositions = getAllPipeTiles(tileIndices, tileSetIndices);
        EnemyManager enemyManager = EnemyManager.instance;
        enemyManager.validSpawningTiles = pipeTilePositions;

        if (scaleAfterGeneration) {
            levelGrid.transform.localScale = Vector3.one * scaleFactor;
        }

        allStopwatch.Stop();
        Debug.Log($"Level created in: {allStopwatch.ElapsedMilliseconds}ms, wfc/constraints:{wfcStopwatch.ElapsedMilliseconds}ms, navigation:{navigationStopwatch.ElapsedMilliseconds}ms");
    }

    private void placeWindows(TileGrid levelGrid, SamplerResult samplerResult) {
        int emptyTileIndex = tileSets[TILESET_MAIN].getTileIndexFromTileObject(emptyTile);
        // Directions3D windowDirection = Directions3D.FORWARD;
        bool placedWindowLastTile = false;
        Directions3D windowDirection = Directions3D.UP;

        ConnectionData[] connectionData = samplerResult.connections;
        //iterate through xz rows
        for (int y = 0; y < fullDimensions.y; y++) {
            for (int x = 0; x < fullDimensions.x; x++) {
                placedWindowLastTile = false;
                for (int z = 0; z < fullDimensions.z; z++) {
                    Vector3 currentPosition = new Vector3(x, y, z);
                    int tileIndex = levelGrid.tileIndices.at(x, y, z);
                    int tileRotation = levelGrid.tileRotations.at(x, y, z);

                    bool isWallOnRight = false;
                    bool isWallOnLeft = false;
                    bool isStairsTile = false;

                    bool changedDirection = false;
                    bool noDirection = false;

                    Vector3 offset = new Vector3(-0.5f, 0, -0.5f);

                    if (tileIndex == emptyTileIndex) {
                        placedWindowLastTile = false;
                    } else {
                        isWallOnRight = !connectionData[tileIndex].canConnectFromDirection(Directions3D.RIGHT, tileRotation);
                        isWallOnLeft = !connectionData[tileIndex].canConnectFromDirection(Directions3D.LEFT, tileRotation);
                        isStairsTile = connectionData[tileIndex].canConnectFromDirection(Directions3D.UP, tileRotation);

                        if (isStairsTile) {
                            if (isWallOnRight) {
                                windowDirection = Directions3D.RIGHT;
                            } else {
                                noDirection = true;
                            }
                        } else {
                            if (placedWindowLastTile) {
                                if ((windowDirection == Directions3D.RIGHT && !isWallOnRight)
                                    || (windowDirection == Directions3D.LEFT && !isWallOnLeft)) {
                                    noDirection = true;
                                }
                            } else {
                                noDirection = true;
                            }

                            if (noDirection) {
                                noDirection = false;
                                changedDirection = true;

                                if (isWallOnLeft && isWallOnRight) {
                                    switch (UnityEngine.Random.Range(0, 1)) {
                                        case 0:
                                            windowDirection = Directions3D.RIGHT;
                                            break;
                                        case 1:
                                            windowDirection = Directions3D.LEFT;
                                            break;

                                    }
                                } else if (isWallOnRight) {
                                    windowDirection = Directions3D.RIGHT;
                                } else if (isWallOnLeft) {
                                    windowDirection = Directions3D.LEFT;
                                } else {
                                    noDirection = true;
                                    changedDirection = false;
                                }
                            }
                        }




                        if (!noDirection) {
                            // Debug.Log(changedDirection);
                            // Debug.Log(windowDirection);

                            Quaternion windowRotation = Quaternion.Euler(0, ((int)windowDirection - 2) * 90, 0);
                            GameObject windowObject = Instantiate(windowPrefab, currentPosition + offset, windowRotation);
                            windowObject.transform.SetParent(levelGrid.transform);

                            if (placedWindowLastTile && !changedDirection) {
                                Vector3 previousOffest = new Vector3(0, 0, -0.5f);
                                GameObject previousWindowObject = Instantiate(windowPrefab, currentPosition + offset + previousOffest, windowRotation);
                                previousWindowObject.transform.SetParent(levelGrid.transform);
                            }

                            placedWindowLastTile = true;
                        } else {
                            placedWindowLastTile = false;
                        }
                    }
                }
            }
        }


        //iterate thorugh zx rows
        // for (int y = 0; y < levelDimensions.y; y++) {
        //     for (int z = 0; z < levelDimensions.z; z++) {
        //         for (int x = 0; x < levelDimensions.x; x++) {

        //         }
        //     }
        // }
    }

    private void createEmptyBorderConstraint(List<ITileConstraint> wfcConstraints, Dictionary<int, DeBroglie.Tile> wfcTiles) {
        int emptyTileIndex = tileSets[TILESET_MAIN].getTileIndexFromTileObject(emptyTile);
        DeBroglie.Tile emptyWfcTile = wfcTiles[TileUtils.tileIndexToModelIndex(emptyTileIndex, TileGrid.NO_ROTATION)];
        BorderConstraint emptyBorderConstraint = new BorderConstraint();
        emptyBorderConstraint.Sides = BorderSides.All;
        emptyBorderConstraint.Tiles = new DeBroglie.Tile[] { emptyWfcTile };
        wfcConstraints.Add(emptyBorderConstraint);
    }

    private void initializeNavigation(Grid3D<int> tileIndices, Grid3D<int> tileSetIndices) {
        Vector3Int fullDimensions = tileIndices.dimensions;
        int[,,] inputDistanceField = new int[fullDimensions.x, fullDimensions.y, fullDimensions.z];
        int blockedTile = -1;

        tileIndices.forEach((x, y, z, tileIndex) => {
            if (tileIndex == TileGrid.TILE_EMPTY
                || tileSetIndices.at(x, y, z) != TILESET_MAIN
                || tileIndex == tileSets[TILESET_MAIN].emptyTileIndex) {
                inputDistanceField[x, y, z] = blockedTile;
            } else {
                inputDistanceField[x, y, z] = int.MaxValue / 2;
            }
        });

        NavigationManager navigation = NavigationManager.instance;
        navigation.calculateVectorFields(inputDistanceField, blockedTile);

        navigation.tileSize = scaleFactor;
        navigation.tileGridDimensions = fullDimensions;
    }

    private List<Vector3Int> getAllPipeTiles(Grid3D<int> tileIndices, Grid3D<int> tileSetIndices) {

        List<Vector3Int> pipeTiles = new List<Vector3Int>();

        tileIndices.forEach((x, y, z, tileIndex) => {
            if (tileIndex != TileGrid.TILE_EMPTY
            && tileSetIndices.at(x, y, z) == TILESET_MAIN
            && tileIndex != tileSets[TILESET_MAIN].emptyTileIndex) {
                pipeTiles.Add(new Vector3Int(x, y, z));
            }
        });

        return pipeTiles;
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

    public void setGeneratedTiles(int[,,] generatedTiles,
                                     Dictionary<int, List<Vector3Int>> generatedTilePositionsByIndex,
                                     Grid3D<int> tileIndices,
                                     Grid3D<int> tileRotations,
                                     Grid3D<int> tileSetIndices) {
        for (int x = 0; x < fullDimensions.x; x++) {
            for (int y = 0; y < fullDimensions.y; y++) {
                for (int z = 0; z < fullDimensions.z; z++) {
                    int tileIndex = TileUtils.modelIndexToTileIndex(generatedTiles[x, y, z]);

                    if (!generatedTilePositionsByIndex.ContainsKey(tileIndex)) {
                        generatedTilePositionsByIndex[tileIndex] = new List<Vector3Int>();
                    }

                    generatedTilePositionsByIndex[tileIndex].Add(new Vector3Int(x, y, z));

                    tileIndices.set(x, y, z, tileIndex);
                    tileRotations.set(x, y, z, TileUtils.modelIndexToRotation(generatedTiles[x, y, z]));
                    tileSetIndices.set(x, y, z, TILESET_MAIN);
                }
            }
        }
    }

    private void createStructureConstraints(Vector3Int position, int setIndex, List<ITileConstraint> wfcConstraints, Dictionary<int, DeBroglie.Tile> wfcTiles) {

        Structure structure = structureSet.frequencies[setIndex].structure;
        List<StructureTile> structureTiles = structure.getTilesCollection().tiles;
        int structurePlaceholderIndex = tileSets[TILESET_MAIN].getTileIndexFromTileObject(structurePlaceholderTile);
        int pipeUpTileIndex = tileSets[TILESET_MAIN].getTileIndexFromTileObject(verticalPipeTile);
        int pipeHorizontalTileIndex = tileSets[TILESET_MAIN].getTileIndexFromTileObject(horizontalPipeTile);

        int structureCount = 0;
        HashSet<Vector3Int> fixedTiles = new HashSet<Vector3Int>();

        foreach (StructureTile tile in structureTiles) {
            structureCount++;
            Vector3Int structureTilePosition = position + tile.position;
            // Debug.Log(structureTilePosition);
            fixedTiles.Add(structureTilePosition);

            wfcConstraints.Add(new FixedTileConstraint() {
                Point = new Point(structureTilePosition.x, structureTilePosition.y, structureTilePosition.z),
                Tiles = new DeBroglie.Tile[] {
                    wfcTiles[TileUtils.tileIndexToModelIndex(structurePlaceholderIndex, TileGrid.NO_ROTATION)]
                }
            });
        }

        foreach (StructureTile tile in structureTiles) {
            foreach (Directions3D direction in DirectionUtils.allDirections) {
                Vector3Int tilePosition = position + tile.position + DirectionUtils.DirectionsToVectors[direction];

                if (fixedTiles.Contains(tilePosition)) {
                    continue;
                } else {
                    fixedTiles.Add(tilePosition);
                }

                if (tile.isSideOpen(direction)) {
                    DeBroglie.Tile tileToFix;

                    switch (direction) {
                        case Directions3D.UP:
                        case Directions3D.DOWN:
                            tileToFix = wfcTiles[TileUtils.tileIndexToModelIndex(pipeUpTileIndex, TileGrid.NO_ROTATION)];
                            break;
                        case Directions3D.FORWARD:
                        case Directions3D.BACK:
                            tileToFix = wfcTiles[TileUtils.tileIndexToModelIndex(pipeHorizontalTileIndex, 1)];
                            break;
                        case Directions3D.RIGHT:
                        case Directions3D.LEFT:
                            tileToFix = wfcTiles[TileUtils.tileIndexToModelIndex(pipeHorizontalTileIndex, TileGrid.NO_ROTATION)];
                            break;
                    }

                    // Debug.Log(tilePosition);
                    wfcConstraints.Add(new FixedTileConstraint() {
                        Point = new Point(tilePosition.x, tilePosition.y, tilePosition.z),
                        Tiles = new DeBroglie.Tile[] {
                            tileToFix
                        }
                    });
                } else {
                    int emptyTileIndex = TileUtils.tileIndexToModelIndex(tileSets[TILESET_MAIN].emptyTileIndex, TileGrid.NO_ROTATION);

                    wfcConstraints.Add(new FixedTileConstraint() {
                        Point = new Point(tilePosition.x, tilePosition.y, tilePosition.z),
                        Tiles = new DeBroglie.Tile[] {
                            wfcTiles[emptyTileIndex]
                        }
                    });
                }
            }
        }

        wfcConstraints.Add(new CountConstraint() {
            Comparison = CountComparison.Exactly,
            Count = structureCount,
            Tiles = new HashSet<DeBroglie.Tile>() {
                    wfcTiles[TileUtils.tileIndexToModelIndex(structurePlaceholderIndex, TileGrid.NO_ROTATION)]
            }
        });
    }

    private void instantiateStructure(Vector3Int position, int setIndex) {
        Vector3 tileOffset = new Vector3(0, -0.5f, 0);
        GameObject prefabToSpawn = structureSet.frequencies[setIndex].structure.structurePrefab;
        GameObject structure = Instantiate(prefabToSpawn, position.toVector3() + tileOffset /* scaleFactor */, Quaternion.identity);
        structure.transform.SetParent(levelGrid.transform);
    }

    private Vector3Int randomVector3Int(Vector3Int min, Vector3Int max) {
        return new Vector3Int(
            UnityEngine.Random.Range(min.x, max.x),
            UnityEngine.Random.Range(min.y, max.y),
            UnityEngine.Random.Range(min.z, max.z)
        );
    }

    private void placeStairs(TileGrid grid, SamplerResult samplerResult) {
        Vector3 offset = new Vector3(0.5f, 0, 0.5f);
        grid.tileIndices.forEach((Vector3Int position, int index) => {
            if (samplerResult.connections[index].canConnectFromDirection(Directions3D.UP, TileGrid.NO_ROTATION)) {
                GameObject stairsObject = Instantiate(stairsPrefab, position.toVector3() - offset, Quaternion.identity);
                stairsObject.transform.SetParent(levelGrid.transform);
                Vector3Int tileAbovePosition = position + Vector3Int.up;
                if (grid.tileIndices.inBounds(position + Vector3Int.up)) {
                    int tileAbove = grid.tileIndices.at(tileAbovePosition);
                    if (samplerResult.connections[tileAbove].canConnectFromDirection(Directions3D.LEFT, grid.tileRotations.at(tileAbovePosition))) {
                        GameObject walkwayObject = Instantiate(walkwayPrefab, position.toVector3() - offset, Quaternion.identity);
                        walkwayObject.transform.SetParent(levelGrid.transform);
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
        EditorGUILayout.Space(10);
        EditorUtils.filePicker("Pipes model", builder.pipesSampleFileName, $"{Application.dataPath}/SamplerSaves", (fileName) => {
            builder.pipesSampleFileName = fileName as string;
        });
    }
}

