using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class GameLoop : MonoBehaviour {
    [SerializeField] LevelBuilder levelBuilder;
    NavigationManager navigationManager;
    [SerializeField] GameObject playerPrefab;
    Transform playerTransform;
    [SerializeField] NavigationHintSpawner navigationHintSpawner;
    Vector3Int playerTile;
    Vector3Int previousPlayerTile;
    [SerializeField] GameObject goalPrefab;
    Vector3Int goalTile;
    int reachedGoalCounter = 0;
    int currentDifficulty = 0;
    [SerializeField] List<int> nextDifficultyGoalCount;
    [SerializeField] List<DifficultyData> difficultyData;
    DifficultyData currentDifficultyData;
    float remainingTime;
    bool timeElapsed;
    int distanceToGoal;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI addedTimeText;
    [SerializeField] TextMeshProUGUI distanceText;

    Sequence flashSequence;
    float flashTimeThreshold = 10f;

    void Start() {
        levelBuilder.generateLevel();
        navigationManager = NavigationManager.instance;

        currentDifficultyData = difficultyData[0];

        spawnPlayer();
        pickGoal();
        createNavigationHints();

        flashSequence = createTimeTextFlashSequence();
    }


    private void Update() {
        if(timeElapsed) {
            return;
        }
        Vector3Int currentTile = navigationManager.worldPositionToGridPosition(playerTransform.position);

        if (currentTile != playerTile) {
            previousPlayerTile = playerTile;
            playerTile = currentTile;
            updateDistanceToGoal();
            updateDistanceUIText();
            createNavigationHints();
        }

        updateRemainingTime();

        if(remainingTime < flashTimeThreshold) {
            flashSequence.Play();
        } else {
            if(flashSequence.IsPlaying()) {
                flashSequence.Rewind();
            }
        }

    }

    private void spawnPlayer() {
        Vector3Int playerSpawnTile = levelBuilder.generateStructures ? levelBuilder.playerSpawn : navigationManager.getRandomWalkableTile();
        // Debug.Log(playerSpawnTile);

        Vector3 playerSpawn = navigationManager.gridPositionToWorldPosition(playerSpawnTile);
        playerTile = playerSpawnTile;
        previousPlayerTile = playerSpawnTile;
        GameObject player = Instantiate(playerPrefab, playerSpawn, Quaternion.identity);
        playerTransform = player.transform.GetChild(0);

        navigationManager.playerTransform = playerTransform;
    }

    private void pickGoal() {
        int minDistance = currentDifficultyData.minDistance;
        int maxDistance = currentDifficultyData.maxDistance;

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
                    addRemainingTime(distance * currentDifficultyData.timePerDistance);
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
        goal.onGoalReached += goalReached;

        goalTile = result;
    }

    void createNavigationHints() {
        Vector3 heightOffset = Vector3.down * navigationManager.tileSize * 0.3f;
        List<Vector3> followNodes = new List<Vector3>();

        Vector3Int startingTile = previousPlayerTile;
        Vector3 playerPosition = navigationManager.gridPositionToWorldPosition(startingTile);
        followNodes.Add(playerPosition + heightOffset);

        while (startingTile != goalTile) {
            Vector3Int nextMove = Vector3Int.FloorToInt(navigationManager.getPathVector(goalTile, startingTile));
            if (nextMove == Vector3Int.zero) {
                break;
            } else {
                startingTile += nextMove;
                Vector3 nodePosition = navigationManager.gridPositionToWorldPosition(startingTile);
                followNodes.Add(nodePosition + heightOffset);
            }
        }

        navigationHintSpawner.replaceFollowNodes(followNodes);
    }

    void updateDifficulty() {
        reachedGoalCounter++;

        if (nextDifficultyGoalCount[currentDifficulty] == -1) {
            return;
        }

        if (reachedGoalCounter > nextDifficultyGoalCount[currentDifficulty]) {
            Debug.Log("next difficulty");
            reachedGoalCounter = 0;
            currentDifficulty++;
            currentDifficultyData = difficultyData[currentDifficulty];
        }
    }

    void goalReached() {
        reachedGoalCounter++;
        updateDifficulty();
        pickGoal();
        previousPlayerTile = playerTile;
        createNavigationHints();
        updateDistanceToGoal();
        updateDistanceUIText();
    }

    void updateRemainingTime() {
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0) {
            remainingTime = 0;
            onTimeElapsed();
        }

        updateTimeUI();

    }

    void addRemainingTime(float timeToAdd) {
        remainingTime += timeToAdd;
        showAddedTimeUIText(timeToAdd);
    }

    void onTimeElapsed() {
        timeElapsed = true;
        Debug.Log("time elapsed!");
    }

    void updateTimeUI() {
        timerText.text = formatUITime(remainingTime);
    }

    void showAddedTimeUIText(float addedTime) {
        // Debug.Log(addedTime);
        float tweenDuration = 1.5f;
        float originalHeight = addedTimeText.rectTransform.anchoredPosition.y;
        float targetHeight = addedTimeText.rectTransform.anchoredPosition.y - 128;

        addedTimeText.text = $"+{formatUITime(addedTime)}";

        Color textColor = addedTimeText.color;
        textColor.a = 1f;
        addedTimeText.color = textColor;

        DOTween.To(() => {
            Color textColor = addedTimeText.color;
            return textColor.a;
        }, newAlpha => {
            Color textColor = addedTimeText.color;
            textColor.a = newAlpha;
            addedTimeText.color = textColor;

        }, 0f, tweenDuration).SetEase(Ease.InQuad);

        var heightTween = DOTween.To(() => {
            return addedTimeText.rectTransform.anchoredPosition.y;
        }, height => {
            Vector2 positon = addedTimeText.rectTransform.anchoredPosition;
            positon.y = height;
            addedTimeText.rectTransform.anchoredPosition = positon;
        }, targetHeight, tweenDuration);

        heightTween.onComplete += () => {
            Vector2 positon = addedTimeText.rectTransform.anchoredPosition;
            positon.y = originalHeight;
            addedTimeText.rectTransform.anchoredPosition = positon;
        };
    }

    Sequence createTimeTextFlashSequence() {
        float flashDuration = 0.5f;

        Color startingColor = timerText.color;
        Color flashColor = Color.red;

        return DOTween.Sequence().Append(
            DOTween.To(() => timerText.color, color => timerText.color = color, flashColor, flashDuration).SetEase(Ease.InQuart)
        ).Append(
            DOTween.To(() => timerText.color, color => timerText.color = color, startingColor, flashDuration).SetEase(Ease.OutQuart)
        ).SetLoops(-1);
    }

    string formatUITime(float time) {

        float minutes = Mathf.Floor(time / 60);
        float seconds = (int)time % 60;
        return $"{minutes.ToString("00")}:{seconds.ToString("00")}";
    }

    void updateDistanceToGoal() {
        distanceToGoal = navigationManager.getGridDistance(playerTile, goalTile);
    }

    void updateDistanceUIText() {
        distanceText.text = $"distance: {distanceToGoal.ToString("00")}";
    }

    [System.Serializable]
    struct DifficultyData {
        public int minDistance;
        public int maxDistance;
        public float timePerDistance;
    }
}
