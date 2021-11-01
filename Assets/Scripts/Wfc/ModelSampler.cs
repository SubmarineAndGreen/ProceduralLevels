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
        var constraints = new List<GridAdjacencyConstraint>();
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    searchNeighbors(new Vector3Int(x, y, z));
                }
            }
        }

        void searchNeighbors(Vector3Int origin)
        {
            foreach(var item in ACUtils.DirectionsToVectors) {
                Vector3Int neighborPosition = origin + item.Value;
                if(ACUtils.isInBounds(inputGrid.dimensions ,neighborPosition)) {
                    constraints.Add(new GridAdjacencyConstraint(
                        tileIndexToModelIndex(
                            inputGrid.tileIndices[origin.x, origin.y, origin.z],
                            inputGrid.tileRotations[origin.x, origin.y, origin.z]
                        ),
                        tileIndexToModelIndex(
                            inputGrid.tileIndices[neighborPosition.x, neighborPosition.y, neighborPosition.z],
                            inputGrid.tileRotations[neighborPosition.x, neighborPosition.y, neighborPosition.z]
                        ),
                        item.Key
                    ));
                }
            }
        }

        return constraints;
    }

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

