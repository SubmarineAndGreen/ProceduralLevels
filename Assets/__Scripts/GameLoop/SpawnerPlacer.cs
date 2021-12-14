using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPlacer : MonoBehaviour {
    [SerializeField] GameObject spawnerPrefab;
    [SerializeField] int spawnerCount;
    EnemyManager enemyManager;
    NavigationManager navigationManager;
    HashSet<Vector3Int> spawnerTiles;
    void Start() {
        enemyManager = EnemyManager.instance;
        navigationManager = NavigationManager.instance;
        instantiateSpawnersInRandomTiles();
    }

    void instantiateSpawnersInRandomTiles() {
        int createdSpawners = 0;
        spawnerTiles = new HashSet<Vector3Int>();
        while (createdSpawners < spawnerCount) {
            Vector3Int validSpawnerTile = enemyManager.getRandomValidSpawningTile();
            if (!spawnerTiles.Contains(validSpawnerTile)) {
                spawnerTiles.Add(validSpawnerTile);
                Instantiate(spawnerPrefab,
                            navigationManager.gridPositionToWorldPosition(validSpawnerTile),
                            Quaternion.identity);
                createdSpawners++;
            }
        }
    }
}