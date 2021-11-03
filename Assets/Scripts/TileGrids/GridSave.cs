using System;
using UnityEngine;

[Serializable]
public struct GridSave {
    public Vector3Int dimensions;
    // public TileModel[] tiles;
    public int[] tileIndices;
    public int[] tileRotations;

    public GridSave(Vector3Int dimensions, Array3D<int> tileIndices, Array3D<int> tileRotations) {
        this.dimensions = dimensions;
        this.tileIndices = tileIndices.flatArray();
        this.tileRotations = tileRotations.flatArray();
    }

    public Array3D<int> getTileIndices() {
        return new Array3D<int>(dimensions, tileIndices);
    }

    public Array3D<int>  getTileRotations() {
        return new Array3D<int>(dimensions, tileRotations);
    }
}
