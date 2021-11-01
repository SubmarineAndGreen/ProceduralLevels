
using System;
using UnityEngine;

[Serializable]
public struct TileModel {
    public int tileIndex;
    public int rotation;
    public Vector3Int position;

    public TileModel(int tileIndex, int rotation, Vector3Int position)
    {
        this.tileIndex = tileIndex;
        this.rotation = rotation;
        this.position = position;
    }
}