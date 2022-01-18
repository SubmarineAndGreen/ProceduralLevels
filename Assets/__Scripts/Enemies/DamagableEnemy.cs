using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableEnemy : MonoBehaviour {
    Damagable damagable;

    [SerializeField] float playerRemainingTimeToAdd = 1f;
    [SerializeField] GameObject explosionPrefab;

    private void Awake() {
        damagable = GetComponent<Damagable>();
        damagable.afterTakeDamage += () => {
            if (damagable.health <= 0) {
                Destroy(this.gameObject);
            }
        };
    }
    private void OnDestroy() {
        GameObject explosionVFX = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosionVFX, 4f);
        Game.instance.globalEnemyCount -= 1;
        Game.instance.addTime(playerRemainingTimeToAdd);
    }


}
