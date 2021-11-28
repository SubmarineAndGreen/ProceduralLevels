using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using UnityEditor;

public class Navigation : MonoBehaviour {
    // private int[] xOffset = { 0, 0, 0, 1, 0, -1 };
    // private int[] yOffset = { 1, -1, 0, 0, 0, 0 };
    // private int[] zOffset = { 0, 0, 1, 0, -1, 0 };

    public const int BLOCKED_CELL = -1;
    private const int INIT_DISTANCE = 255;
    private const int NEIGHBOURS_COUNT = 6;

    public static Vector3Int[] neighbourOffset = {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.forward,
        Vector3Int.right,
        Vector3Int.down,
        Vector3Int.left
    };

    public TileGrid tileGrid;
    public Vector3Int sourceCell;
    // public int stepCount = 20;
    const int wallTileIndex = 0;
    int[,,] inputGrid;
    Vector3Int dimensions;
    int[,,] distanceField;
    bool[,,] visitedNodes;
    SimplePriorityQueue<Vector3Int, int> queue;
    public NavigationVisuals visuals;

    public void intializeDijkstra() {
        Grid3D<int> tiles = tileGrid.tileIndices;
        dimensions = new Vector3Int(tiles.dimensions.x, tiles.dimensions.y, tiles.dimensions.z);
        distanceField = new int[dimensions.x, dimensions.y, dimensions.z];

        tiles.forEach((x, y, z, tileIndex) => {
            if (tileIndex == wallTileIndex) {
                distanceField[x, y, z] = BLOCKED_CELL;
            } else {
                distanceField[x, y, z] = INIT_DISTANCE;
            }
        });


        visitedNodes = new bool[dimensions.x, dimensions.y, dimensions.z];
        queue = new SimplePriorityQueue<Vector3Int, int>();
        distanceField[sourceCell.x, sourceCell.y, sourceCell.z] = 0;
        queue.Enqueue(sourceCell, 0);
    }

    public bool stepDijkstra() {
        if (queue.Count == 0) {
            Debug.Log("Distance field finished");
            visuals.updateDistanceFieldVisuals(distanceField);
            return true;
        }

        Vector3Int currentCell = queue.Dequeue();
        int currentDistance = distanceField[currentCell.x, currentCell.y, currentCell.z];
        visitedNodes[currentCell.x, currentCell.y, currentCell.z] = true;

        for (int i = 0; i < NEIGHBOURS_COUNT; i++) {
            Vector3Int neighbourCell = currentCell + neighbourOffset[i];
            if (inBounds(neighbourCell.x, neighbourCell.y, neighbourCell.z, dimensions)) {
                int neighbourDistance = distanceField[neighbourCell.x, neighbourCell.y, neighbourCell.z];
                if (neighbourDistance == BLOCKED_CELL || visitedNodes[neighbourCell.x, neighbourCell.y, neighbourCell.z]) {
                    continue;
                } else {
                    if (currentDistance + 1 < neighbourDistance) {
                        distanceField[neighbourCell.x, neighbourCell.y, neighbourCell.z] = currentDistance + 1;
                    }
                    queue.Enqueue(neighbourCell, distanceField[neighbourCell.x, neighbourCell.y, neighbourCell.z]);
                }
            }
        }

        return false;
    }

    public int[,,] vectorField() {
        int[,,] vectorField = new int[dimensions.x, dimensions.y, dimensions.z];
        for (int x = 0; x < dimensions.x; x++) {
            for (int y = 0; y < dimensions.y; y++) {
                for (int z = 0; z < dimensions.z; z++) {
                    int vector = -1;
                    int minDistance = distanceField[x,y,z];
                    for (int i = 0; i < NEIGHBOURS_COUNT; i++) {
                        Vector3Int neighbourCell = new Vector3Int(x, y, z) + neighbourOffset[i];
                        if (inBounds(neighbourCell.x, neighbourCell.y, neighbourCell.z, dimensions) 
                                && distanceField[neighbourCell.x, neighbourCell.y, neighbourCell.z] != BLOCKED_CELL) {
                                    
                            if(distanceField[neighbourCell.x, neighbourCell.y, neighbourCell.z] < minDistance) {
                                minDistance = distanceField[neighbourCell.x, neighbourCell.y, neighbourCell.z];
                                vector = i;
                            }
                        }
                    }
                    vectorField[x,y,z] = vector;
                }
            }
        }

        return vectorField;
    }

    private bool inBounds(int x, int y, int z, Vector3Int bounds) {
        return x >= 0 && y >= 0 && z >= 0 && x < bounds.x && y < bounds.y && z < bounds.z;
    }
}


[CustomEditor(typeof(Navigation))]
public class NavigationEditor : Editor {

    Navigation navigation;

    private void OnEnable() {
        navigation = target as Navigation;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorUtils.guiButton("Initialize", navigation.intializeDijkstra);
        // EditorUtils.guiButton("Step", () => navigation.stepDijkstra());
        EditorUtils.guiButton("Run Distance Field", () => {
            while (!navigation.stepDijkstra()) ;
        });
        EditorUtils.guiButton("Run Vector Field", () => {
            int[,,] vectorField = navigation.vectorField();
            navigation.visuals.updateVectorFieldVisuals(vectorField);
        });
    }
}
