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
    [SerializeField] private GameObject cursorPrefab;
    private GameObject cursor;
    private Vector3Int cursorPosition;


    private void Start()
    {
        loadIndicesArray();
        cursor = Instantiate(cursorPrefab, Vector3.zero, Quaternion.Euler(0, 90, 0));
    }

    private void Update() {
        gridCursorMovement();
    }

    void gridCursorMovement() {
        const float cursorStep = 1f;

        if(MyInput.gridControls.disabled) {
            return;
        }

        bool keyW = MyInput.gridControls.keyW;
        bool keyA = MyInput.gridControls.keyA;
        bool keyS = MyInput.gridControls.keyS;
        bool keyD = MyInput.gridControls.keyD;

        bool heightToggle = MyInput.gridControls.heightToggle;

        Vector3Int cursorMovement;

        if(heightToggle) {
            cursorMovement = new Vector3Int(
                0,
                (keyW ? 1 : 0) + (keyS ? -1 : 0),
                0
            );
        } else {
            cursorMovement = new Vector3Int(
                (keyA ? -1 : 0) + (keyD ? 1 : 0),
                0,
                (keyW ? 1 : 0) + (keyS ? -1 : 0)
            );
        }

        Debug.Log(cursorMovement);

        cursorPosition += cursorMovement;
        cursorPosition.x = Mathf.Clamp(cursorPosition.x, 0, dimensions.x - 1);
        cursorPosition.y = Mathf.Clamp(cursorPosition.y, 0, dimensions.y - 1);
        cursorPosition.z = Mathf.Clamp(cursorPosition.z, 0, dimensions.z - 1);

        cursor.transform.localPosition = new Vector3(
            cursorPosition.x * cursorStep,
            cursorPosition.y * cursorStep,
            cursorPosition.z * cursorStep
        );
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



