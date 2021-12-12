
using UnityEngine;

public static class VectorExtensions {
    public static Vector3 toVector3(this Vector3Int vec) {
        return new Vector3(vec.x, vec.y, vec.z);
    }
}