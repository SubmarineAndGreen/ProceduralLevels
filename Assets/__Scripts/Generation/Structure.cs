using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Structure", menuName = "wfc/Structure", order = 0)]
public class Structure : ScriptableObject {
    public List<StructureTile> tiles;
}