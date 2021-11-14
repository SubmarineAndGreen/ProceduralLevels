using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileGrid))]
public class TileGridEditor : Editor
{

    TileGrid editedGrid = null;
    string saveDir;
    GUIContent gridPickerTooltip;
    string newGridName = "";
    Vector3Int resizeDimensions = new Vector3Int();

    private void OnEnable()
    {
        editedGrid = (TileGrid)target;
    }

    public override void OnInspectorGUI()
    {
        
        base.OnInspectorGUI();

        saveDir = $"{Application.dataPath}/{TileGrid.SAVE_FOLDER}";
        //tworzy folder tylko gdy go nie ma
        Directory.CreateDirectory(saveDir);

        EditorGUILayout.Space(10);
        EditorUtils.guiButton("Edit this grid", toggleEditingGrids);
        EditorGUILayout.Space(10);

        EditorUtils.filePicker("Grid Save File", editedGrid.currentSaveFile, saveDir, (object parameter) => {
            editedGrid.currentSaveFile = parameter as string;
        });

        GUILayout.BeginHorizontal();

        EditorUtils.guiButton("Save", editedGrid.saveToFile);
        EditorUtils.guiButton("Load", () => {
            editedGrid.loadFromFile();
            editedGrid.rebuildGrid();
        });

        GUILayout.EndHorizontal();

        newGridName = EditorGUILayout.TextField(new GUIContent("New file name"), newGridName);
        EditorUtils.guiButton("Create new file", newGrid);
        // EditorUtils.guiButton("Rebuild", editedGrid.rebuild);
        EditorUtils.guiButton("Clear Grid", editedGrid.clear);
        
        resizeDimensions = EditorGUILayout.Vector3IntField("New Dimensions", resizeDimensions);
        EditorUtils.guiButton("Resize", () => editedGrid.resize(resizeDimensions));
    }

    void newGrid()
    {
        FileStream newFile;

        if(String.IsNullOrWhiteSpace(newGridName)) {
            Debug.LogError("File not created: invalid filename");
            return;
        }

        try
        {
            newFile = File.Create($"{saveDir}/{newGridName}.json");
            Debug.Log("Created new file: " + newFile.Name);
        }
        catch
        {
            Debug.LogError("Something went wrong creating new file!");
        }
    }

    void toggleEditingGrids() {
        TileGrid[] allGrids = FindObjectsOfType<TileGrid>();
        foreach(TileGrid grid in allGrids) {
            if(grid != editedGrid) {
                grid.areEditingControlsOn = false;
            }
        }
        editedGrid.areEditingControlsOn = true;
    }
}
