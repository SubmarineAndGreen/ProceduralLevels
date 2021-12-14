using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    SpawnerPlacer spawnerPlacer;
    NavigationManager navigationManager;
    EnemyManager enemyManager;
    [SerializeField] GameObject playerPrefab;

    private void Awake() {
        spawnerPlacer = GetComponent<SpawnerPlacer>();
    }
    void Start()
    {
        navigationManager = NavigationManager.instance;
        enemyManager = EnemyManager.instance;

        Vector3Int playerSpawnTile = enemyManager.getRandomValidSpawningTile();
        Vector3 playerSpawn = navigationManager.gridPositionToWorldPosition(playerSpawnTile);
        GameObject player = Instantiate(playerPrefab, playerSpawn, Quaternion.identity);
        enemyManager.playerTransform = player.transform;

        navigationManager.playerTransform = player.transform.GetChild(0);
        
        spawnerPlacer.enabled = true;
    }
}