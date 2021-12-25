using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Tile", menuName = "wfc/Tile", order = 0)]
public class Tile : ScriptableObject {
    public GameObject tilePrefab;
    public RotationalSymmetry symmetry;
    // public bool excludeFromGeneration = false;
    // const int numberOfDirections = 6;
    // public bool[] possibleConnections = new bool[numberOfDirections];
    public static Dictionary<RotationalSymmetry, int> symmetryToNumberOfRotations = new Dictionary<RotationalSymmetry, int>() {
        {RotationalSymmetry.NONE, 4},
        {RotationalSymmetry.I, 2},
        {RotationalSymmetry.X, 1}
    };
}

public enum RotationalSymmetry {
    NONE,
    I,
    X
}

