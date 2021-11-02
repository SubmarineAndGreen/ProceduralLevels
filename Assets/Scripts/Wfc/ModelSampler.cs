using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModelSampler : MonoBehaviour
{
    public const string SAVE_FOLDER = "WfcModels";
    const int nOfRotations = 4;
    [SerializeField] TileGrid inputGrid;
    [SerializeField] bool ignoreEmptyTiles;

    public HashSet<GridAdjacencyConstraint> run()
    {
        return simpleTiledModel(inputGrid.dimensions, inputGrid.tileIndices);
    }

    private HashSet<GridAdjacencyConstraint> simpleTiledModel(Vector3Int dimensions, int[,,] tileIndices)
    {
        var constraints = new HashSet<GridAdjacencyConstraint>();
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
            foreach (var item in ACUtils.DirectionsToVectors)
            {
                Vector3Int neighborPosition = origin + item.Value;

                if (ACUtils.isInBounds(inputGrid.dimensions, neighborPosition))
                {
                    //dont make constraints where some tile is an empty tile
                    if (ignoreEmptyTiles &&
                    (
                       inputGrid.tileIndices[neighborPosition.x, neighborPosition.y, neighborPosition.z] == TileGrid.TILE_EMPTY
                    || inputGrid.tileIndices[origin.x, origin.y, origin.z] == TileGrid.TILE_EMPTY
                    ))
                    {
                        continue;
                    }

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

    int tileIndexToModelIndex(int index, int rotation)
    {
        return index * nOfRotations + rotation;
    }

    int modelIndexToTileIndex(int index)
    {
        return (index - (index % nOfRotations)) / nOfRotations;
    }

    int modelIndexToRotation(int index)
    {
        return index % nOfRotations;
    }


}



