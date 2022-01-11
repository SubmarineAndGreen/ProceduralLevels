using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour {
    [SerializeField] LevelBuilder levelBuilder;
    SpawnerPlacer spawnerPlacer;
    NavigationManager navigationManager;
    [SerializeField] GameObject playerPrefab;
    Transform playerTransform;
    [SerializeField] NavigationHintSpawner navigationHintSpawner;
    Vector3Int playerTile;
    [SerializeField] GameObject goalPrefab;
    Vector3Int goalTile;

    // int difficultyModifier = 0;
    int startingMinDistance = 10, startingMaxDistance = 20;

    private void Awake() {
        spawnerPlacer = GetComponent<SpawnerPlacer>();
    }
    void Start() {
        levelBuilder.generateLevel();
        navigationManager = NavigationManager.instance;
        spawnerPlacer.enabled = true;
        spawnPlayer();
        pickGoal(startingMinDistance, startingMaxDistance);
        createNavigationHints();
    }


    private void Update() {
        Vector3Int currentTile = navigationManager.worldPositionToGridPosition(playerTransform.position);

        if (currentTile != playerTile) {
            playerTile = currentTile;
            createNavigationHints();

        }

    }

    private void spawnPlayer() {
        Vector3Int playerSpawnTile = levelBuilder.generateStructures ? levelBuilder.playerSpawn : navigationManager.getRandomWalkableTile();
        // Debug.Log(playerSpawnTile);

        Vector3 playerSpawn = navigationManager.gridPositionToWorldPosition(playerSpawnTile);
        playerTile = playerSpawnTile;
        GameObject player = Instantiate(playerPrefab, playerSpawn, Quaternion.identity);
        playerTransform = player.transform.GetChild(0);

        navigationManager.playerTransform = playerTransform;
    }

    private void pickGoal(int minDistance, int maxDistance) {
        const int tries = 10;
        bool foundGoalTile = false;
        Vector3Int result = Vector3Int.zero;

        while (!foundGoalTile) {
            int currentTries = tries;
            while (currentTries > 0) {
                currentTries--;
                Vector3Int randomGoalTile = navigationManager.getRandomDoorTile();

                float distance = navigationManager.getGridDistanceToPlayer(randomGoalTile);

                if (distance >= minDistance && distance <= maxDistance) {
                    // Debug.Log(distance);
                    result = randomGoalTile;
                    foundGoalTile = true;
                    break;
                }
            }

            if (!foundGoalTile) {
                if (minDistance > 2) {
                    minDistance--;
                }
                maxDistance++;
            }
        }

        GameObject goalObject = Instantiate(goalPrefab, navigationManager.gridPositionToWorldPosition(result), Quaternion.identity, levelBuilder.levelGrid.transform);
        Goal goal = goalObject.GetComponentInChildren<Goal>();
        goal.onGoalReached += onGoalReached;

        goalTile = result;
    }

    void createNavigationHints() {
        Vector3 heightOffset = Vector3.down * navigationManager.tileSize * 0.3f;
        List<Vector3> followNodes = new List<Vector3>();

        Vector3Int currentTile = navigationManager.worldPositionToGridPosition(playerTransform.position);
        Vector3 playerPosition = navigationManager.gridPositionToWorldPosition(currentTile);
        followNodes.Add(playerPosition + heightOffset);

        while (currentTile != goalTile) {
            Vector3Int nextMove = Vector3Int.FloorToInt(navigationManager.getPathVector(goalTile, currentTile));
            if (nextMove == Vector3Int.zero) {
                break;
            } else {
                currentTile += nextMove;
                Vector3 nodePosition = navigationManager.gridPositionToWorldPosition(currentTile);
                followNodes.Add(nodePosition + heightOffset);
            }
        }

        navigationHintSpawner.replaceFollowNodes(followNodes);
    }

    void onGoalReached() {
        pickGoal(startingMinDistance, startingMaxDistance);
        createNavigationHints();
    }
}
