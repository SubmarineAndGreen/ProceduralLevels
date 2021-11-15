using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Tile {
    public GameObject tilePrefab;
    public RotationalSymmetry symmetry;
    public bool openTop;
    public bool openBottom;
    public List<Sides> openSides;

    public bool isSideOpen(Sides side, int rotation) {
        //sides are enumerated clockwise and put clockwise in all sides array
        //rotations clockwise are ints from 0 to 3
        //side rotated by 90deg is at index + 1
        //if we overshoot last side (3 or Xminus)
        //loop back to beginning
        //x % 4 = (... 0, 1, 2, 3, 0, 1, 2, 3, 0 ...)
        //'Xminus' and rotation = 3 gives us 3 + 3 % 4 = 2 => 'Zminus' (OK!)
        Sides sideAfterRotation = allSides[(int)side + rotation % 4];
        return openSides.Contains(sideAfterRotation);
    }

    public static Dictionary<RotationalSymmetry, int> symmetryToNumberOfRotations = new Dictionary<RotationalSymmetry, int>() {
        {RotationalSymmetry.NONE, 4},
        {RotationalSymmetry.I, 2},
        {RotationalSymmetry.X, 1}
    };

    private static Sides[] allSides = { Sides.Zplus, Sides.Xplus, Sides.Zminus, Sides.Xminus };
}

public enum RotationalSymmetry {
    NONE,
    I,
    X
}

//clockwise starting from z plus axis
public enum Sides {
    Zplus,
    Xplus,
    Zminus,
    Xminus
}


