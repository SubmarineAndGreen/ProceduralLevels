using System;
using UnityEngine;

[Serializable]
public struct GridSave
{
    public Vector3Int dimensions;
    public TileModel[] tiles;

    public GridSave(Vector3Int dimensions, int[,,] tileIndices, int[,,] tileRotations)
    {
        this.dimensions = dimensions;

        int tileCount = dimensions.x * dimensions.y * dimensions.z;

        this.tiles = new TileModel[tileCount];

        int currentTile = 0;
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    tiles[currentTile++] =
                        new TileModel(tileIndices[x, y, z], tileRotations[x, y, z], new Vector3Int(x, y, z));
                }
            }
        }
    }

    public int[,,] getTileIndices()
    {
        int[,,] indices = new int[dimensions.x, dimensions.y, dimensions.z];

        foreach (TileModel model in tiles)
        {
            indices[model.position.x, model.position.y, model.position.z] = model.tileIndex;
        }

        return indices;
    }

    public int[,,] getTileRotations() {
        int[,,] rotations = new int[dimensions.x, dimensions.y, dimensions.z];

        foreach (TileModel model in tiles)
        {
            rotations[model.position.x, model.position.y, model.position.z] = model.rotation;
        }

        return rotations;
    }
}
