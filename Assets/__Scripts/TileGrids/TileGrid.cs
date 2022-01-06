using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static InputManager;

public class TileGrid : MonoBehaviour {
    public const int TILE_EMPTY = -1;
    public const int NO_ROTATION = 0;
    public const string SAVE_FOLDER = "InputGrids";
    public bool debugUI = true;
    public bool cursorHidden = true;
    [SerializeField] private TextMeshProUGUI previewRotationText;
    [SerializeField] private TextMeshProUGUI selectedTileRotationText;
    [SerializeField] private TextMeshProUGUI tileText;
    [SerializeField] private GameObject cursorPrefab;
    [Header("Editor Controls")]
    // public TileSet defaultTileSet;
    public TileSet[] tileSets;
    [HideInInspector] public int currentTilesetIndex = 0;
    [SerializeField] private bool loadFromFileOnStart = false;
    [HideInInspector] public string currentSaveFile = "";
    private Vector3Int defaultDimensions = new Vector3Int(3, 3, 3);
    [HideInInspector] public Vector3Int dimensions;
    [HideInInspector] public bool areEditingControlsOn = false;
    [HideInInspector] public Grid3D<int> tileIndices;
    [HideInInspector] public Grid3D<int> tileRotations;
    [HideInInspector] public Grid3D<int> tileSetIndices;
    [HideInInspector] public Grid3D<GameObject> tileObjects;

    private GameObject cursor;
    [HideInInspector] public Vector3Int cursorPosition;
    private List<GameObject> tilePreviews;
    private int activeTileIndex;
    private int activeTileRotation;
    private GameObject activePreview;



    private void Awake() {
        cursor = Instantiate(cursorPrefab, transform.position, Quaternion.Euler(0, 90, 0));
        cursor.transform.SetParent(this.transform);

        if (tileSets.GetLength(0) == 0) {
            cursor.SetActive(false);
            return;
        }

        bool loadedFromFile;

        // if (tileSets == null || tileSets.GetLength(0) == 0) {
        //     tileSets = new TileSet[1];
        //     tileSets[0] = defaultTileSet;
        // }

        if (!loadFromFileOnStart) {
            loadedFromFile = false;
        } else {
            loadedFromFile = loadFromFile();
        }

        if (!loadedFromFile) {
            tileIndices = new Grid3D<int>(defaultDimensions);
            tileRotations = new Grid3D<int>(defaultDimensions);
            tileObjects = new Grid3D<GameObject>(defaultDimensions);
            tileSetIndices = new Grid3D<int>(defaultDimensions);
            dimensions = defaultDimensions;
            clear();
        }


        if (loadedFromFile) {
            rebuildGrid();
        }

        reloadTilePreviews();

        if (cursorHidden) {
            cursor.SetActive(false);
        }
    }

    private void Update() {
        if (areEditingControlsOn && inputs.GridEditor.enabled) {
            gridCursorMovement();
            tileSelectionControls();
            rotationControls();
            placeAndRemoveControls();
            if (debugUI) {
                updateUI();
            }
        }
    }

    public void saveToFile() {
        GridSave save = new GridSave(dimensions, tileIndices, tileRotations, tileSetIndices);
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
            tileSetIndices = loadedSave.getTileSetIndices();

            tileObjects = new Grid3D<GameObject>(dimensions);

            return true;
        } catch (FileNotFoundException e) {
            Debug.Log("Loading grid file failed: file not found " + e.FileName);
        } catch (Exception e) {
            Debug.Log("Loading grid file failed: " + e.Message);
        }

        return false;
    }

    public void clear() {
        tileIndices.updateEach(value => TILE_EMPTY);
        tileRotations.updateEach(value => NO_ROTATION);
        tileSetIndices.updateEach(value => currentTilesetIndex);
        rebuildGrid();
    }

    //przeladuj wszystkie plytki w przypadku np. ladowania z pliku
    public void rebuildGrid() {
        // destroyTileObjects();

        tileIndices.forEach((position) => {
            createTile(position);
        });
    }

    public void destroyTileObjects() {
        tileObjects.updateEach(tile => {
            Destroy(tile);
            return null;
        });
    }

    void gridCursorMovement() {
        const float cursorStep = 1f;

        bool heightToggle = inputs.GridEditor.HeightModifier.ReadValue<float>() == 1;

        Vector2Int keyboardInput = Vector2Int.zero;
        if (inputs.GridEditor.CursorMovement.triggered) {
            keyboardInput = inputs.GridEditor.CursorMovement.ReadValue<Vector2>().toVector2Int();
        }
        Vector3Int cursorMovement;

        if (heightToggle) {
            cursorMovement = new Vector3Int(
                0,
                keyboardInput.y,
                0
            );
        } else {
            cursorMovement = new Vector3Int(
                keyboardInput.x,
                0,
                keyboardInput.y
            );
        }

        cursorPosition += cursorMovement;
        cursorPosition.x = Mathf.Clamp(cursorPosition.x, 0, dimensions.x - 1);
        cursorPosition.y = Mathf.Clamp(cursorPosition.y, 0, dimensions.y - 1);
        cursorPosition.z = Mathf.Clamp(cursorPosition.z, 0, dimensions.z - 1);

        // Debug.Log(cursorPosition);

        cursor.transform.position = new Vector3(
            this.transform.position.x + cursorPosition.x * cursorStep,
            this.transform.position.y + cursorPosition.y * cursorStep,
            this.transform.position.z + cursorPosition.z * cursorStep
        );
    }

    void changeTilePreview() {
        Vector3 halfTileOffset = new Vector3(0.5f, 0, -0.5f);

        if (activePreview != null) {
            activePreview.transform.SetParent(this.transform);
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
        if (inputs.GridEditor.NextTile.triggered) {
            activeTileIndex++;
            if (activeTileIndex > tileSets[currentTilesetIndex].tiles.Count - 1) {
                activeTileIndex = 0;
            }
            indexChanged = true;
        }
        if (inputs.GridEditor.PreviousTile.triggered) {
            activeTileIndex--;
            if (activeTileIndex < 0) {
                activeTileIndex = tileSets[currentTilesetIndex].tiles.Count - 1;
            }
            indexChanged = true;
        }
        if (indexChanged) {
            activeTileRotation = NO_ROTATION;
            changeTilePreview();
        }
    }

    void rotationControls() {
        RotationalSymmetry symmetry = tileSets[currentTilesetIndex].tiles[activeTileIndex].symmetry;
        if (inputs.GridEditor.RotateTile.triggered) {
            activeTileRotation += 1;
            if (activeTileRotation >= Tile.symmetryToNumberOfRotations[symmetry]) {
                activeTileRotation = 0;
            }

            activePreview.transform.rotation = Quaternion.Euler(0, 90 * activeTileRotation, 0);
        }
    }

    void placeAndRemoveControls() {
        if (inputs.GridEditor.PlaceTile.triggered) {
            placeTile(activeTileIndex, currentTilesetIndex, cursorPosition, activeTileRotation);
        }
        if (inputs.GridEditor.DeleteTile.triggered) {
            removeTile(cursorPosition);
        }
    }

    public void placeTile(int tileIndex, int tileSetIndex, Vector3Int position, int rotation) {
        if (tileObjects.at(position) != null) {
            Destroy(tileObjects.at(position));
        }

        Vector3 halfTileOffset = new Vector3(-0.5f, 0, -0.5f);

        if (tileIndex != TILE_EMPTY) {
            GameObject newTileObject = Instantiate(tileSets[tileSetIndex].tiles[tileIndex].tilePrefab,
                        transform.position + position + halfTileOffset,
                        Quaternion.Euler(0, rotation * 90, 0));
            newTileObject.transform.SetParent(this.transform, true);
            tileObjects.set(position, newTileObject);
        }

        tileIndices.set(position, tileIndex);
        tileRotations.set(position, rotation);
        tileSetIndices.set(position, tileSetIndex);
    }

    public void createTile(Vector3Int position) {
        if (tileObjects.at(position) != null) {
            Destroy(tileObjects.at(position));
        }

        Vector3 halfTileOffset = new Vector3(-0.5f, 0, -0.5f);

        int tileIndex = tileIndices.at(position);
        int tileSetIndex = tileSetIndices.at(position);
        int rotation = tileRotations.at(position);

        if (tileIndex != TILE_EMPTY && tileIndex != tileSets[currentTilesetIndex].emptyTileIndex) {
            GameObject newTileObject = Instantiate(tileSets[tileSetIndex].tiles[tileIndex].tilePrefab,
            transform.position + position + halfTileOffset,
            Quaternion.Euler(0, rotation * 90, 0));
            newTileObject.transform.SetParent(this.transform, true);
            tileObjects.set(position, newTileObject);
        }
    }

    public void placeTileDontInstantiate(int tileIndex, int tileSetIndex, Vector3Int position, int rotation) {
        removeTile(position);
        tileIndices.set(position, tileIndex);
        tileRotations.set(position, rotation);
        tileSetIndices.set(position, tileSetIndex);
    }

    void removeTile(Vector3Int position) {
        Destroy(tileObjects.at(position));
        tileRotations.set(position, NO_ROTATION);
        tileIndices.set(position, TILE_EMPTY);
        tileSetIndices.set(position, currentTilesetIndex);
    }

    public void resizePerserveTiles(Vector3Int newDimensions) {
        if (newDimensions.x <= 0 || newDimensions.y <= 0 || newDimensions.z <= 0) {
            Debug.LogError("Error resizing the grid: invalid dimensions");
            return;
        }

        Grid3D<int> newTileIndices = new Grid3D<int>(newDimensions);
        Grid3D<int> newTileRotations = new Grid3D<int>(newDimensions);
        Grid3D<int> newTileSetIndices = new Grid3D<int>(newDimensions);
        Grid3D<GameObject> newTileObjects = new Grid3D<GameObject>(newDimensions);

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

        newTileSetIndices.updateEach((x, y, z, value) => {
            if (x < dimensions.x && y < dimensions.y && z < dimensions.z) {
                return tileSetIndices.at(x, y, z);
            } else {
                return currentTilesetIndex;
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
        tileSetIndices = newTileSetIndices;
        tileObjects = newTileObjects;
        dimensions = newDimensions;
    }

    void updateUI() {
        const string text1 = "Rotation: ";
        const string text2 = "Selected Tile Rotation: ";
        const string text3 = "{0}: {1}";

        previewRotationText.text = text1 + activeTileRotation;
        selectedTileRotationText.text = text2 + tileRotations.at(cursorPosition);
        tileText.text = string.Format(text3, activeTileIndex, tileSets[tileSetIndices.at(cursorPosition)].tiles[activeTileIndex].tilePrefab.name);
    }

    void reloadTilePreviews() {
        tilePreviews = new List<GameObject>();

        foreach (Tile tile in tileSets[currentTilesetIndex].tiles) {
            GameObject preview = Instantiate(tile.tilePrefab);
            preview.SetActive(false);
            preview.transform.SetParent(this.transform);
            tilePreviews.Add(preview);
        }

        changeTilePreview();
    }

    public void toggleCursor() {
        cursorHidden = !cursorHidden;
        cursor.SetActive(!cursorHidden);
    }
}



