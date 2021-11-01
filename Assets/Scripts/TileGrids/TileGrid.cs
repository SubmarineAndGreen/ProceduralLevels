using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class TileGrid : MonoBehaviour
{
    [HideInInspector] public const int TILE_EMPTY = -1;
    [SerializeField] private TextMeshProUGUI previewRotationText;
    [SerializeField] private TextMeshProUGUI selectedTileRotationText;
    [SerializeField] private GameObject cursorPrefab;
    [Header("Editor Controls")]
    [SerializeField] private TileSet tileSet;
    [HideInInspector] public string currentSaveFile;
    public const string saveFolder = "InputGrids";
    [HideInInspector] public Vector3Int dimensions = new Vector3Int(3, 3, 3);
    [HideInInspector] public int[,,] tileIndices;
    [HideInInspector] public int[,,] tileRotations;
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
        Boolean loadedFromFile = loadFromFile();

        if (!loadedFromFile)
        {
            tileIndices = new int[dimensions.x, dimensions.y, dimensions.z];
            tileRotations = new int[dimensions.x, dimensions.y, dimensions.z];
        }

        tileObjects = new GameObject[dimensions.x, dimensions.y, dimensions.z];

        if (loadedFromFile)
        {
            rebuildGrid();
        }




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

    public void clearGrid()
    {
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    tileIndices[x, y, z] = TILE_EMPTY;
                    tileRotations[x, y, z] = 0;
                }
            }
        }
        rebuildGrid();
    }

    //przeladuj wszystkie plytki w przypadku np. ladowania z pliku
    public void rebuildGrid()
    {
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    if (tileIndices[x, y, z] == TILE_EMPTY)
                    {
                        removeTile(new Vector3Int(x, y, z));
                    }
                    else
                    {
                        Destroy(tileObjects[x, y, z]);
                        placeTile(tileIndices[x, y, z], new Vector3Int(x, y, z), tileRotations[x, y, z]);
                    }
                }
            }
        }
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
        activePreview.transform.rotation = Quaternion.Euler(0, activeTileRotation * 90, 0);
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
            removeTile(cursorPosition);
            placeTile(activeTileIndex, cursorPosition, activeTileRotation);
        }
        if (MyInput.gridControls.remove)
        {
            removeTile(cursorPosition);
        }
    }

    void placeTile(int tileSetIndex, Vector3Int position, int rotation)
    {
        Vector3 halfTileOffset = new Vector3(-0.5f, 0, -0.5f);
        tileIndices[position.x, position.y, position.z] = tileSetIndex;
        tileObjects[position.x, position.y, position.z] =
                Instantiate(tileSet.tiles[tileSetIndex],
                            transform.position + position + halfTileOffset,
                            Quaternion.Euler(0, rotation * 90, 0));
        tileRotations[position.x, position.y, position.z] = rotation;
    }

    void removeTile(Vector3Int position)
    {
        Destroy(tileObjects[position.x, position.y, position.z]);
        tileRotations[position.x, position.y, position.z] = 0;
        tileIndices[position.x, position.y, position.z] = TILE_EMPTY;
    }

    public void resize(Vector3Int newDimensions)
    {
        if (newDimensions.x <= 1 || newDimensions.y <= 0 || newDimensions.z <= 0)
        {
            Debug.LogError("Error resizing the grid: minimum grid size is 2x2x2");
            return;
        }

        int[,,] newTileIndices = new int[newDimensions.x, newDimensions.y, newDimensions.z];
        int[,,] newTileRotations = new int[newDimensions.x, newDimensions.y, newDimensions.z];
        GameObject[,,] newTileObjects = new GameObject[newDimensions.x, newDimensions.y, newDimensions.z];

        //copy overlaping tiles
        for (int x = 0; x < newDimensions.x; x++)
        {
            for (int y = 0; y < newDimensions.y; y++)
            {
                for (int z = 0; z < newDimensions.z; z++)
                {
                    if (x < dimensions.x && y < dimensions.y && z < dimensions.z)
                    {
                        newTileIndices[x, y, z] = tileIndices[x, y, z];
                        newTileRotations[x, y, z] = tileRotations[x, y, z];
                        newTileObjects[x, y, z] = tileObjects[x, y, z];
                    }
                    else
                    {
                        newTileIndices[x, y, z] = TILE_EMPTY;
                        newTileRotations[x, y, z] = 0;
                    }
                }
            }
        }

        //cleanup out of bounds tiles
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    if (x >= newDimensions.x || y >= newDimensions.y || z >= newDimensions.z)
                    {
                        Destroy(tileObjects[x, y, z]);
                    }
                }
            }
        }

        tileIndices = newTileIndices;
        tileRotations = newTileRotations;
        tileObjects = newTileObjects;
        dimensions = newDimensions;
    }

    void updateUI()
    {
        const string text1 = "Rotation: ";
        const string text2 = "Selected Tile Rotation: ";

        previewRotationText.text = text1 + activeTileRotation;
        selectedTileRotationText.text = text2 + tileRotations[cursorPosition.x, cursorPosition.y, cursorPosition.z];
    }
}



