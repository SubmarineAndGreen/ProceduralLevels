using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "wfc/Tile", order = 0)]
public class Tile : ScriptableObject {
    public int tileIndex;
    public GameObject tilePrefab;
    public TileRotations allowedRotation;
    public TileSymmetries symmetry;
}