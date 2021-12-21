using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[RequireComponent(typeof(TileGrid))]
public class StructureEdit : MonoBehaviour {
    [SerializeField] Structure structureScriptableObject;
    GameObject structureInstance;
    Structure structure;
    TileGrid grid;
    const int STRUCTURE_TILE_INDEX = 0;
    const int STRUCTURE_TILE_SET = 0;
    private void Awake() {
        grid = GetComponent<TileGrid>();
        if (structureScriptableObject != null && structureScriptableObject.structurePrefab != null) {
            instantiateStructure();
        }
    }

    public void toggleOpenSideAtCursor(Directions3D side) {
        GameObject tileObject = grid.tileObjects.at(grid.cursorPosition);
        StructureTileObject structureTileObject = tileObject.GetComponent<StructureTileObject>();
        StructureTile structureTile = structureTileObject.structureTile;
        if (structureTile != null) {
            structureTile.setSideOpen(side, !structureTile.isSideOpen(side));
        } else {
            Debug.LogWarning("Tried to set open side on non structure tile");
        }
        structureTileObject.instantiateMarkers();
    }

    public void toggleUnwalkableAtCursor() {
        GameObject tileObject = grid.tileObjects.at(grid.cursorPosition);
        StructureTileObject structureTileObject = tileObject.GetComponent<StructureTileObject>();
        StructureTile structureTile = structureTileObject.structureTile;
        if (structureTile != null) {
            structureTile.walkable = !structureTile.walkable;
        } else {
            Debug.LogWarning("Tried to set walkable on non structure tile");
        }
        structureTileObject.instantiateMarkers();
    }

    public void saveStructure() {
        structureScriptableObject.tiles = new List<StructureTile>();
        structureScriptableObject.dimensions = grid.dimensions;

        grid.tileIndices.forEach((Vector3Int position, int index) => {
            if (index != TileGrid.TILE_EMPTY) {
                StructureTile structureTile = grid.tileObjects.at(position)
                                                              .GetComponent<StructureTileObject>().structureTile;
                structureTile.position = position;
                structureScriptableObject.tiles.Add(structureTile);
            }
        });

        structureScriptableObject.saveTilesToFile();
    }

    public void loadStructure() {
        structureScriptableObject.tiles = structureScriptableObject.getTilesFromFile();
        grid.clear();
        grid.resizePerserveTiles(structureScriptableObject.dimensions);

        foreach (StructureTile tile in structureScriptableObject.tiles) {
            grid.placeTile(STRUCTURE_TILE_INDEX, STRUCTURE_TILE_SET, tile.position, TileGrid.NO_ROTATION);
            GameObject newTileObject = grid.tileObjects.at(tile.position);
            StructureTileObject structureTileObject = newTileObject.GetComponent<StructureTileObject>();
            structureTileObject.structureTile = tile;
            structureTileObject.instantiateMarkers();
        }
    }

    public void instantiateStructure() {
        Destroy(structureInstance);
        structureInstance = Instantiate(structureScriptableObject.structurePrefab);
    }
}



[CustomEditor(typeof(StructureEdit))]
public class StructureEditorUI : Editor {

    StructureEdit structureEditor;

    private void OnEnable() {
        structureEditor = target as StructureEdit;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField("Toggle open side:");
        EditorGUILayout.BeginHorizontal();
        EditorUtils.guiButton("Y+", () => structureEditor.toggleOpenSideAtCursor(Directions3D.UP));
        EditorUtils.guiButton("Y-", () => structureEditor.toggleOpenSideAtCursor(Directions3D.DOWN));
        EditorUtils.guiButton("X+", () => structureEditor.toggleOpenSideAtCursor(Directions3D.RIGHT));
        EditorUtils.guiButton("X-", () => structureEditor.toggleOpenSideAtCursor(Directions3D.LEFT));
        EditorUtils.guiButton("Z+", () => structureEditor.toggleOpenSideAtCursor(Directions3D.BACK));
        EditorUtils.guiButton("Z-", () => structureEditor.toggleOpenSideAtCursor(Directions3D.FORWARD));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10);
        EditorUtils.guiButton("Toggle walkable", structureEditor.toggleUnwalkableAtCursor);
        EditorUtils.guiButton("Instantiate structure", structureEditor.instantiateStructure);
        EditorUtils.guiButton("Load structure tiles", structureEditor.loadStructure);
        EditorUtils.guiButton("Saves structure tiles", structureEditor.saveStructure);
    }
}
