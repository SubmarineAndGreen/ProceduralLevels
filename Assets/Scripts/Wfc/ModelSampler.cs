using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSampler : MonoBehaviour
{
    const int nOfRotations = 4;
    [SerializeField] TileGrid inputGrid;
    [SerializeField] bool ignoreEmptyTiles;

    public List<GridAdjacencyConstraint> simpleTiledModel(Vector3Int dimensions, int[,,] tileIndices)
    {
        var adjcacencies = new List<GridAdjacencyConstraint>();
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {

                }
            }
        }

        void searchNeighbors(Vector3Int origin)
        {
            foreach(var item in DirectionsToVectors) {
                Vector3Int neighborPosition = origin + item.Value;
                if(isInBounds(inputGrid.dimensions ,neighborPosition)) {
                    adjcacencies.Add(new GridAdjacencyConstraint(
                        tileIndexToModelIndex(
                            inputGrid.tileIndices[origin.x, origin.y, origin.z],
                            inputGrid.tileRotations[origin.x, origin.y, origin.z]
                        ),
                        tileIndexToModelIndex(
                            inputGrid.tileIndices[neighborPosition.x, neighborPosition.y, neighborPosition.z],
                            inputGrid.tileRotations[neighborPosition.x, neighborPosition.y, neighborPosition.z]
                        ),
                        item.Key
                    ))
                }
            }
        }
    }

    bool isInBounds(Vector3Int dimensions, Vector3Int position)
    {
        if (position.x >= dimensions.x || position.y >= dimensions.y || position.z >= dimensions.z)
        {
            return false;
        }
        if (position.x < 0 || position.y < 0 || position.z < 0)
        {
            return false;
        }
        return true;
    }

    Dictionary<Directions3D, Vector3Int> DirectionsToVectors = new Dictionary<Directions3D, Vector3Int>()
    {
        {Directions3D.UP, Vector3Int.up},
        {Directions3D.DOWN, Vector3Int.down},
        {Directions3D.FORWARD, Vector3Int.forward},
        {Directions3D.RIGHT, Vector3Int.right},
        {Directions3D.BACK, Vector3Int.back},
        {Directions3D.LEFT, Vector3Int.left}
    };

    int tileIndexToModelIndex(int index, int rotation) {
        return index * nOfRotations + rotation;
    }

    int modelIndexToTileIndex(int index) {
        return (index - (index % nOfRotations)) / nOfRotations;
    }

    int modelIndexToRotation(int index) {
        return index % nOfRotations;
    }
}

