using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SniperEnemy : MonoBehaviour {
    [SerializeField] Color laserStartingColor, laserEndColor;
    [HideInInspector] public Transform target;
    [SerializeField] NavigationAI navigationAI;
    NavigationManager navigationManager;

    [SerializeField] float shotCooldown, chargeTime, multiBulletCooldown;
    [SerializeField] int maxMultiBulletCount = 3;
    int currentBulletCount = 0;
    Timer shotCooldownTimer;
    Timer chargeTimer;
    Timer multiBulletTimer;
    bool charging;
    bool shotReady = true;
    LineRenderer laserLineRenderer;
    Tween laserTween;
    float bulletStartingOffset = 2f;
    [SerializeField] GameObject bulletPrefab;

    private void Awake() {
        shotCooldownTimer = TimerManager.getInstance().CreateAndRegisterTimer(shotCooldown, false, false, () => shotReady = true);
        chargeTimer = TimerManager.getInstance().CreateAndRegisterTimer(chargeTime, false, false, chargeFinished);
        multiBulletTimer = TimerManager.getInstance().CreateAndRegisterTimer(multiBulletCooldown, true, false, spawnBullet);
        laserLineRenderer = GetComponent<LineRenderer>();
        laserLineRenderer.sharedMaterial.SetColor("_BaseColor", laserStartingColor);
    }

    private void Start() {
        navigationManager = NavigationManager.instance;
        target = navigationManager.playerTransform;
    }

    private void Update() {
        if (shotReady && navigationAI.playerInSight) {
            startCharge();
        }

        if (charging) {
            if (navigationAI.playerInSight) {
                laserLineRenderer.SetPosition(0, transform.position);
                laserLineRenderer.SetPosition(1, target.position);
            } else {
                stopCharge();
                chargeTimer.stop();
                chargeTimer.resetTime();
                shotReady = true;
            }
        }
    }



    void startCharge() {
        // Debug.Log("charge start");
        shotReady = false;

        chargeTimer.resetTime();
        chargeTimer.run();

        charging = true;

        laserLineRenderer.positionCount = 2;
        laserLineRenderer.SetPosition(0, transform.position);
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        laserTween = DOTween.To(
            () => {
                return laserLineRenderer.material.GetColor("_BaseColor");
            },
            color => {
                laserLineRenderer.GetPropertyBlock(propertyBlock, 0);
                propertyBlock.SetColor("_BaseColor", color);
                laserLineRenderer.SetPropertyBlock(propertyBlock, 0);
            },
            laserEndColor, chargeTime
        ).SetEase(Ease.InCubic);
    }

    void stopCharge() {
        charging = false;
        laserLineRenderer.positionCount = 0;

        if (laserTween.IsActive()) {
            laserTween.Rewind();
        } else {
            laserLineRenderer.sharedMaterial.SetColor("_BaseColor", laserStartingColor);
        }
    }

    void chargeFinished() {
        // Debug.Log("charge finished");
        stopCharge();
        multiBulletTimer.resetTime();
        multiBulletTimer.run();
    }

    void spawnBullet() {
        if (currentBulletCount == maxMultiBulletCount) {
            currentBulletCount = 0;
            multiBulletTimer.stop();
            shotCooldownTimer.resetTime();
            shotCooldownTimer.run();
        }

        Vector3 bulletOffset = target.position - this.transform.position;
        bulletOffset = bulletOffset.normalized * bulletStartingOffset;
        GameObject bulletObject = Instantiate(bulletPrefab, transform.position + bulletOffset, Quaternion.identity);
        IBullet bullet = bulletObject.GetComponent<IBullet>();
        bullet.setTarget(target);
        currentBulletCount++;
    }
}
