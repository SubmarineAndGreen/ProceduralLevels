using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class TestFollowAI : MonoBehaviour {
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
        Debug.Log("start:" + currentTile + "goal:" + goalTile);
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = currentTile;
        List<Vector3> positions = new List<Vector3>();
        positions.Add(currentTile);
        while (true) {
            yield return new WaitUntil(() => Keyboard.current.nKey.wasPressedThisFrame);
            Debug.Log(sphere.transform.position);
            Vector3 nextMove = navigationManager.getPathVector(goalTile, currentTile);
            if (nextMove == Vector3.zero) {
                break;
            }
            sphere.transform.position += nextMove;
            currentTile += new Vector3Int((int)nextMove.x, (int)nextMove.y, (int)nextMove.z);
            positions.Add(currentTile);
            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
        }


    }
}
