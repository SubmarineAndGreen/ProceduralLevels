using System;
using UnityEngine;

[Serializable]
public struct GridSave {
    public Vector3Int dimensions;
    // public TileModel[] tiles;
    public int[] tileIndices;
    public int[] tileRotations;
    public int[] tileSetIndices;

    public GridSave(Vector3Int dimensions, Grid3D<int> tileIndices, Grid3D<int> tileRotations, Grid3D<int> tileSetIndices) {
        this.dimensions = dimensions;
        this.tileIndices = tileIndices.flatArray();
        this.tileRotations = tileRotations.flatArray();
        this.tileSetIndices = tileSetIndices.flatArray();
    }

    public Grid3D<int> getTileIndices() {
        return new Grid3D<int>(dimensions, tileIndices);
    }

    public Grid3D<int>  getTileRotations() {
        return new Grid3D<int>(dimensions, tileRotations);
    }

    public Grid3D<int> getTileSetIndices() {
        //quick fix remove later
        if(tileSetIndices == null) {
            tileSetIndices = new Grid3D<int>(dimensions).flatArray();
        }
        return new Grid3D<int>(dimensions, tileSetIndices);
    }
}
