using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using DeBroglie;
using DeBroglie.Constraints;
using System;
using System.Threading.Tasks;

[RequireComponent(typeof(WfcRunner))]
public class LevelBuilder : MonoBehaviour {
    public int seed;
    public const int NO_SEED = -1;
    private WfcRunner wfcRunner;
    public TileGrid levelGrid;
    [SerializeField] Vector3Int levelDimensions;
    [HideInInspector] public Vector3Int fullDimensions;
    [SerializeField] bool scaleAfterGeneration;
    public float scaleFactor;
    public TileSet[] tileSets;
    [SerializeField] private bool pathConstraint = true;
    private const int TILESET_MAIN = 0;

    [Header("Structures")]
    public bool generateStructures;
    [SerializeField] Tile structurePlaceholderTile;
    [SerializeField] Tile verticalPipeTile;
    [SerializeField] Tile horizontalPipeTile;
    [SerializeField] Tile emptyTile;
    [Header("Decorations")]
    [SerializeField] GameObject stairsPrefab;
    [SerializeField] GameObject walkwayPrefab;
    [SerializeField] GameObject windowPrefab;
    [SerializeField] GameObject doorPrefab;
    [SerializeField] Structure patioBase, patioOpen, patioClosed, patioTop;


    [HideInInspector] public string pipesSampleFileName;
    [HideInInspector] public Vector3Int playerSpawn;

    public IEnumerator generateLevel() {
        #region MAIN_STRUCTURE
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

        if (generateStructures) {
            playerSpawn = placePatioStructure(wfcConstraints, wfcTiles);
        } else {
            int index = tileSets[TILESET_MAIN].getTileIndexFromTileObject(structurePlaceholderTile);

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
        // createStructureConstraints(new Vector3Int(6, 6, 6), patioBase, wfcConstraints, wfcTiles, new List<Vector3Int>());

        var wfcTask = Task<int[,,]>.Run(() => wfcRunner.runAdjacentModel(fullDimensions,
                                                            tileSets[TILESET_MAIN],
                                                            samplerResult.uniqueTiles,
                                                            samplerResult.rules,
                                                            wfcTiles,
                                                            wfcConstraints,
                                                            seed)); 

        yield return new WaitUntil(() => wfcTask.IsCompleted);
        
        int[,,] generatedTiles = wfcTask.Result;

        wfcStopwatch.Stop();



        Dictionary<int, List<Vector3Int>> generatedTilePositionsByIndex = new Dictionary<int, List<Vector3Int>>();

        setGeneratedTiles(generatedTiles, generatedTilePositionsByIndex, tileIndices, tileRotations, tileSetIndices);

        levelGrid.tileIndices = tileIndices;
        levelGrid.tileRotations = tileRotations;
        levelGrid.tileSetIndices = tileSetIndices;
        levelGrid.tileObjects = tileObjects;
        levelGrid.rebuildGrid();
        #endregion MAIN_STRUCTURE
        placeStairs(levelGrid, samplerResult);
        placeWindows(levelGrid, samplerResult);
        List<Vector3Int> doorTiles = placeDoors(levelGrid, samplerResult);

        System.Diagnostics.Stopwatch navigationStopwatch = System.Diagnostics.Stopwatch.StartNew();
        yield return StartCoroutine(initializeNavigation(tileIndices, tileSetIndices));
        navigationStopwatch.Stop();

        List<Vector3Int> walkableTilePositions = getWalkableTiles(tileIndices, tileSetIndices);
        NavigationManager navigationManager = NavigationManager.instance;
        navigationManager.walkableTiles = walkableTilePositions;
        navigationManager.doorTiles = doorTiles;

        if (scaleAfterGeneration) {
            levelGrid.transform.localScale = Vector3.one * scaleFactor;
        }

        allStopwatch.Stop();
        Debug.Log($"Level created in: {allStopwatch.ElapsedMilliseconds}ms, wfc/constraints:{wfcStopwatch.ElapsedMilliseconds}ms, navigation:{navigationStopwatch.ElapsedMilliseconds}ms");
    }



    private void createEmptyBorderConstraint(List<ITileConstraint> wfcConstraints, Dictionary<int, DeBroglie.Tile> wfcTiles) {
        int emptyTileIndex = tileSets[TILESET_MAIN].getTileIndexFromTileObject(emptyTile);
        DeBroglie.Tile emptyWfcTile = wfcTiles[TileUtils.tileIndexToModelIndex(emptyTileIndex, TileGrid.NO_ROTATION)];
        BorderConstraint emptyBorderConstraint = new BorderConstraint();
        emptyBorderConstraint.Sides = BorderSides.All;
        emptyBorderConstraint.Tiles = new DeBroglie.Tile[] { emptyWfcTile };
        wfcConstraints.Add(emptyBorderConstraint);
    }

    private IEnumerator initializeNavigation(Grid3D<int> tileIndices, Grid3D<int> tileSetIndices) {
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
        var fieldsTask = Task.Run(() => navigation.calculateVectorFields(inputDistanceField, blockedTile));

        yield return new WaitUntil(() => fieldsTask.IsCompleted);

        navigation.tileSize = scaleFactor;
        navigation.tileGridDimensions = fullDimensions;

        yield return null;
    }

    private List<Vector3Int> getWalkableTiles(Grid3D<int> tileIndices, Grid3D<int> tileSetIndices) {

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

    private int createStructureConstraints(Vector3Int position,
                                            Structure structure,
                                            List<ITileConstraint> wfcConstraints,
                                            Dictionary<int, DeBroglie.Tile> wfcTiles,
                                            List<Vector3Int> nonSpawnableTiles) {

        // Structure structure = structure;
        List<StructureTile> structureTiles = structure.getTilesCollection().tiles;
        int structurePlaceholderIndex = tileSets[TILESET_MAIN].getTileIndexFromTileObject(structurePlaceholderTile);
        int pipeUpTileIndex = tileSets[TILESET_MAIN].getTileIndexFromTileObject(verticalPipeTile);
        int pipeHorizontalTileIndex = tileSets[TILESET_MAIN].getTileIndexFromTileObject(horizontalPipeTile);

        int structureTileCount = 0;
        HashSet<Vector3Int> fixedTiles = new HashSet<Vector3Int>();

        foreach (StructureTile tile in structureTiles) {
            Vector3Int structureTilePosition = position + tile.position;
            // Debug.Log(structureTilePosition);
            fixedTiles.Add(structureTilePosition);

            if (tile.noConstraints) {
                continue;
            } else {
                structureTileCount++;
            }

            if (tile.excludeFromSpawning) {
                nonSpawnableTiles.Add(structureTilePosition);
            }

            wfcConstraints.Add(new FixedTileConstraint() {
                Point = new Point(structureTilePosition.x, structureTilePosition.y, structureTilePosition.z),
                Tiles = new DeBroglie.Tile[] {
                    wfcTiles[TileUtils.tileIndexToModelIndex(structurePlaceholderIndex, TileGrid.NO_ROTATION)]
                }
            });
        }

        foreach (StructureTile tile in structureTiles) {
            if (tile.noConstraints) {
                continue;
            }
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

        return structureTileCount;
    }

    private void constrainStructureTileCount(int count, List<ITileConstraint> wfcConstraints, Dictionary<int, DeBroglie.Tile> wfcTiles) {

        int structurePlaceholderIndex = tileSets[TILESET_MAIN].getTileIndexFromTileObject(structurePlaceholderTile);

        wfcConstraints.Add(new CountConstraint() {
            Comparison = CountComparison.Exactly,
            Count = count,
            Tiles = new HashSet<DeBroglie.Tile>() {
                    wfcTiles[TileUtils.tileIndexToModelIndex(structurePlaceholderIndex, TileGrid.NO_ROTATION)]
            }
        });
    }

    private void instantiateStructure(Vector3Int position, Structure structure) {
        Vector3 tileOffset = new Vector3(0, -0.5f, 0);
        GameObject prefabToSpawn = structure.structurePrefab;
        GameObject structureObject = Instantiate(prefabToSpawn, position.toVector3() + tileOffset /* scaleFactor */, Quaternion.identity);
        structureObject.transform.SetParent(levelGrid.transform);
    }

    private Vector3Int placePatioStructure(List<ITileConstraint> wfcConstraints, Dictionary<int, DeBroglie.Tile> wfcTiles) {
        List<Vector3Int> nonSpawnableTiles = new List<Vector3Int>();
        int structureTileCount = 0;

        const int structureWidth = 7;
        const int baseY = 1;
        Vector3Int basePositon = new Vector3Int(UnityEngine.Random.Range(structureWidth + 1, fullDimensions.x - 1 - structureWidth),
                                                baseY,
                                                UnityEngine.Random.Range(structureWidth + 1, fullDimensions.x - 1 - structureWidth));
        int maxY = fullDimensions.y - 3;
        int yOffset = 0;

        structureTileCount += createStructureConstraints(basePositon, patioBase, wfcConstraints, wfcTiles, nonSpawnableTiles);
        instantiateStructure(basePositon, patioBase);

        bool placeOpenPart = false;
        for (int i = yOffset; i <= maxY - baseY - 1; i++) {
            Structure structureToPlace = placeOpenPart ? patioOpen : patioClosed;
            placeOpenPart = !placeOpenPart;

            Vector3Int newPosition = basePositon + Vector3Int.up * i;
            structureTileCount += createStructureConstraints(newPosition, structureToPlace, wfcConstraints, wfcTiles, nonSpawnableTiles);
            instantiateStructure(newPosition, structureToPlace);
        }

        structureTileCount += createStructureConstraints(basePositon + Vector3Int.up * (maxY - baseY), patioTop, wfcConstraints, wfcTiles, nonSpawnableTiles);
        instantiateStructure(basePositon + Vector3Int.up * (maxY - baseY), patioTop);

        // structureTileCount -= 9;

        constrainStructureTileCount(structureTileCount, wfcConstraints, wfcTiles);

        Vector3Int playerSpawn = new Vector3Int(basePositon.x + 1, UnityEngine.Random.Range(baseY, maxY - 1), basePositon.z + 1);
        return playerSpawn;
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

    private void placeWindows(TileGrid levelGrid, SamplerResult samplerResult) {
        // Directions3D[,,] windowPlacement = new Directions3D[fullDimensions.x, fullDimensions.y, fullDimensions.z];
        int emptyTileIndex = tileSets[TILESET_MAIN].getTileIndexFromTileObject(emptyTile);
        int structureTileIndex = tileSets[TILESET_MAIN].getTileIndexFromTileObject(structurePlaceholderTile);
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

                    if (tileIndex == emptyTileIndex || tileIndex == structureTileIndex) {
                        placedWindowLastTile = false;
                    } else {
                        isWallOnRight = !connectionData[tileIndex].canConnectFromDirection(Directions3D.RIGHT, tileRotation);
                        isWallOnLeft = !connectionData[tileIndex].canConnectFromDirection(Directions3D.LEFT, tileRotation);
                        isStairsTile = connectionData[tileIndex].canConnectFromDirection(Directions3D.UP, tileRotation);

                        if (isStairsTile) {
                            if (isWallOnRight) {
                                if (windowDirection == Directions3D.LEFT) {
                                    changedDirection = true;
                                }
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
                            Quaternion windowRotation = Quaternion.Euler(0, ((int)windowDirection - 2) * 90, 0);
                            GameObject windowObject = Instantiate(windowPrefab, currentPosition + offset, windowRotation);
                            windowObject.transform.SetParent(levelGrid.transform);

                            if (placedWindowLastTile && !changedDirection) {
                                Vector3 previousOffest = new Vector3(0, 0, -0.5f);
                                GameObject previousWindowObject = Instantiate(windowPrefab, currentPosition + offset + previousOffest, windowRotation);
                                previousWindowObject.transform.SetParent(levelGrid.transform);
                            }

                            placedWindowLastTile = true;
                            // windowPlacement[x, y, z] = windowDirection;
                        } else {
                            placedWindowLastTile = false;
                            // windowPlacement[x, y, z] = Directions3D.UP;
                        }
                    }
                }
            }
        }

        // return windowPlacement;
    }

    private List<Vector3Int> placeDoors(TileGrid levelGrid, SamplerResult samplerResult) {
        Vector3 offset = new Vector3(-0.5f, 0, -0.5f);
        int emptyTileIndex = tileSets[TILESET_MAIN].getTileIndexFromTileObject(emptyTile);
        int structureTileIndex = tileSets[TILESET_MAIN].getTileIndexFromTileObject(structurePlaceholderTile);

        const float doorProbability = 0.8f;
        ConnectionData[] connectionData = samplerResult.connections;
        int[,,] tileIndices = levelGrid.tileIndices.toArray();

        int doorNumber = 0;
        HashSet<Vector3Int> doorPlacements = new HashSet<Vector3Int>();

        for (int y = 0; y < fullDimensions.y; y++) {
            for (int z = 0; z < fullDimensions.z; z++) {
                for (int x = 0; x < fullDimensions.x; x++) {
                    int index = tileIndices[x, y, z];
                    Vector3Int position = new Vector3Int(x, y, z);
                    int rotation = levelGrid.tileRotations.at(position);
                    if (index != emptyTileIndex && index != structureTileIndex
                        && !connectionData[index].canConnectFromDirection(Directions3D.DOWN, TileGrid.NO_ROTATION)) {

                        if (!connectionData[index].canConnectFromDirection(Directions3D.FORWARD, rotation)) {
                            if (UnityEngine.Random.Range(0f, 1f) <= doorProbability) {
                                doorPlacements.Add(position);
                                GameObject doorObject = Instantiate(doorPrefab, position.toVector3() + offset, Quaternion.Euler(0f, 90f, 0f), levelGrid.transform);
                                doorNumber++;
                                Door door = doorObject.GetComponentInChildren<Door>();
                                door.setDoorNumber(doorNumber);
                            }
                        }
                        if (!connectionData[index].canConnectFromDirection(Directions3D.BACK, rotation)
                            && !connectionData[index].canConnectFromDirection(Directions3D.UP, TileGrid.NO_ROTATION)) {
                            if (UnityEngine.Random.Range(0f, 1f) <= doorProbability) {
                                doorPlacements.Add(position);
                                GameObject doorObject = Instantiate(doorPrefab, position.toVector3() + offset, Quaternion.Euler(0f, -90f, 0f), levelGrid.transform);
                                doorNumber++;
                                Door door = doorObject.GetComponentInChildren<Door>();
                                door.setDoorNumber(doorNumber);
                            }
                        }
                    }
                }
            }
        }

        return doorPlacements.ToList();
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

