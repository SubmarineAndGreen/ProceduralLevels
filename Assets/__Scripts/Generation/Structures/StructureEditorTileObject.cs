using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureEditorTileObject : MonoBehaviour {
    public StructureTile structureTile;
    [SerializeField] GameObject openSideMarker, unwalkableMarker, spawningMarker, noConstraintsMarker;
    List<GameObject> markers;

    private void Awake() {
        markers = new List<GameObject>();
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
        if (!structureTile.walkable) {
            GameObject marker = Instantiate(unwalkableMarker,
                                        transform.position + Vector3.up / 2,
                                        Quaternion.identity);
            markers.Add(marker);
            marker.transform.SetParent(this.transform);
        }
        if (structureTile.excludeFromSpawning) {
            GameObject marker = Instantiate(spawningMarker,
                                        transform.position + Vector3.up / 2,
                                        Quaternion.identity);
            markers.Add(marker);
            marker.transform.SetParent(this.transform);
        }
        if (structureTile.noConstraints) {
            GameObject marker = Instantiate(noConstraintsMarker,
                                        transform.position + Vector3.up / 2,
                                        Quaternion.identity);
            markers.Add(marker);
            marker.transform.SetParent(this.transform);
        }

    }
}
