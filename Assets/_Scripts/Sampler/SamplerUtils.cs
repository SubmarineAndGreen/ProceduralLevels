using System.Collections.Generic;
using UnityEngine;

public class SamplerUtils {
    public static bool isInBounds(Vector3Int dimensions, Vector3Int position) {
        if (position.x >= dimensions.x || position.y >= dimensions.y || position.z >= dimensions.z) {
            return false;
        }
        if (position.x < 0 || position.y < 0 || position.z < 0) {
            return false;
        }
        return true;
    }




    public const int nOfDirections = 6;
    public static Directions3D[] allDirections = {
        Directions3D.UP,
        Directions3D.DOWN,
        Directions3D.FORWARD,
        Directions3D.RIGHT,
        Directions3D.BACK,
        Directions3D.LEFT
    };

    // public static Vector3Int[] directionsToVectors = new Vector3Int[] {
    //     Vector3Int.up,
    //     Vector3Int.down,
    //     Vector3Int.forward,
    //     Vector3Int.right,
    //     Vector3Int.back,
    //     Vector3Int.left
    // };

    // public static Vector3Int[] oppositeDirections = new Vector3Int[] {

    // };

    public static Dictionary<Directions3D, Vector3Int> DirectionsToVectors = new Dictionary<Directions3D, Vector3Int>() {
        {Directions3D.UP, Vector3Int.up},
        {Directions3D.DOWN, Vector3Int.down},
        {Directions3D.FORWARD, Vector3Int.forward},
        {Directions3D.RIGHT, Vector3Int.right},
        {Directions3D.BACK, Vector3Int.back},
        {Directions3D.LEFT, Vector3Int.left}
    };

    public static Dictionary<Directions3D, Directions3D> oppositeDirections = new Dictionary<Directions3D, Directions3D>() {
        {Directions3D.UP, Directions3D.DOWN},
        {Directions3D.DOWN, Directions3D.UP},
        {Directions3D.FORWARD, Directions3D.BACK},
        {Directions3D.RIGHT, Directions3D.LEFT},
        {Directions3D.BACK, Directions3D.FORWARD},
        {Directions3D.LEFT, Directions3D.RIGHT}
    };

    public static Dictionary<Directions3D, Directions3D> nextDirectionClockwise = new Dictionary<Directions3D, Directions3D>() {
        {Directions3D.FORWARD, Directions3D.RIGHT},
        {Directions3D.RIGHT, Directions3D.BACK},
        {Directions3D.BACK, Directions3D.LEFT},
        {Directions3D.LEFT, Directions3D.FORWARD},
        // {Directions3D.UP, Directions3D.UP},
        // {Directions3D.DOWN, Directions3D.DOWN}
    };
}