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
    [SerializeField] GameLoop gameLoop;
    public int seed;
    public const int NO_SEED = -1;
    private WfcRunner wfcRunner;
    [SerializeField] private TileGrid levelGrid;
    [SerializeField] Vector3Int generatedGridDimensions;
    [HideInInspector] public Vector3Int fullDimensions;
    [SerializeField] bool scaleAfterGeneration;
    public float scaleFactor;
    public TileSet[] tileSets;
    [SerializeField] private bool pathConstraint = true;
    private const int TILESET_PIPES = 0;
    private const int TILESET_CAPS = 1;
    [Header("Tile Caps")]
    [SerializeField] int upTileCapIndex;
    [SerializeField] int downTileCapIndex;
    [SerializeField] int sideTileCapIndex;
    [SerializeField] int structurePlaceholderIndex;
    [SerializeField] int pipeUpTileIndex;
    [SerializeField] int pipeHorizontalTileIndex;
    [SerializeField] GameObject fanPrefab;
    [SerializeField] GameObject vinePrefab;

    [Space(10)]

    [HideInInspector] public string pipesSampleFileName;
    // private const int structureTilesOffset = 64;

    // [SerializeField] private int structureCount;

    private void Start() {
        generate();
        gameLoop.enabled = true;
    }

    void generate() {
        System.Diagnostics.Stopwatch allStopwatch = System.Diagnostics.Stopwatch.StartNew();

        if (seed != NO_SEED) {
            UnityEngine.Random.InitState(seed);
        }

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

        var pipeTiles = createWfcTiles(pipesSample.uniqueTiles);

        List<ITileConstraint> wfcConstraints = new List<ITileConstraint>();

        Vector3Int testPos = Vector3Int.one * 3;
        int testStructure = 0;
        createStructureConstraints(testPos, testStructure, wfcConstraints, pipeTiles);
        instantiateStructure(testPos, testStructure);


        if (pathConstraint) {
            wfcConstraints.Add(new PathConstraint(tilesWithoutEmpty(pipesSample.uniqueTiles,
                                                                    tileSets[TILESET_PIPES],
                                                                    pipeTiles)));
        }

        int[,,] generatedTiles;
        bool generationSuccess = wfcRunner.runAdjacentModel(out generatedTiles,
                                                            generatedGridDimensions,
                                                            tileSets[TILESET_PIPES],
                                                            pipesSample.uniqueTiles,
                                                            pipesSample.rules,
                                                            pipeTiles,
                                                            wfcConstraints,
                                                            seed);

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

        System.Diagnostics.Stopwatch navigationStopwatch = System.Diagnostics.Stopwatch.StartNew();
        initializeNavigation(tileIndices, tileSetIndices);
        navigationStopwatch.Stop();

        List<Vector3Int> pipeTilePositions = getAllPipeTiles(tileIndices, tileSetIndices);
        EnemyManager enemyManager = EnemyManager.instance;
        enemyManager.validSpawningTiles = pipeTilePositions;

        instantiateDecorations(pipeTilePositions);

        if (scaleAfterGeneration) {
            levelGrid.transform.localScale = Vector3.one * scaleFactor;
        }

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

        navigation.tileSize = scaleFactor;
        navigation.tileGridDimensions = fullDimensions;
    }

    private List<Vector3Int> getAllPipeTiles(Grid3D<int> tileIndices, Grid3D<int> tileSetIndices) {

        List<Vector3Int> pipeTiles = new List<Vector3Int>();

        tileIndices.forEach((x, y, z, tileIndex) => {
            if (tileIndex != TileGrid.TILE_EMPTY
            && tileSetIndices.at(x, y, z) == TILESET_PIPES
            && tileIndex != tileSets[TILESET_PIPES].emptyTileIndex) {
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

    private void createStructureConstraints(Vector3Int position, int setIndex, List<ITileConstraint> wfcConstraints, Dictionary<int, DeBroglie.Tile> wfcTiles) {

        List<StructureTile> structureTiles = structureSet.frequencies[setIndex].structure.getTilesFromFile();

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
                    int emptyTileIndex = TileUtils.tileIndexToModelIndex(tileSets[TILESET_PIPES].emptyTileIndex, TileGrid.NO_ROTATION);

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
        Vector3 tileOffset = new Vector3(1, 0.5f, 1);
        GameObject prefabToSpawn = structureSet.frequencies[setIndex].structure.structurePrefab;
        GameObject structure = Instantiate(prefabToSpawn, position.toVector3() + tileOffset /* scaleFactor */, Quaternion.identity);
        structure.transform.SetParent(levelGrid.transform);
    }

    private void instantiateDecorations(List<Vector3Int> tilesToDecorate) {
        instantiateFans(tilesToDecorate);
        instantiateVines(tilesToDecorate);
    }

    private void instantiateFans(List<Vector3Int> tilesToDecorate) {
        const float chanceToSpawn = 0.1f;
        Vector3 tileOffset = new Vector3(-0.5f, 0, -0.5f);

        foreach (Vector3Int tilePosition in tilesToDecorate) {
            if (UnityEngine.Random.Range(0f, 1f) < chanceToSpawn) {
                if (levelGrid.tileIndices.at(tilePosition) == pipeHorizontalTileIndex) {
                    int rotation = levelGrid.tileRotations.at(tilePosition);
                    GameObject fanObject = Instantiate(fanPrefab,
                                                       tilePosition.toVector3() + tileOffset,
                                                       Quaternion.Euler(0, 90 * rotation, 0));
                    fanObject.transform.SetParent(levelGrid.transform);
                } else if (levelGrid.tileIndices.at(tilePosition) == pipeUpTileIndex) {
                    GameObject fanObject = Instantiate(fanPrefab,
                                                       tilePosition.toVector3() + tileOffset,
                                                       Quaternion.Euler(0, 0, 90));
                    fanObject.transform.SetParent(levelGrid.transform);
                }
            }
        }
    }

    private void instantiateVines(List<Vector3Int> tilesToDecorate) {

    }

    private Vector3Int randomVector3Int(Vector3Int min, Vector3Int max) {
        return new Vector3Int(
            UnityEngine.Random.Range(min.x, max.x),
            UnityEngine.Random.Range(min.y, max.y),
            UnityEngine.Random.Range(min.z, max.z)
        );
    }
}

// [Serializable]
// public class CuboidStructure {
//     public Vector3Int dimensions;
//     public Vector3Int position;

//     private bool inStructureBounds(Vector3Int point) {
//         if (point.x < position.x + dimensions.x
//             || point.y < position.y + dimensions.y
//             || point.z < position.z + dimensions.z
//             || point.x >= position.x
//             || point.y >= position.y
//             || point.z >= position.z) {
//             return false;
//         }

//         return true;
//     }
// }




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

