using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureTileObject : MonoBehaviour {
    public StructureTile structureTile;
    [SerializeField] GameObject openSideMarker, unwalkableMarker;
    List<GameObject> markers;

    private void Awake() {
        structureTile = new StructureTile();
        markers = new List<GameObject>();
    }
    
    private void Start() {
        // structureTile.setSideOpen(Directions3D.DOWN, true);
        // structureTile.setSideOpen(Directions3D.RIGHT, true);
        // instantiateMarkers();
    }

    public void instantiateMarkers() {
        foreach (GameObject marker in markers) {
            Destroy(marker);
        }

        markers = new List<GameObject>();

        foreach (Directions3D direction in DirectionUtils.allDirections) {
            if (structureTile.isSideOpen(direction)) {
                GameObject marker = Instantiate(openSideMarker,
                                        transform.position + Vector3.up / 2 + DirectionUtils.DirectionsToVectors[direction].toVector3() / 2,
                                        Quaternion.identity);
                markers.Add(marker);
                marker.transform.SetParent(this.transform);
            }
        }
        if(!structureTile.walkable) {
            GameObject marker = Instantiate(unwalkableMarker,
                                        transform.position + Vector3.up / 2,
                                        Quaternion.identity);
                markers.Add(marker);
                marker.transform.SetParent(this.transform);
        }

    }
}
