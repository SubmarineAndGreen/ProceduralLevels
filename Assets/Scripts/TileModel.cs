
using System;
using UnityEngine;

[Serializable]
public struct TileModel {
    public int tileIndex;
    public Vector3Int position;

    public TileModel(int tileIndex, Vector3Int position)
    {
        this.tileIndex = tileIndex;
        this.position = position;
    }
}