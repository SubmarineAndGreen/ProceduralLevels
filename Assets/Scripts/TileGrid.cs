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
    public const string saveFolder = "InputGrids";
    private Vector3Int dimensions = new Vector3Int(3, 3, 3);
    int[,,] tileIndices;

    private void Start()
    {
        loadIndicesArray();
        // saveToFile();
    }

    void loadIndicesArray()
    {
        if(!loadFromFile()) {
            tileIndices = new int[dimensions.x, dimensions.y, dimensions.z];
        }
    }

    public void saveToFile()
    {
        GridSave save = new GridSave(dimensions, tileIndices);
        string jsonString = JsonUtility.ToJson(save);
        Debug.Log(jsonString);
        File.WriteAllText($"{Application.dataPath}/{saveFolder}/{currentSaveFile}", jsonString);
    }

    public bool loadFromFile()
    {
        try
        {
            string jsonString = File.ReadAllText($"{Application.dataPath}/{saveFolder}/{currentSaveFile}");
            GridSave loadedSave = JsonUtility.FromJson<GridSave>(jsonString);
            dimensions = loadedSave.dimensions;
            tileIndices = loadedSave.TilesAsIntArray();
            return true;
        }
        catch (FileNotFoundException e)
        {
            Debug.Log("Loading grid file failed: file not found " + e.FileName);
        }
        catch (Exception e)
        {
            Debug.Log("Loading grid file failed: " + e.Message);
        }

        return false;
    }
}



