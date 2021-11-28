using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class NavigationVisuals : MonoBehaviour {
    public TileGrid grid;
    public int maxDistance = 20;
    public GameObject arrowPrefab;

    private void Update() {
        if (Keyboard.current.pKey.wasPressedThisFrame) {
            var tiles = grid.tileIndices;
            tiles.updateEach(value => 1);
            grid.rebuildGrid();
        }
    }

    public void updateDistanceFieldVisuals(int[,,] distanceField) {
        grid.tileIndices.forEach((x, y, z, value) => {
            if (distanceField[x, y, z] != Navigation.BLOCKED_CELL) {
                var node = grid.tileObjects.at(x, y, z).GetComponent<NavigationNode>();
                node.setColor(new Color(0, Mathf.Max((maxDistance - distanceField[x, y, z]) / (float)maxDistance, 0), 0));
                node.setText(distanceField[x, y, z].ToString());
            }
        });
    }

    public void updateVectorFieldVisuals(int[,,] vectorField) {
        grid.tileObjects.forEach((x, y, z, value) => {
            if(vectorField[x,y,z] != -1) {
                var arrow = Instantiate(arrowPrefab, value.transform.position, Quaternion.identity);
                arrow.transform.LookAt(arrow.transform.position + Navigation.neighbourOffset[vectorField[x,y,z]]);
            }
        });
    }
}
