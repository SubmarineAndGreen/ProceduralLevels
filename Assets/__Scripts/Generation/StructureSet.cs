using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StructureSet", menuName = "wfc/StructureSet", order = 0)]
public class StructureSet : ScriptableObject {
    public List<Structure> structures;
}