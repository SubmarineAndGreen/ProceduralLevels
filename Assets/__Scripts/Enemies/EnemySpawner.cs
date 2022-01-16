using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    [SerializeField] List<GameObject> enemyPrefabs;
    NavigationManager navigationManager;

    private void Start() {
        navigationManager = NavigationManager.instance;
    }

    bool once = true;
    private void Update() {
        if (once) {
            Vector3Int spawn = navigationManager.getRandomWalkableTile();
            Vector3 worldsSpawn = navigationManager.gridPositionToWorldPosition(spawn);
            foreach (GameObject g in enemyPrefabs) {
                Instantiate(g, spawn, Quaternion.identity);
            }
            once = false;
        }
    }
}
