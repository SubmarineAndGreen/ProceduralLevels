using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    [SerializeField] LevelBuilder levelBuilder;
    SpawnerPlacer spawnerPlacer;
    NavigationManager navigationManager;
    EnemyManager enemyManager;
    [SerializeField] GameObject playerPrefab;

    private void Awake() {
        spawnerPlacer = GetComponent<SpawnerPlacer>();
    }
    void Start()
    {
        levelBuilder.generate();

        navigationManager = NavigationManager.instance;
        enemyManager = EnemyManager.instance;

        Vector3Int playerSpawnTile = enemyManager.getRandomValidSpawningTile();
        Vector3 playerSpawn = navigationManager.gridPositionToWorldPosition(playerSpawnTile);
        GameObject player = Instantiate(playerPrefab, playerSpawn, Quaternion.identity);

        navigationManager.playerTransform = player.transform.GetChild(0);
        enemyManager.playerTransform = player.transform.GetChild(0);
        
        spawnerPlacer.enabled = true;
    }
}
