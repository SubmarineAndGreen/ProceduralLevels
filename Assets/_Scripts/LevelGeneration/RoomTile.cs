using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTile : MonoBehaviour {
    [SerializeField] GameObject wall, wallOpen;

    // private void Start() {
    //     instantiateWall(Directions3D.UP, WallType.OPEN);
    //     instantiateWall(Directions3D.RIGHT, WallType.OPEN);
    //     instantiateWall(Directions3D.LEFT, WallType.OPEN);
    //     instantiateWall(Directions3D.DOWN, WallType.OPEN);
    //     instantiateWall(Directions3D.FORWARD, WallType.OPEN);
    //     instantiateWall(Directions3D.BACK, WallType.OPEN);
    // }

    private void OnDrawGizmos() {
        Gizmos.DrawCube(transform.position, Vector3.one / 2);
    }

    public void instantiateWall(Directions3D direction, WallType wallType) {
        GameObject objectToInstantiate = null;
        switch (wallType) {
            case WallType.SOLID:
                objectToInstantiate = wall;
                break;
            case WallType.OPEN:
                objectToInstantiate = wallOpen;
                break;
        }
        GameObject wallObj = Instantiate(objectToInstantiate,
                                   this.transform.position + DirectionUtils.DirectionsToVectors[direction].toVector3() / 2,
                                   Quaternion.identity);
        wallObj.transform.LookAt(this.transform);
        wallObj.transform.SetParent(this.transform);
    }

    public enum WallType {
        SOLID,
        OPEN
    }
}
