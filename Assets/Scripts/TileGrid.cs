using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;
using System;

public class TileGrid : MonoBehaviour
{
    [HideInInspector] public string currentSaveFile;
    [SerializeField] private Vector3 dimensions;
}



[CustomEditor(typeof(TileGrid))]
public class TileGridEditor : Editor
{

    TileGrid editedGrid = null;
    string saveDir;
    const string saveFolder = "InputGrids";
    GUIContent gridPickerTooltip;
    string newGridName = "";

    private void OnEnable()
    {
        editedGrid = (TileGrid)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        saveDir = Application.dataPath + "/" + saveFolder;
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

        guiButton("Save", loadGrid);
        guiButton("Load", saveGrid);

        GUILayout.EndHorizontal();

        newGridName = EditorGUILayout.TextField(new GUIContent("New File Name"), newGridName);
        guiButton("Create new grid file", newGrid);
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

    void guiButton(string label, Action callback)
    {
        if (GUILayout.Button(label))
        {
            callback();
        }
    }

    void loadGrid()
    {

    }

    void saveGrid()
    {

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
