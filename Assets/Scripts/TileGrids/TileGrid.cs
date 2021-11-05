using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class TileGrid : MonoBehaviour {
    public const int TILE_EMPTY = -1;
    public const int NO_ROTATION = 0;
    public const string SAVE_FOLDER = "InputGrids";
    [SerializeField] private TextMeshProUGUI previewRotationText;
    [SerializeField] private TextMeshProUGUI selectedTileRotationText;
    [SerializeField] private GameObject cursorPrefab;
    [Header("Editor Controls")]
    [SerializeField] private TileSet tileSet;
    [SerializeField] private bool loadFromFileOnStart = false;
    [HideInInspector] public string currentSaveFile = "";
    private Vector3Int defaultDimensions = new Vector3Int(3, 3, 3);
    [HideInInspector] public Vector3Int dimensions;
    [HideInInspector] public bool areEditingControlsOn = false;
    [HideInInspector] public Array3D<int> tileIndices;
    [HideInInspector] public Array3D<int> tileRotations;
    private Array3D<GameObject> tileObjects;

    private GameObject cursor;
    [HideInInspector] public Vector3Int cursorPosition;
    private List<GameObject> tilePreviews;
    private int activeTileIndex;
    private int activeTileRotation;
    private GameObject activePreview;



    private void Start() {
        Boolean loadedFromFile;

        if (!loadFromFileOnStart) {
            loadedFromFile = false;
        } else {
            loadedFromFile = loadFromFile();
        }

        if (!loadedFromFile) {
            tileIndices = new Array3D<int>(defaultDimensions);
            tileRotations = new Array3D<int>(defaultDimensions);
            tileObjects = new Array3D<GameObject>(defaultDimensions);
            dimensions = defaultDimensions;
            fillWithEmpty();
        }


        if (loadedFromFile) {
            rebuildGrid();
        }





        cursor = Instantiate(cursorPrefab, transform.position, Quaternion.Euler(0, 90, 0));

        tilePreviews = new List<GameObject>();

        foreach (Tile tile in tileSet.tiles) {
            GameObject preview = Instantiate(tile.tilePrefab);
            preview.SetActive(false);
            tilePreviews.Add(preview);
        }

        changeTilePreview();
    }

    private void Update() {
        if (areEditingControlsOn) {
            gridCursorMovement();
            tileSelectionControls();
            rotationControls();
            placeAndRemoveControls();
            updateUI();
        }
    }

    public void saveToFile() {
        GridSave save = new GridSave(dimensions, tileIndices, tileRotations);
        string jsonString = JsonUtility.ToJson(save);
        Debug.Log(jsonString);
        File.WriteAllText($"{Application.dataPath}/{SAVE_FOLDER}/{currentSaveFile}", jsonString);
    }

    public bool loadFromFile() {
        if (String.IsNullOrWhiteSpace(currentSaveFile)) {
            return false;
        }

        try {
            string jsonString = File.ReadAllText($"{Application.dataPath}/{SAVE_FOLDER}/{currentSaveFile}");
            GridSave loadedSave = JsonUtility.FromJson<GridSave>(jsonString);
            if (tileObjects != null) {
                destroyTileObjects();
            }
            dimensions = loadedSave.dimensions;
            tileIndices = loadedSave.getTileIndices();
            tileRotations = loadedSave.getTileRotations();
            tileObjects = new Array3D<GameObject>(dimensions);

            return true;
        } catch (FileNotFoundException e) {
            Debug.Log("Loading grid file failed: file not found " + e.FileName);
        } catch (Exception e) {
            Debug.Log("Loading grid file failed: " + e.Message);
        }

        return false;
    }

    public void fillWithEmpty() {
        tileIndices.updateEach(value => TILE_EMPTY);
        tileRotations.updateEach(value => NO_ROTATION);
        rebuildGrid();
    }

    //przeladuj wszystkie plytki w przypadku np. ladowania z pliku
    public void rebuildGrid() {
        destroyTileObjects();

        tileIndices.forEach((position, value) => {
            placeTile(value, position, tileRotations.at(position));
        });
    }

    public void destroyTileObjects() {
        tileObjects.updateEach(value => {
            Destroy(value);
            return null;
        });
    }

    void gridCursorMovement() {
        const float cursorStep = 1f;

        if (MyInput.gridControls.disabled) {
            return;
        }

        bool keyW = MyInput.gridControls.keyW;
        bool keyA = MyInput.gridControls.keyA;
        bool keyS = MyInput.gridControls.keyS;
        bool keyD = MyInput.gridControls.keyD;

        bool heightToggle = MyInput.gridControls.heightToggle;

        Vector3Int cursorMovement;

        if (heightToggle) {
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

        // Debug.Log(cursorMovement);

        cursorPosition += cursorMovement;
        cursorPosition.x = Mathf.Clamp(cursorPosition.x, 0, dimensions.x - 1);
        cursorPosition.y = Mathf.Clamp(cursorPosition.y, 0, dimensions.y - 1);
        cursorPosition.z = Mathf.Clamp(cursorPosition.z, 0, dimensions.z - 1);

        cursor.transform.position = new Vector3(
            this.transform.position.x + cursorPosition.x * cursorStep,
            this.transform.position.y + cursorPosition.y * cursorStep,
            this.transform.position.z + cursorPosition.z * cursorStep
        );
    }

    void changeTilePreview() {
        Vector3 halfTileOffset = new Vector3(0.5f, 0, -0.5f);

        if (activePreview != null) {
            activePreview.transform.SetParent(null);
            activePreview.SetActive(false);
        }
        activePreview = tilePreviews[activeTileIndex];
        activePreview.SetActive(true);
        activePreview.transform.SetParent(cursor.transform);
        activePreview.transform.localPosition = Vector3.zero + halfTileOffset;
        activePreview.transform.rotation = Quaternion.Euler(0, activeTileRotation * 90, 0);
    }

    void tileSelectionControls() {
        bool indexChanged = false;
        if (MyInput.gridControls.nextTile) {
            activeTileIndex++;
            if (activeTileIndex > tileSet.tiles.Count - 1) {
                activeTileIndex = 0;
            }
            indexChanged = true;
        }
        if (MyInput.gridControls.previousTile) {
            activeTileIndex--;
            if (activeTileIndex < 0) {
                activeTileIndex = tileSet.tiles.Count - 1;
            }
            indexChanged = true;
        }
        if (indexChanged) {
            activeTileRotation = NO_ROTATION;
            changeTilePreview();
        }
    }

    void rotationControls() {
        Symmetry symmetry = tileSet.tiles[activeTileIndex].symmetry;
        if (MyInput.gridControls.rotate) {
            activeTileRotation += 1;
            if (activeTileRotation >= Tile.symmetryToNumOfRotations[symmetry]) {
                activeTileRotation = 0;
            }

            activePreview.transform.rotation = Quaternion.Euler(0, 90 * activeTileRotation, 0);
        }
    }

    void placeAndRemoveControls() {
        Vector3 halfTileOffset = new Vector3(0.5f, 0, -0.5f);

        if (MyInput.gridControls.place) {
            placeTile(activeTileIndex, cursorPosition, activeTileRotation);
        }
        if (MyInput.gridControls.remove) {
            removeTile(cursorPosition);
        }
    }

    void placeTile(int tileSetIndex, Vector3Int position, int rotation) {
        removeTile(position);

        Vector3 halfTileOffset = new Vector3(-0.5f, 0, -0.5f);

        if (tileSetIndex != TILE_EMPTY) {
            GameObject newTileObject = Instantiate(tileSet.tiles[tileSetIndex].tilePrefab,
                        transform.position + position + halfTileOffset,
                        Quaternion.Euler(0, rotation * 90, 0));
            tileObjects.set(position, newTileObject);
        }

        tileIndices.set(position, tileSetIndex);
        tileRotations.set(position, rotation);
    }

    void removeTile(Vector3Int position) {
        Destroy(tileObjects.at(position));
        tileRotations.set(position, NO_ROTATION);
        tileIndices.set(position, TILE_EMPTY);
    }

    public void resize(Vector3Int newDimensions) {
        if (newDimensions.x <= 0 || newDimensions.y <= 0 || newDimensions.z <= 0) {
            Debug.LogError("Error resizing the grid: invalid dimensions");
            return;
        }

        Array3D<int> newTileIndices = new Array3D<int>(newDimensions);
        Array3D<int> newTileRotations = new Array3D<int>(newDimensions);
        Array3D<GameObject> newTileObjects = new Array3D<GameObject>(newDimensions);

        //copy overlaping tiles their rotations and spawned prefabs
        newTileIndices.updateEach((x, y, z, value) => {
            if (x < dimensions.x && y < dimensions.y && z < dimensions.z) {
                return tileIndices.at(x, y, z);
            } else {
                return TILE_EMPTY;
            }
        });

        newTileRotations.updateEach((x, y, z, value) => {
            if (x < dimensions.x && y < dimensions.y && z < dimensions.z) {
                return tileRotations.at(x, y, z);
            } else {
                return NO_ROTATION;
            }
        });

        newTileObjects.updateEach((x, y, z, value) => {
            if (x < dimensions.x && y < dimensions.y && z < dimensions.z) {
                return tileObjects.at(x, y, z);
            } else {
                return null;
            }
        });

        //cleanup out of bounds tiles
        tileObjects.forEach((x, y, z) => {
            if (x >= newDimensions.x || y >= newDimensions.y || z >= newDimensions.z) {
                Destroy(tileObjects.at(x, y, z));
            }
        });

        tileIndices = newTileIndices;
        tileRotations = newTileRotations;
        tileObjects = newTileObjects;
        dimensions = newDimensions;
    }

    void updateUI() {
        const string text1 = "Rotation: ";
        const string text2 = "Selected Tile Rotation: ";

        previewRotationText.text = text1 + activeTileRotation;
        selectedTileRotationText.text = text2 + tileRotations.at(cursorPosition);
    }
}



