using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using UnityEditor;
using System.Diagnostics;
using UnityEngine.InputSystem;

public class NavigationTest : MonoBehaviour {
    // private int[] xOffset = { 0, 0, 0, 1, 0, -1 };
    // private int[] yOffset = { 1, -1, 0, 0, 0, 0 };
    // private int[] zOffset = { 0, 0, 1, 0, -1, 0 };

    public const int BLOCKED_CELL = -1;
    private const int INIT_DISTANCE = 255;
    private const int NEIGHBOURS_COUNT = 6;

    public static Vector3Int[] directionVectors = {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.forward,
        Vector3Int.right,
        Vector3Int.back,
        Vector3Int.left
    };

    public const int NO_VECTOR = -1;

    public TileGrid tileGrid;
    public Vector3Int sourceCell;
    // public int stepCount = 20;
    const int wallTileIndex = 0;
    int[,,] inputGrid;
    Vector3Int dimensions;
    public int[,,] distanceField;
    bool[,,] visitedNodes;
    SimplePriorityQueue<Vector3Int, int> queue;
    public NavigationVisuals visuals;

    private void Update() {
        // if(Keyboard.current.spaceKey.wasPressedThisFrame) {
        //     runOneDistanceField(sourceCell);
        // }
    }

    public void runAll() {
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
        Stopwatch timer = Stopwatch.StartNew();
        Navigation.calculateVectorFieldForEachCell(distanceField, BLOCKED_CELL);
        UnityEngine.Debug.Log(timer.ElapsedMilliseconds + "ms");
    }

    public void runOneDistanceField(Vector3Int sourceCell) {
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
        Stopwatch timer = Stopwatch.StartNew();
        distanceField = Navigation.calculateDijkstraDistanceField(distanceField, BLOCKED_CELL, sourceCell);
        UnityEngine.Debug.Log(timer.ElapsedMilliseconds + "ms");
        visuals.updateDistanceFieldVisuals(distanceField);
    }

    public void runOneVectorField() {
        Stopwatch timer = Stopwatch.StartNew();
        var res = Navigation.calculateVectorField(distanceField, BLOCKED_CELL);
        UnityEngine.Debug.Log(timer.ElapsedMilliseconds + "ms");
        visuals.updateVectorFieldVisuals(res);
    }
}


[CustomEditor(typeof(NavigationTest))]
public class NavigationEditor : Editor {

    NavigationTest navigation;

    private void OnEnable() {
        navigation = target as NavigationTest;
    }
    bool toggle = false;
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        EditorUtils.guiButton("Run Distance Field", () => {
            navigation.runOneDistanceField(navigation.sourceCell);
        });
        EditorUtils.guiButton("Run Vector Field", () => {
            navigation.runOneVectorField();
        });
        EditorUtils.guiButton("Run All For Each Cell", () => {
            navigation.runAll();
        });
        EditorUtils.guiButton("Show/Hide distance field", () => {
            navigation.visuals.showDistanceField(navigation.distanceField, toggle);
            toggle = !toggle;
        });
    }
}
