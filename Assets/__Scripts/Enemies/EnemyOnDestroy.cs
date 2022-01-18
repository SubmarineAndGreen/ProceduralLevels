using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOnDestroy : MonoBehaviour
{
    [SerializeField] float playerRemainingTimeToAdd = 1f;
    [SerializeField] GameObject explosionPrefab;
    private void OnDestroy() {
        GameObject explosionVFX = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosionVFX, 4f);
        Game.instance.globalEnemyCount -= 1;
        Game.instance.addTime(playerRemainingTimeToAdd);
    }
}
