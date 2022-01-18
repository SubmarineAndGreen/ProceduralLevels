using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Dash : MonoBehaviour {
    [SerializeField] float dashCooldown;
    [SerializeField] int maxDashCount = 2;
    int availableDashCount = 0;
    [SerializeField] float dashAddedSpeed = 2;
    Timer dashCooldownTimer;
    Rigidbody rb;
    [SerializeField] Transform playerCameraTransform;
    [SerializeField] public GameUI ui;
    [SerializeField] public GameObject dashEffect;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        ui = FindObjectOfType<GameUI>();
    }

    private void Start() {
        dashCooldownTimer = TimerManager.getInstance().CreateAndRegisterTimer(dashCooldown, false, false, onDashCooldownDone);
        availableDashCount = maxDashCount;
    }

    private void onDashCooldownDone() {
        addDash();
        runCooldownIfNotMaxDashes();
    }

    private void addDash() {
        availableDashCount += 1;
        // Debug.Log("(cooldown) dashes: " + availableDashCount);
        updateDashCountUI();
    }

    private void runCooldownIfNotMaxDashes() {
        if (availableDashCount != maxDashCount) {
            dashCooldownTimer.resetTime();
            dashCooldownTimer.run();
            // Debug.Log("cooldown running");
        }
    }

    private void tryDash() {
        if (availableDashCount > 0) {
            availableDashCount--;

            if (!dashCooldownTimer.isRunning()) {
                dashCooldownTimer.resetTime();
                dashCooldownTimer.run();
            }

            Vector3 lookDirection = playerCameraTransform.forward.normalized;
            float speedAfterDash = rb.velocity.magnitude + dashAddedSpeed;
            rb.velocity = lookDirection * speedAfterDash;
            // Debug.Log("(use) dashes: " + availableDashCount);
            ui.DashEffect();
            //Instantiate(dashEffect, playerCameraTransform);
            updateDashCountUI();
        }
    }

    void Update() {
        if (Mouse.current.rightButton.wasPressedThisFrame) {
            tryDash();
        }
        UpdateDashesCooldown(dashCooldownTimer.getTimeRunning());
    }

    void updateDashCountUI() {
        for (int i = 0; i < maxDashCount; i++) {
            ui.dashImage[i].SetActive(i < availableDashCount);
        }
    }

    public void UpdateDashesCooldown(float cd) {
        ui.dashSliderImage.material.SetFloat("_Progress", cd / dashCooldown);
    }
}
