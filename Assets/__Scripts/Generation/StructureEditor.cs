using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[RequireComponent(typeof(TileGrid))]
public class StructureEditor : MonoBehaviour {
    [SerializeField] Structure structureScriptableObject;
    TileGrid grid;
    private void Awake() {
        grid = GetComponent<TileGrid>();
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
}



[CustomEditor(typeof(StructureEditor))]
public class StructureEditorUI : Editor {

    StructureEditor structureEditor;

    private void OnEnable() {
        structureEditor = target as StructureEditor;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField("Toggle open side");
        EditorGUILayout.BeginHorizontal();
        EditorUtils.guiButton("Y+", () => structureEditor.toggleOpenSideAtCursor(Directions3D.UP));
        EditorUtils.guiButton("Y-", () => structureEditor.toggleOpenSideAtCursor(Directions3D.DOWN));
        EditorUtils.guiButton("X+", () => structureEditor.toggleOpenSideAtCursor(Directions3D.RIGHT));
        EditorUtils.guiButton("X-", () => structureEditor.toggleOpenSideAtCursor(Directions3D.LEFT));
        EditorUtils.guiButton("Z+", () => structureEditor.toggleOpenSideAtCursor(Directions3D.BACK));
        EditorUtils.guiButton("Z-", () => structureEditor.toggleOpenSideAtCursor(Directions3D.FORWARD));
        EditorGUILayout.EndHorizontal();
    }
}
