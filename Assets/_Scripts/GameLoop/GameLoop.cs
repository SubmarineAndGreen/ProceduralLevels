using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    SpawnerPlacer spawnerPlacer;
    NavigationManager navigationManager;
    EnemyManager enemyManager;

    private void Awake() {
        spawnerPlacer = GetComponent<SpawnerPlacer>();
    }
    void Start()
    {
        navigationManager = NavigationManager.instance;
        enemyManager = EnemyManager.instance;

        GameObject dummyPlayer = new GameObject("dummyPlayer");
        Vector3Int playerTile = enemyManager.getRandomValidSpawningTile();
        dummyPlayer.transform.position = navigationManager.gridPositionToWorldPosition(playerTile);
        Debug.Log(playerTile);

        navigationManager.playerTransform = dummyPlayer.transform;
        
        spawnerPlacer.enabled = true;
    }
}
