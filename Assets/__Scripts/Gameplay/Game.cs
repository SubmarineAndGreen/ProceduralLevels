using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class Game : MonoBehaviour {

    public static Game instance;

    [SerializeField] LevelBuilder levelBuilder;
    NavigationManager navigationManager;
    [SerializeField] GameObject playerPrefab;
    [HideInInspector] public GameObject playerObject;
    Transform playerTransform;
    [SerializeField] NavigationHintSpawner navigationHintSpawner;
    Vector3Int playerTile;
    Vector3Int previousPlayerTile;
    [SerializeField] GameObject goalPrefab;
    Vector3Int goalTile;
    int reachedGoalCounter = 0;
    int totalReachedGoalCounter = 0;
    int currentDifficulty = 0;
    [SerializeField] List<int> nextDifficultyGoalCount;
    [SerializeField] List<CommonDifficultyData> difficultyData;
    CommonDifficultyData currentDifficultyData;
    static float remainingTime;
    bool timeElapsed;
    int distanceToGoal;

    [HideInInspector] public int globalEnemyCount;
    [HideInInspector] public int enemyCount;
    [SerializeField] List<GameObject> enemyPrefabs;
    [SerializeField] List<GameObject> bulletPrefabs;
    [SerializeField] float enemySpawnCooldown = 10f;
    [SerializeField] float minEnemySpawnDistance, maxEnemySpawnDistance;
    Timer enemySpawnTimer;

    [Header("UI")]
    [SerializeField] Canvas gameUI;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI addedTimeText;
    [SerializeField] TextMeshProUGUI distanceText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] List<Image> difficultyStarImages;

    [SerializeField] Sprite starFilled, starBorder;

    Sequence flashSequence;
    float flashTimeThreshold = 10f;

    private void Awake() {
        instance = this;
    }

    void Start() {
        remainingTime = 20f;

        updateScoreUIText();
        updateDifficultyUI();

        levelBuilder.generateLevel();
        navigationManager = NavigationManager.instance;

        currentDifficultyData = difficultyData[0];

        flashSequence = createTimeTextFlashSequence();

        spawnPlayer();
        pickGoal();
        updateDistanceToGoal();
        createNavigationHints();

        enemySpawnTimer = TimerManager.getInstance().CreateAndRegisterTimer(enemySpawnCooldown, true, true, spawnEnemies);
    }


    private void Update() {
        if (timeElapsed) {
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

        if (remainingTime < flashTimeThreshold) {
            flashSequence.Play();
        } else {
            if (flashSequence.IsPlaying()) {
                flashSequence.Rewind();
            }
        }

    }

    void spawnEnemies() {
        int distanceToPlayer = -1;
        Vector3Int randomSpawnTile = Vector3Int.zero;

        while (!(distanceToPlayer >= minEnemySpawnDistance && distanceToPlayer <= maxEnemySpawnDistance)) {
            randomSpawnTile = navigationManager.getRandomWalkableTile();
            distanceToPlayer = navigationManager.getGridDistanceToPlayer(randomSpawnTile);
        }

        // Debug.Log("enemy:" + randomSpawnTile);
        // Debug.Log(navigationManager.worldPositionToGridPosition(navigationManager.playerTransform.position));

        Vector3 spawnPosition = navigationManager.gridPositionToWorldPosition(randomSpawnTile);
        int spawnedEnemies = 0;
        while (spawnedEnemies < currentDifficultyData.maxEnemySpawnGroup && globalEnemyCount < currentDifficultyData.maxEnemyCount) {
            spawnedEnemies++;
            globalEnemyCount++;

            float chance = UnityEngine.Random.Range(0f, 100f);
            int enemyIndex = 0;
            chance -= currentDifficultyData.enemySpawnChances[0];
            // Debug.Log("1:" + chance);
            while (chance >= 0) {
                enemyIndex++;
                // Debug.Log("2:" + currentDifficultyData.enemySpawnChances[enemyIndex]);
                // Debug.Log("3:" + chance);
                chance -= currentDifficultyData.enemySpawnChances[enemyIndex];
            }

            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * (navigationManager.tileSize / 2);
            Instantiate(enemyPrefabs[enemyIndex], spawnPosition + randomOffset, Quaternion.identity);
        }
    }

    private void spawnPlayer() {
        Vector3Int playerSpawnTile = levelBuilder.generateStructures ? levelBuilder.playerSpawn : navigationManager.getRandomWalkableTile();
        // Debug.Log(playerSpawnTile);

        Vector3 playerSpawn = navigationManager.gridPositionToWorldPosition(playerSpawnTile);
        playerTile = playerSpawnTile;
        previousPlayerTile = playerSpawnTile;
        GameObject playerObject = Instantiate(playerPrefab, playerSpawn, Quaternion.identity);
        playerTransform = playerObject.transform.GetChild(0);

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
        updateDifficultyUI();

        if (nextDifficultyGoalCount[currentDifficulty] == -1) {
            return;
        }

        if (reachedGoalCounter > nextDifficultyGoalCount[currentDifficulty]) {
            // Debug.Log("next difficulty");
            reachedGoalCounter = 0;
            currentDifficulty++;
            currentDifficultyData = difficultyData[currentDifficulty];

            updateEnemyPrefabs(currentDifficulty);
        }
    }

    void updateDifficultyUI() {
        for (int i = 0; i < difficultyStarImages.Count; i++) {
            difficultyStarImages[i].sprite = currentDifficulty >= i ? starFilled : starBorder;
        }
    }

    void updateEnemyPrefabs(float currentDifficulty) {
        if (currentDifficulty > 1) {
            CommonEnemy commonEnemy = enemyPrefabs[0].GetComponent<CommonEnemy>();
            CommonEnemy homingEnemy = enemyPrefabs[1].GetComponent<CommonEnemy>();
            SniperEnemy sniperEnemy = enemyPrefabs[2].GetComponent<SniperEnemy>();
            CommonEnemy mineEnemy = enemyPrefabs[3].GetComponent<CommonEnemy>();

            SimpleBullet commonBullet = bulletPrefabs[0].GetComponent<SimpleBullet>();
            HomingBullet homingBullet = bulletPrefabs[1].GetComponent<HomingBullet>();

            switch (currentDifficulty) {
                case 2:
                    commonEnemy.bulletsInBarrage = 12;
                    homingEnemy.bulletsInBarrage = 12;
                    sniperEnemy.maxMultiBulletCount = 6;
                    mineEnemy.bulletsInBarrage = 2;
                    break;
                case 3:
                    commonEnemy.barrageCooldown = 1;
                    homingEnemy.barrageCooldown = 1;
                    break;
                case 4:
                    mineEnemy.bulletsInBarrage = 3;
                    commonBullet.velocity = 70;
                    homingBullet.velocity = 60;
                    break;
            }
        }
    }

    void goalReached() {
        reachedGoalCounter++;
        totalReachedGoalCounter++;
        updateScoreUIText();
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
        TextMeshProUGUI addedTimeTextCopy = Instantiate(addedTimeText.gameObject, gameUI.transform).GetComponent<TextMeshProUGUI>();

        // Debug.Log(addedTime);
        float tweenDuration = 1.5f;
        float originalHeight = addedTimeTextCopy.rectTransform.anchoredPosition.y;
        float targetHeight = addedTimeTextCopy.rectTransform.anchoredPosition.y - 128;

        addedTimeTextCopy.text = $"+{formatUITime(addedTime)}";

        Color textColor = addedTimeTextCopy.color;
        textColor.a = 1f;
        addedTimeTextCopy.color = textColor;

        DOTween.To(() => {
            Color textColor = addedTimeTextCopy.color;
            return textColor.a;
        }, newAlpha => {
            Color textColor = addedTimeTextCopy.color;
            textColor.a = newAlpha;
            addedTimeTextCopy.color = textColor;

        }, 0f, tweenDuration).SetEase(Ease.InQuad);

        var heightTween = DOTween.To(() => {
            return addedTimeTextCopy.rectTransform.anchoredPosition.y;
        }, height => {
            Vector2 positon = addedTimeTextCopy.rectTransform.anchoredPosition;
            positon.y = height;
            addedTimeTextCopy.rectTransform.anchoredPosition = positon;
        }, targetHeight, tweenDuration);

        heightTween.onComplete += () => {
            Destroy(addedTimeTextCopy.gameObject);
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

    void updateScoreUIText() {
        scoreText.text = $"score: {totalReachedGoalCounter}";
    }

    [System.Serializable]
    class CommonDifficultyData {
        public int minDistance;
        public int maxDistance;
        public float timePerDistance;
        public int maxEnemyCount;
        public float[] enemySpawnChances;
        public float maxEnemySpawnGroup;
    }

    public void addTime(float time) {
        remainingTime += time;
        showAddedTimeUIText(time);
    }
}
