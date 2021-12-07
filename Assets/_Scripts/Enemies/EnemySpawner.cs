using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public GameObject enemyPrefab;
    public float spawningPeriod;
    public float minSpawningDistance;
    public float maxSpawningDistance;

    private void Start() {
        TimerManager.getInstance().CreateAndRegisterTimer(spawningPeriod, true, true, spawnEnemy);
    }

    private void spawnEnemy() {
        Vector3 randomVector = UnityEngine.Random.insideUnitSphere;
        Vector3 spawnOffset = minSpawningDistance * randomVector.normalized + randomVector * (maxSpawningDistance - minSpawningDistance); 
        Instantiate(enemyPrefab, transform.position + spawnOffset, Quaternion.identity);
    }
}