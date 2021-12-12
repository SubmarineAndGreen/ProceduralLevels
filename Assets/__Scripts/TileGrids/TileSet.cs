using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Tile Set", menuName = "wfc/Tile Set", order = 0)]
public class TileSet : ScriptableObject {
    public int emptyTileIndex;
    public List<Tile> tiles;
}   