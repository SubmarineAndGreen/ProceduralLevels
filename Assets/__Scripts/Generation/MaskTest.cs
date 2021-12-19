using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskTest : MonoBehaviour
{
    StructureTile testTile = new StructureTile();
    
    private void Start() {
        testTile.setSideOpen(Directions3D.UP, true);
        testTile.setSideOpen(Directions3D.FORWARD, true);
        testTile.setSideOpen(Directions3D.UP, false);
        testTile.setSideOpen(Directions3D.LEFT, true);

        Debug.Log(testTile.openSidesMask);
        Debug.Log(testTile.isSideOpen(Directions3D.UP));
        Debug.Log(testTile.isSideOpen(Directions3D.FORWARD));
    }
}
