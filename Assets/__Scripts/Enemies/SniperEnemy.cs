using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SniperEnemy : MonoBehaviour {
    [SerializeField] Color laserStartingColor, laserEndColor;
    [HideInInspector] public Transform target;
    [SerializeField] NavigationAI navigationAI;
    [SerializeField] float shotCooldown, chargeTime;
    Timer shotCooldownTimer;
    Timer chargeTimer;
    bool charging;
    bool shotReady = true;
    LineRenderer laserLineRenderer;
    Tween laserTween;
    float bulletStartingOffset = 1f;
    [SerializeField] GameObject bulletPrefab;

    private void Update() {
        if (shotReady && navigationAI.playerInSight) {
            startCharge();
        }

        if (charging) {
            if (navigationAI.playerInSight) {
                laserLineRenderer.SetPosition(1, target.position);
            } else {
                stopCharge();
                chargeTimer.stop();
                chargeTimer.resetTime();
                shotReady = true;
            }
        }
    }

    private void Awake() {
        shotCooldownTimer = TimerManager.getInstance().CreateAndRegisterTimer(shotCooldown, false, false, () => shotReady = true);
        chargeTimer = TimerManager.getInstance().CreateAndRegisterTimer(chargeTime, false, false, chargeFinished);
        laserLineRenderer = GetComponent<LineRenderer>();
        laserLineRenderer.sharedMaterial.SetColor("_BaseColor", laserStartingColor);
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
                // laserLineRenderer.material.SetColor("_BaseColor", color);
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
        shotCooldownTimer.resetTime();
        shotCooldownTimer.run();

        Vector3 bulletOffset = target.position - this.transform.position;
        bulletOffset = bulletOffset.normalized * bulletStartingOffset;
        GameObject bulletObject = Instantiate(bulletPrefab, transform.position + bulletOffset, Quaternion.identity);
        IBullet bullet = bulletObject.GetComponent<IBullet>();
        bullet.setTarget(target);
    }
}
