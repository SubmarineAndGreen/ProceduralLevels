using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour {
    [SerializeField] Aiming aiming;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float shootingCooldown = 0.2f;
    Timer cooldownTimer;

    bool cooldownReady = true;

    private void Awake() {
        cooldownTimer = TimerManager.getInstance()
                                    .CreateAndRegisterTimer(shootingCooldown, false, false, () => cooldownReady = true);
    }

    void Update() {
        if (cooldownReady && Mouse.current.leftButton.isPressed) {
            cooldownReady = false;
            cooldownTimer.resetTime();
            cooldownTimer.run();

            Vector3 toTarget = aiming.aimingRayHit - transform.position;
            float spawnOffset = 0.75f;

            Instantiate(bulletPrefab, transform.position + toTarget.normalized * spawnOffset, Quaternion.LookRotation(toTarget));
            GameObject.FindObjectOfType<SaveSerial>().shotsFiredToSave++;
        }
    }
}
