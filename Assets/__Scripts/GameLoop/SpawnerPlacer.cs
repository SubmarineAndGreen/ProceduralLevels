using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPlacer : MonoBehaviour {
    [SerializeField] GameObject spawnerPrefab;
    [SerializeField] int spawnerCount;
    [SerializeField] float enemySpawningPeriod;
    NavigationManager navigationManager;
    HashSet<Vector3Int> spawnerTiles;
    void Start() {
        navigationManager = NavigationManager.instance;
        instantiateSpawnersInRandomTiles();
    }

    void instantiateSpawnersInRandomTiles() {
        int createdSpawners = 0;
        int tries = 100;
        spawnerTiles = new HashSet<Vector3Int>();
        while (createdSpawners < spawnerCount && tries-- > 0) {
            Vector3Int validSpawnerTile = navigationManager.getRandomWalkableTile();
            if (!spawnerTiles.Contains(validSpawnerTile)) {
                spawnerTiles.Add(validSpawnerTile);
                GameObject spawnerObject = Instantiate(spawnerPrefab,
                                                       navigationManager.gridPositionToWorldPosition(validSpawnerTile),
                                                       Quaternion.identity);
                EnemySpawner spawner = spawnerObject.GetComponent<EnemySpawner>();
                spawner.setSpawningPeriod(enemySpawningPeriod); 
                createdSpawners++;
            }
        }
    }
}
