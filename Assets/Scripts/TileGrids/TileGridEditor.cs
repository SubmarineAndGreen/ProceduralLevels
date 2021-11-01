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

        saveDir = $"{Application.dataPath}/{TileGrid.saveFolder}";
        //tworzy folder tylko gdy go nie ma
        Directory.CreateDirectory(saveDir);

        if (!String.IsNullOrEmpty(editedGrid.currentSaveFile))
        {
            gridPickerTooltip = new GUIContent(editedGrid.currentSaveFile);
        }
        else
        {
            gridPickerTooltip = new GUIContent("File Name");
        }


        if (EditorGUILayout.DropdownButton(gridPickerTooltip, FocusType.Passive))
        {
            GenericMenu gridPickerMenu = new GenericMenu();
            var saveFiles = getGridSaveFiles();
            if (saveFiles != null)
            {
                foreach (string item in saveFiles)
                {
                    gridPickerMenu.AddItem(new GUIContent(item), false, HandleItemClick, item);
                }
            }

            gridPickerMenu.ShowAsContext();
        }

        GUILayout.BeginHorizontal();

        EditorUtils.guiButton("Save", editedGrid.saveToFile);
        EditorUtils.guiButton("Load", () => {
            editedGrid.loadFromFile();
            editedGrid.rebuildGrid();
        });

        GUILayout.EndHorizontal();

        newGridName = EditorGUILayout.TextField(new GUIContent("New File Name"), newGridName);
        EditorUtils.guiButton("Create new file", newGrid);
        // EditorUtils.guiButton("Rebuild", editedGrid.rebuild);
        EditorUtils.guiButton("Clear Grid", editedGrid.clearGrid);
        
        resizeDimensions = EditorGUILayout.Vector3IntField("New Dimensions", resizeDimensions);
        EditorUtils.guiButton("Resize", () => editedGrid.resize(resizeDimensions));
    }

    void HandleItemClick(object parameter)
    {
        if (editedGrid)
        {
            editedGrid.currentSaveFile = parameter as string;
        }
    }

    List<string> getGridSaveFiles()
    {
        try
        {
            List<string> filePaths = Directory.GetFiles(saveDir)
            .Where(
                fileName => !fileName.EndsWith(".meta")
            ).ToList<string>();

            List<string> fileNames = new List<string>();
            filePaths.ForEach(path => fileNames.Add(Path.GetFileName(path)));
            return fileNames;
        }
        catch
        {
            Debug.LogError("Trouble getting grid save files: invalid directory");
            return null;
        }

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
}
