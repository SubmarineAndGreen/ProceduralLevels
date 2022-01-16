using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour {
    public static NavigationManager instance;
    public TileGrid levelGrid;
    private Vector3 gridOrigin;
    public Vector3Int tileGridDimensions;
    public float tileSize;
    [HideInInspector] public List<Vector3Int> walkableTiles;
    [HideInInspector] public List<Vector3Int> doorTiles;
    public int playerLayer;
    public LayerMask levelMask;
    public LayerMask ignoreDecorationsMask;
    public Vector3Int getRandomWalkableTile() {
        return walkableTiles[Random.Range(0, walkableTiles.Count - 1)];
    }

    public Vector3Int getRandomDoorTile() {
        return doorTiles[Random.Range(0, doorTiles.Count - 1)];
    }

    [SerializeField] public Transform playerTransform;
    private void Awake() {
        instance = this;
        gridOrigin = levelGrid.transform.position;
        playerLayer = LayerMask.NameToLayer("Player");
        levelMask = ~((1 << LayerMask.NameToLayer("Player"))
        | (1 << LayerMask.NameToLayer("Enemy"))
        | (1 << LayerMask.NameToLayer("Decorations"))
        | (1 << LayerMask.NameToLayer("EnemyIgnoreDecorations"))
        | (1 << LayerMask.NameToLayer("Bullets"))
        | (1 << LayerMask.NameToLayer("MineDetection"))
        );
        ignoreDecorationsMask = ~(1 << LayerMask.NameToLayer("Decorations"));
    }

    public int[,,][,,] vectorFields;
    public int[,,][,,] distanceFields;

    public Vector3 getPathVector(Vector3Int goalTile, Vector3Int currentTile) {
        if (!Navigation.inBounds(goalTile, tileGridDimensions) || !Navigation.inBounds(currentTile, tileGridDimensions)) {
            return Vector3.zero;
        }
        var vectorField = vectorFields[goalTile.x, goalTile.y, goalTile.z];

        if (vectorField == null) {
            return Vector3.zero;
        }

        int vector = vectorField[currentTile.x, currentTile.y, currentTile.z];
        if (vector != Navigation.NO_VECTOR) {
            return Navigation.directionVectors[vector];
        } else {
            // Debug.Log("Navigation returned Vector3.zero");
            return Vector3.zero;
        }
    }

    public int getGridDistance(Vector3Int goalTile, Vector3Int sourceTile) {
        if (!Navigation.inBounds(goalTile, tileGridDimensions) || !Navigation.inBounds(sourceTile, tileGridDimensions)) {
            return int.MaxValue;
        }

        var distanceField = distanceFields[goalTile.x, goalTile.y, goalTile.z];

        if (distanceField == null) {
            return int.MaxValue;
        }

        int distance = distanceField[sourceTile.x, sourceTile.y, sourceTile.z];

        return distance;
    }

    public Vector3 getPathVectorToPlayer(Vector3 currentPosition) {
        return getPathVector(worldPositionToGridPosition(playerTransform.position), worldPositionToGridPosition(currentPosition));
    }

    public int getGridDistanceToPlayerWorld(Vector3 currentPosition) {
        return getGridDistance(worldPositionToGridPosition(playerTransform.position), worldPositionToGridPosition(currentPosition));
    }

    public int getGridDistanceToPlayer(Vector3Int currentPosition) {
        return getGridDistance(worldPositionToGridPosition(playerTransform.position), currentPosition);
    }

    public void calculateVectorFields(int[,,] inputDistanceField, int unwalkableTileValue) {
        int xMax = inputDistanceField.GetLength(0);
        int yMax = inputDistanceField.GetLength(1);
        int zMax = inputDistanceField.GetLength(2);
        distanceFields = new int[xMax, yMax, zMax][,,];
        vectorFields = new int[xMax, yMax, zMax][,,];
        for (int x = 0; x < xMax; x++) {
            for (int y = 0; y < yMax; y++) {
                for (int z = 0; z < zMax; z++) {
                    if (inputDistanceField[x, y, z] == unwalkableTileValue) {
                        continue;
                    }
                    int[,,] input = new int[xMax, yMax, zMax];

                    for (int x1 = 0; x1 < xMax; x1++) {
                        for (int y1 = 0; y1 < yMax; y1++) {
                            for (int z1 = 0; z1 < zMax; z1++) {
                                input[x1, y1, z1] = inputDistanceField[x1, y1, z1];
                            }
                        }
                    }

                    distanceFields[x, y, z] = Navigation.calculateDijkstraDistanceField(input, unwalkableTileValue, new Vector3Int(x, y, z));
                    vectorFields[x, y, z] = Navigation.calculateVectorField(distanceFields[x, y, z], unwalkableTileValue);
                }
            }
        }
        // vectorFields = Navigation.calculateVectorFieldForEachCell(inputDistanceField, unwalkableTileValue);
    }

    public Vector3Int worldPositionToGridPosition(Vector3 worldPosition) {
        Vector3 tileCenterOffset = new Vector3(0, tileSize / 2f, 0);
        Vector3 worldOffset = worldPosition - gridOrigin;
        Vector3Int result = Vector3Int.CeilToInt((worldOffset - tileCenterOffset) / tileSize);
        // Debug.Log(result);
        return result;
    }

    public Vector3 gridPositionToWorldPosition(Vector3Int gridPosition) {
        Vector3 tileCenterOffset = new Vector3(tileSize / 2, 0, tileSize / 2);
        return gridOrigin + gridPosition.toVector3() * tileSize - tileCenterOffset;
    }
}
