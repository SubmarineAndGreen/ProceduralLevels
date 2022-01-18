using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonEnemy : MonoBehaviour
{
    [HideInInspector] public Transform target;
    [SerializeField] NavigationAI navigationAI;
    NavigationManager navigationManager;
    Timer barrageCooldownTimer;
    Timer barrageEachBulletCooldownTimer;

    public float barrageCooldown = 3, barrageEachBulletCooldown = 0.2f;
    bool barrageReady = true;
    public int bulletsInBarrage = 5;
    int spawnedBulletCount = 0;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletStartingOffset = 1f;
    private void Awake() {
        barrageCooldownTimer = TimerManager.getInstance().CreateAndRegisterTimer(barrageCooldown, false, false, () => barrageReady = true);
        barrageEachBulletCooldownTimer = TimerManager.getInstance().CreateAndRegisterTimer(barrageEachBulletCooldown, true, false, spawnBarrageBullet);
    }

    private void Start() {
        navigationManager = NavigationManager.instance;
        target = navigationManager.playerTransform;
    }

    private void Update() {
        if(barrageReady && navigationAI.playerInSight) {
            startBulletBarrage();
        }
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

    private void OnDestroy() {
        barrageCooldownTimer.stop();
        barrageEachBulletCooldownTimer.stop();
    }
}
