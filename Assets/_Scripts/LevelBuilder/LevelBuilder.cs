using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(WfcRunner))]
public class LevelBuilder : MonoBehaviour {
    private WfcRunner wfcRunner;
    [SerializeField] private TileGrid levelGrid;
    [SerializeField] Vector3Int generatedGridDimensions;
    Vector3Int fullDimensions;
    [SerializeField] int upTileCapIndex;
    [SerializeField] int downTileCapIndex;
    [SerializeField] int sideTileCapIndex;


    void Start() {
        var a = new List<int>();

        wfcRunner = GetComponent<WfcRunner>();
        wfcRunner.output = levelGrid;
        //levelGrid.clear();
        //leave 1 tile of margin on each side for ending hole blocking tiles
        Vector3Int oneTileMargin = Vector3Int.one * 2;
        fullDimensions = generatedGridDimensions + oneTileMargin;
        levelGrid.resize(fullDimensions);

        int[,,] generatedTiles;
        bool generationSuccess = wfcRunner.runAdjacentModel(out generatedTiles, generatedGridDimensions);

        if (!generationSuccess) {
            Debug.LogError("WFC failed!");
            return;
        }

        Grid3D<int> tileIndices = levelGrid.tileIndices;
        Grid3D<int> tileRotations = levelGrid.tileRotations;

        //leave 1 empty tile border
        for (int x = 1; x < fullDimensions.x - 1; x++) {
            for (int y = 1; y < fullDimensions.y - 1; y++) {
                for (int z = 1; z < fullDimensions.z - 1; z++) {
                    tileIndices.set(x, y, z, TileUtils.modelIndexToTileIndex(generatedTiles[x - 1, y - 1, z - 1]));
                    tileRotations.set(x, y, z, TileUtils.modelIndexToRotation(generatedTiles[x - 1, y - 1, z - 1]));
                }
            }
        }
        capOffTileEnds();
        levelGrid.rebuildGrid();
    }

    private void capOffTileEnds() {
        int Xmax = fullDimensions.x;
        int Ymax = fullDimensions.y;
        int Zmax = fullDimensions.z;
        int setX, setY, setZ;
        Directions3D testDirection;

        var tileIndices = levelGrid.tileIndices;
        var tileRotations = levelGrid.tileRotations;
        var connectionData = wfcRunner.samplerResult.connections;

        setY = Ymax;
        testDirection = Directions3D.UP;
        for (int x = 1; x < Xmax - 1; x++) {
            for (int z = 1; z < Zmax - 1; z++) {
                int tileIndex = tileIndices.at(x, setY - 2, z);
                int tileRotation = tileIndices.at(x, setY - 2, z);
                if (connectionData[tileIndex].canConnectFromDirection(testDirection, tileRotation)) {
                    levelGrid.placeTile(upTileCapIndex, new Vector3Int(x, setY - 1, z), TileGrid.NO_ROTATION);
                }
            }
        }

        setY = 0;
        testDirection = Directions3D.DOWN;
        for (int x = 1; x < Xmax - 1; x++) {
            for (int z = 1; z < Zmax - 1; z++) {
                int tileIndex = tileIndices.at(x, setY + 1, z);
                int tileRotation = tileIndices.at(x, setY + 1, z);
                if (connectionData[tileIndex].canConnectFromDirection(testDirection, tileRotation)) {
                    levelGrid.placeTile(downTileCapIndex, new Vector3Int(x, setY, z), TileGrid.NO_ROTATION);
                }
            }
        }

        setZ = Zmax;
        testDirection = Directions3D.FORWARD;
        for (int x = 1; x < Xmax - 1; x++) {
            for (int y = 1; y < Ymax - 1; y++) {

            }
        }

        setZ = 0;
        testDirection = Directions3D.BACK;
        for (int x = 1; x < Xmax - 1; x++) {
            for (int y = 1; y < Ymax - 1; y++) {

            }
        }

        setX = Xmax;
        testDirection = Directions3D.RIGHT;
        for (int y = 1; y < Ymax - 1; y++) {
            for (int z = 1; z < Zmax - 1; z++) {

            }
        }

        setX = 0;
        testDirection = Directions3D.LEFT;
        for (int y = 1; y < Ymax - 1; y++) {
            for (int z = 1; z < Zmax - 1; z++) {

            }
        }
    }
}

