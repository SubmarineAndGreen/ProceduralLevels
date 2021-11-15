using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WfcRunner))]
public class LevelBuilder : MonoBehaviour {
    private WfcRunner wfcRunner;
    [SerializeField] private TileGrid levelGrid;
    [SerializeField] Vector3Int generatedGridDimensions;

    void Start() {
        wfcRunner = GetComponent<WfcRunner>();
        wfcRunner.output = levelGrid;
        //levelGrid.clear();
        //leave 1 tile of margin on each side for ending hole blocking tiles
        Vector3Int oneTileMargin = Vector3Int.one * 2;
        Vector3Int fullDimensions = generatedGridDimensions + oneTileMargin;
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

        levelGrid.rebuildGrid();
    }

}
