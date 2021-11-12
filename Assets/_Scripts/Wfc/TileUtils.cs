using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class TileUtils {
    const int nOfRotations = 4;

    public static int tileIndexToModelIndex(int index, int rotation) {
        return index * nOfRotations + rotation;
    }

    public static int modelIndexToTileIndex(int index) {
        return (index - (index % nOfRotations)) / nOfRotations;
    }

    public static int modelIndexToRotation(int index) {
        return index % nOfRotations;
    }

}