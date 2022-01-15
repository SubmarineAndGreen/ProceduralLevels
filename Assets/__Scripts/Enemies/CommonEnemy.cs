using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonEnemy : MonoBehaviour
{
    public Transform target;
    Timer barrageCooldownTimer;
    Timer barrageEachBulletCooldownTimer;

    [SerializeField] float barrageCooldown = 3, barrageEachBulletCooldown = 0.2f;
    bool barrageReady = true;
    [SerializeField] int bulletsInBarrage = 5;
    int spawnedBulletCount = 0;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletStartingOffset = 1f;

    private void Update() {
        if(barrageReady) {
            startBulletBarrage();
        }
    }

    private void Awake() {
        barrageCooldownTimer = TimerManager.getInstance().CreateAndRegisterTimer(barrageCooldown, false, false, () => barrageReady = true);
        barrageEachBulletCooldownTimer = TimerManager.getInstance().CreateAndRegisterTimer(barrageEachBulletCooldown, true, false, spawnBarrageBullet);
    }

    void startBulletBarrage() {
        barrageReady = false;
        spawnedBulletCount = 0;
        barrageEachBulletCooldownTimer.run();
        // Debug.Log("barrage start");
    }

    void spawnBarrageBullet() {
        if(spawnedBulletCount == bulletsInBarrage) {
            barrageEachBulletCooldownTimer.resetTime();
            barrageEachBulletCooldownTimer.stop();

            barrageCooldownTimer.resetTime();
            barrageCooldownTimer.run();
            return;
        }
        spawnedBulletCount += 1;
        // Debug.Log("spawned bullets: " + spawnedBulletCount);
        Vector3 bulletOffset = (target.position - this.transform.position).normalized * bulletStartingOffset;
        GameObject bulletObject = Instantiate(bulletPrefab, this.transform.position + bulletOffset, Quaternion.identity);
        IBullet bullet = bulletObject.GetComponentInChildren<IBullet>();
        bullet.setTarget(target);
    }
}
