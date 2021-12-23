using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StructureSet", menuName = "wfc/StructureSet", order = 0)]
public class StructureSet : ScriptableObject {
    public List<StructureFrequency> frequencies;
}

[System.Serializable]
public class StructureFrequency {
    public Structure structure;
    public float frequency;
}