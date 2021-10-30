using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class TileGrid : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI previewRotationText;
    [SerializeField] private TextMeshProUGUI selectedTileRotationText;
    [SerializeField] private GameObject cursorPrefab;
    [Space(50)]
    [SerializeField] private TileSet tileSet;
    [HideInInspector] public string currentSaveFile;
    public const string saveFolder = "InputGrids";
    
    private Vector3Int dimensions = new Vector3Int(3, 3, 3);
    int[,,] tileIndices;
    int[,,] tileRotations;
    GameObject[,,] tileObjects;
    
    private GameObject cursor;
    private Vector3Int cursorPosition;
    private List<GameObject> tilePreviews;
    private int activeTileIndex;
    private int activeTileRotation;
    private GameObject activePreview;
    private int rotation;



    private void Start()
    {

        loadIndicesArray();
        cursor = Instantiate(cursorPrefab, Vector3.zero, Quaternion.Euler(0, 90, 0));

        tilePreviews = new List<GameObject>();

        foreach (GameObject tile in tileSet.tiles)
        {
            GameObject preview = Instantiate(tile);
            preview.SetActive(false);
            tilePreviews.Add(preview);
        }

        changeTilePreview();
    }

    private void Update()
    {
        gridCursorMovement();
        tileSelectionControls();
        rotationControls();
        placeAndRemoveControls();
        updateUI();
    }

    void loadIndicesArray()
    {
        if (!loadFromFile())
        {
            tileIndices = new int[dimensions.x, dimensions.y, dimensions.z];
        }
        tileObjects = new GameObject[dimensions.x, dimensions.y, dimensions.z];
    }

    public void saveToFile()
    {
        GridSave save = new GridSave(dimensions, tileIndices, tileRotations);
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
            tileIndices = loadedSave.getTileIndices();
            tileRotations = loadedSave.getTileRotations();
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

    void gridCursorMovement()
    {
        const float cursorStep = 1f;

        if (MyInput.gridControls.disabled)
        {
            return;
        }

        bool keyW = MyInput.gridControls.keyW;
        bool keyA = MyInput.gridControls.keyA;
        bool keyS = MyInput.gridControls.keyS;
        bool keyD = MyInput.gridControls.keyD;

        bool heightToggle = MyInput.gridControls.heightToggle;

        Vector3Int cursorMovement;

        if (heightToggle)
        {
            cursorMovement = new Vector3Int(
                0,
                (keyW ? 1 : 0) + (keyS ? -1 : 0),
                0
            );
        }
        else
        {
            cursorMovement = new Vector3Int(
                (keyA ? -1 : 0) + (keyD ? 1 : 0),
                0,
                (keyW ? 1 : 0) + (keyS ? -1 : 0)
            );
        }

        // Debug.Log(cursorMovement);

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

    void changeTilePreview()
    {
        Vector3 halfTileOffset = new Vector3(0.5f, 0, -0.5f);

        if (activePreview != null)
        {
            activePreview.transform.SetParent(null);
            activePreview.SetActive(false);
        }
        activePreview = tilePreviews[activeTileIndex];
        activePreview.SetActive(true);
        activePreview.transform.SetParent(cursor.transform);
        activePreview.transform.localPosition = Vector3.zero + halfTileOffset;
    }

    void tileSelectionControls()
    {
        bool indexChanged = false;
        if (MyInput.gridControls.nextTile)
        {
            activeTileIndex++;
            if (activeTileIndex > tileSet.tiles.Count - 1)
            {
                activeTileIndex = 0;
            }
            indexChanged = true;
        }
        if (MyInput.gridControls.previousTile)
        {
            activeTileIndex--;
            if (activeTileIndex < 0)
            {
                activeTileIndex = tileSet.tiles.Count - 1;
            }
            indexChanged = true;
        }
        if (indexChanged)
        {
            changeTilePreview();
        }
    }

    void rotationControls()
    {
        if (MyInput.gridControls.rotate)
        {
            activeTileRotation += 1;
            if (activeTileRotation > 3)
            {
                activeTileRotation = 0;
            }

            activePreview.transform.Rotate(Vector3.up, 90, Space.Self);
        }
    }

    void placeAndRemoveControls()
    {
        Vector3 halfTileOffset = new Vector3(0.5f, 0, -0.5f);
        if (MyInput.gridControls.place)
        {
            removeTile();
            placeTile();
        }
        if(MyInput.gridControls.remove)
        {
            removeTile();
        }
    }

    void placeTile()
    {
        tileObjects[cursorPosition.x, cursorPosition.y, cursorPosition.z] =
                Instantiate(tileSet.tiles[activeTileIndex], 
                            activePreview.transform.position,
                            activePreview.transform.rotation);
        tileRotations[cursorPosition.x, cursorPosition.y, cursorPosition.z] = activeTileRotation;
    }

    void removeTile()
    {
        Destroy(tileObjects[cursorPosition.x, cursorPosition.y, cursorPosition.z]);
        tileRotations[cursorPosition.x, cursorPosition.y, cursorPosition.z] = 0;
    }

    void updateUI() {
        const string text1 = "Rotation: ";
        const string text2 = "Selected Tile Rotation: ";

        previewRotationText.text = text1 + activeTileRotation;
        selectedTileRotationText.text = text2 + tileRotations[cursorPosition.x, cursorPosition.y, cursorPosition.z];
    }
}



