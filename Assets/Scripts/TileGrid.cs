using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    [SerializeField] Vector3Int dimensions = new Vector3Int(5,5,5);
    private Vector3 tileSize = Vector3.one;
    
    private Tile[,,] tiles;

    private void Start() {
        tiles = new Tile[dimensions.x, dimensions.y, dimensions.z];
    }
}
