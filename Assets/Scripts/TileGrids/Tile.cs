using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Tile  {
    public GameObject tilePrefab;
    public Symmetry symmetry;

    public static Dictionary<Symmetry, int> symmetryToNumOfRotations = new Dictionary<Symmetry, int>() {
        {Symmetry.NONE, 4},
        {Symmetry.I, 2},
        {Symmetry.X, 0}
    };
}

public enum Symmetry {
    NONE,
    I,
    X
}