using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class PathDrawer : MonoBehaviour {
    EnemyManager enemyManager;
    NavigationManager navigationManager;
    LineRenderer lineRenderer;

    void Start() {
        enemyManager = EnemyManager.instance;
        navigationManager = NavigationManager.instance;
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update() {
        if (Keyboard.current.mKey.wasPressedThisFrame) {
            StartCoroutine("startStepToGoal");
        }
    }

    private IEnumerator startStepToGoal() {
        Vector3Int currentTile = enemyManager.getRandomValidSpawningTile();
        Vector3Int goalTile = enemyManager.getRandomValidSpawningTile();
        Vector3 offset = new Vector3(-0.5f, 0, -0.5f);
        Debug.Log("start:" + currentTile + "goal:" + goalTile);
        // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // sphere.transform.position = offset + currentTile;
        List<Vector3> positions = new List<Vector3>();
        positions.Add(offset + currentTile);
        while (true) {
            yield return new WaitUntil(() => Keyboard.current.nKey.wasPressedThisFrame);
            // Debug.Log(sphere.transform.position);
            Vector3 nextMove = navigationManager.getPathVector(goalTile, currentTile);
            if (nextMove == Vector3.zero) {
                break;
            }
            // sphere.transform.position += nextMove;
            currentTile += new Vector3Int((int)nextMove.x, (int)nextMove.y, (int)nextMove.z);
            positions.Add(offset + currentTile);
            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
        }


    }
}
