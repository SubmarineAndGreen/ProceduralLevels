using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Aiming), typeof(Rigidbody), typeof(LineRenderer))]
public class GrapplingHook : MonoBehaviour {
    Aiming aiming;
    LineRenderer lineRenderer;
    Rigidbody rb;
    [SerializeField] float maxGrappleDistance = 3f;
    [SerializeField] float startPullForce = 2f;
    [SerializeField] float constantPullForce = 0.5f;
    [SerializeField] float grappleCooldown = 0.5f;
    bool cooldownReady;
    Timer cooldownTimer;
    bool grapplingActive;
    bool stratedPulling = false;
    RaycastHit obstacleCheckHit;
    private void Awake() {
        obstacleCheckHit = new RaycastHit();
        aiming = GetComponent<Aiming>();
        rb = GetComponent<Rigidbody>();

        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start() {
        cooldownTimer = TimerManager.getInstance().CreateAndRegisterTimer(grappleCooldown, false, false, resetCooldown);
        cooldownReady = true;
    }

    Vector3 grapplingPoint;

    private void Update() {
        // bool timerOffCd = cooldownTimer.getTimeRunning() == 0;
        // Debug.Log("on cd: " + !timerOffCd);
        // Debug.Log((transform.position - aiming.aimingHit).magnitude);
        if (cooldownReady && Mouse.current.leftButton.wasPressedThisFrame) {
            if ((transform.position - aiming.aimingHit).magnitude <= maxGrappleDistance) {
                startCooldown();

                grapplingActive = true;
                grapplingPoint = aiming.aimingHit;
                stratedPulling = false;

                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(1, aiming.aimingHit);
            }
        }

        if ((transform.position - grapplingPoint).magnitude > maxGrappleDistance) {
            grapplingActive = false;
        }

        if (!Mouse.current.leftButton.isPressed) {
            grapplingActive = false;
        }

        if (!grapplingActive) {
            lineRenderer.positionCount = 0;
        } else {
            lineRenderer.SetPosition(0, transform.position);
        }
    }

    private void FixedUpdate() {
        if (grapplingActive) {
            checkForObstacles();
            addPullForce();
        }
    }

    void checkForObstacles() {
        Vector3 grapplingPointOffset = grapplingPoint - transform.position;
        Ray obstacleCheckRay = new Ray(transform.position, grapplingPoint - transform.position);
        if (Physics.Raycast(obstacleCheckRay, out obstacleCheckHit, grapplingPointOffset.magnitude - 0.01f)) {
            grapplingActive = false;
        }
    }

    void addPullForce() {
        Vector3 forceDirection = (grapplingPoint - transform.position).normalized;
        if (!stratedPulling) {
            Vector3 impulsePull = forceDirection * startPullForce;
            rb.AddForce(impulsePull, ForceMode.Impulse);
            stratedPulling = true;
        }
        Vector3 constantPull = forceDirection * constantPullForce * Time.fixedDeltaTime;
        rb.AddForce(constantPull, ForceMode.Force);
    }

    void resetCooldown() {
        cooldownReady = true;
        cooldownTimer.resetTime();
    }

    void startCooldown() {
        cooldownReady = false;
        cooldownTimer.run();
    }
}
