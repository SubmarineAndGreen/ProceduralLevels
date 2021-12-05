using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour {
    public static NavigationManager instance;
    public TileGrid levelGrid;
    private Vector3 gridOrigin;
    public Vector3Int tileGridDimensions;
    public float tileSize;
    [SerializeField] private Transform playerTransform;
    private void Awake() {
        instance = this;
        gridOrigin = levelGrid.transform.position;
    }

    public int[,,][,,] vectorFields;
    public int[,,][,,] distanceFields;

    public Vector3 getPathVector(Vector3Int goalTile, Vector3Int currentTile) {
        var map = vectorFields[goalTile.x, goalTile.y, goalTile.z];
        int vector = map[currentTile.x, currentTile.y, currentTile.z];
        if (vector != Navigation.NO_VECTOR) {
            return Navigation.directionVectors[vector];
        } else {
            Debug.Log("Navigation returned invalid vector");
            return Vector3.zero;
        }
    }

    public Vector3 getPathVectorToPlayer(Vector3 currentPosition) {
        return getPathVector(worldPositionToGridPosition(playerTransform.position), worldPositionToGridPosition(currentPosition));
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
                    vectorFields[x, y, z] = Navigation.calculateVectorField(distanceFields[x,y,z] , unwalkableTileValue);
                }
            }
        }
        // vectorFields = Navigation.calculateVectorFieldForEachCell(inputDistanceField, unwalkableTileValue);
    }

    public Vector3Int worldPositionToGridPosition(Vector3 worldPosition) {
        Vector3 worldOffset = gridOrigin - worldPosition;
        return Vector3Int.FloorToInt(worldOffset / tileSize);
    }
}
