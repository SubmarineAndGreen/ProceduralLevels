using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructurePlaceholderTile : MonoBehaviour
{
    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, Vector3.one / 3);
    }
}
