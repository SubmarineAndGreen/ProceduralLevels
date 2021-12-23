using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class NavigationVisuals : MonoBehaviour {
    public TileGrid grid;
    public LevelBuilder levelBuilder;
    public int maxDistance = 20;
    public GameObject arrowPrefab;
    [HideInInspector] public List<GameObject> arrows;
    public Vector3Int goalTile;
    public NavigationManager navigationManager;
    
    private bool showToggle = true;

    private void Start() {
        navigationManager = NavigationManager.instance;
    }

    private void Update() {
        // if (Keyboard.current.rKey.wasPressedThisFrame) {
        //     var tiles = grid.tileIndices;
        //     tiles.updateEach(value => 1);
        //     grid.rebuildGrid();
        // }

        if (Keyboard.current.tKey.wasPressedThisFrame) {
            // grid.resizePerserveTiles(levelBuilder.fullDimensions);
            updateDistanceFieldVisuals(navigationManager.distanceFields[goalTile.x, goalTile.y, goalTile.z]);
            updateVectorFieldVisuals(navigationManager.vectorFields[goalTile.x, goalTile.y, goalTile.z]);
            grid.transform.localScale = new Vector3(levelBuilder.scaleFactor,levelBuilder.scaleFactor,levelBuilder.scaleFactor);
            // grid.transform.position += new Vector3(levelBuilder.scaleFactor,levelBuilder.scaleFactor,levelBuilder.scaleFactor);
        }

        if (Keyboard.current.yKey.wasPressedThisFrame) {
            showToggle = !showToggle; 
            showDistanceField(navigationManager.distanceFields[goalTile.x, goalTile.y, goalTile.z], showToggle);
        }
    }

    public void updateDistanceFieldVisuals(int[,,] distanceField) {
        // showDistanceField(distanceField, true);
        grid.tileIndices.updateEach((x, y, z, value) => {
            return 1;
        });

        grid.rebuildGrid();
        grid.tileIndices.forEach((x, y, z, value) => {
            if (distanceField[x, y, z] != NavigationTest.BLOCKED_CELL) {
                var node = grid.tileObjects.at(x, y, z).GetComponent<NavigationVisualisationNode>();
                node.setColor(new Color(0, Mathf.Max((maxDistance - distanceField[x, y, z]) / (float)maxDistance, 0), 0));
                node.setText(distanceField[x, y, z].ToString());
            }
        });
    }

    public void showDistanceField(int[,,] distanceField, bool toggle) {
        grid.tileIndices.forEach((x, y, z, value) => {
            if (distanceField[x, y, z] != NavigationTest.BLOCKED_CELL) {
                var node = grid.tileObjects.at(x, y, z);
                node.SetActive(toggle);
            }
        });
    }

    public void updateVectorFieldVisuals(int[,,] vectorField) {
        arrows.ForEach(arrow => Destroy(arrow));
        arrows = new List<GameObject>();
        grid.tileObjects.forEach((x, y, z, value) => {
            if (vectorField[x, y, z] != -1) {
                var arrow = Instantiate(arrowPrefab, value.transform.position, Quaternion.identity);
                arrow.transform.LookAt(arrow.transform.position + NavigationTest.directionVectors[vectorField[x, y, z]]);
                arrow.transform.SetParent(this.transform);
                arrows.Add(arrow);
            }
        });
    }
}
