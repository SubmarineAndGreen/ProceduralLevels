using System;
using UnityEngine;

[Serializable]
public struct GridSave {
    public Vector3Int dimensions;
    // public TileModel[] tiles;
    public int[] tileIndices;
    public int[] tileRotations;

    public GridSave(Vector3Int dimensions, Grid3D<int> tileIndices, Grid3D<int> tileRotations) {
        this.dimensions = dimensions;
        this.tileIndices = tileIndices.flatArray();
        this.tileRotations = tileRotations.flatArray();
    }

    public Grid3D<int> getTileIndices() {
        return new Grid3D<int>(dimensions, tileIndices);
    }

    public Grid3D<int>  getTileRotations() {
        return new Grid3D<int>(dimensions, tileRotations);
    }
}
