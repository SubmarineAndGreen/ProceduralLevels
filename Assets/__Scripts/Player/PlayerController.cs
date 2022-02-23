using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    InputMap inputMap;
    Rigidbody rb;
    RaycastHit groundCheckInfo;
    PlayerInput playerInput;
    Vector3 groundNormal;
    int groundContactCount;
    bool grounded;
    [SerializeField] float groundCheckDistanceFromOrigin = 1.25f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float maxGroundAngle;
    float minNormalY;
    [SerializeField] float maxSpeed;
    [SerializeField] float maxVerticalSpeed = 15f;
    [SerializeField] float groundAcceleration;
    [SerializeField] float airAcceleration;
    [SerializeField] float groundDrag;
    [SerializeField] float airDrag;
    [SerializeField] float gravity = 9.8f;
    [SerializeField] float jumpHeight = 1;
    bool jumpedLastUpdate;
    [SerializeField] Transform playerCameraTransform;
    Vector3 cameraOffest;
    Vector3 playerRotation = Vector3.zero;
    float maxVerticalRotation = 85f;
    [SerializeField] Vector2 mouseSensitivity;
    [SerializeField] SFXPlaying sfx;

    private void OnValidate() {
        minNormalY = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    private void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        cameraOffest = playerCameraTransform.position - transform.position;

        rb = GetComponent<Rigidbody>();
        // feetCollider = GetComponent<SphereCollider>();

        inputMap = new InputMap();
        inputMap.Player.Enable();
        playerInput = new PlayerInput();

        groundCheckInfo = new RaycastHit();
    }

    private void FixedUpdate() {
        groundCheck();
        updateRbRotation();
        updateHorizontalVelocity();
        handleJump();
        applyGravity();
        floatAboveGroundColliderSeamFix();
    }

    void Update() {
        readInput();
        updateRotationValues();
    }

    private void LateUpdate() {
        udpateCameraPosition();
        updateCameraRotation();
    }

    void updateRotationValues() {
        const int INVERT_Y = -1;
        playerRotation.y += playerInput.lookAxes.x;
        playerRotation.x += playerInput.lookAxes.y * INVERT_Y;
        playerRotation.x = Mathf.Clamp(playerRotation.x, -maxVerticalRotation, maxVerticalRotation);
    }

    void updateRbRotation() {
        rb.rotation = Quaternion.Euler(0f, playerRotation.y, 0f);
    }

    void updateCameraRotation() {
        playerCameraTransform.localRotation = Quaternion.Euler(playerRotation.x, playerRotation.y, 0f);
    }

    void udpateCameraPosition() {
        playerCameraTransform.position = transform.position + cameraOffest;
    }

    void groundCheck() {
        if (Physics.Raycast(transform.position,
                            Vector3.down,
                            out groundCheckInfo,
                            groundCheckDistanceFromOrigin,
                            groundMask,
                            QueryTriggerInteraction.Ignore)) {
            if (groundCheckInfo.normal.y >= minNormalY) {
                grounded = true;
                groundNormal = groundCheckInfo.normal;
            } else {
                grounded = false;
                groundNormal = Vector3.up;
            }
        } else {
            grounded = false;
            groundNormal = Vector3.up;
        }
    }

    void updateHorizontalVelocity() {
        Vector3 projectedForward = 
            Vector3.ProjectOnPlane(transform.forward, groundNormal);
        Vector3 projectedRight = 
            Vector3.ProjectOnPlane(transform.right, groundNormal);

        Vector3 movementPlaneNormal = grounded ? groundNormal : Vector3.up;
        Vector3 wishDirection =
        Vector3.ProjectOnPlane(
            transform.TransformDirection(playerInput.movementAxes),
        movementPlaneNormal);

        Vector3 oldVelocity = rb.velocity;
        Vector3 verticalComponent = Vector3.Dot(oldVelocity, groundNormal) * groundNormal;
        oldVelocity -= verticalComponent;

        float acceleration = grounded ? groundAcceleration : airAcceleration;
        float drag = grounded ? groundDrag : airDrag;

        float speedFromAcceleration = acceleration * Time.fixedDeltaTime;
        float speedFromDrag = -drag * Time.fixedDeltaTime;

        Vector3 maxNewVelocity = oldVelocity + wishDirection * speedFromAcceleration;

        if (maxNewVelocity.magnitude + speedFromDrag < 0) {
            speedFromDrag = -maxNewVelocity.magnitude;
        }

        Vector3 dragVelocity = speedFromDrag * oldVelocity.normalized;
        Vector3 newVelocityWithDrag = maxNewVelocity + dragVelocity;

        if (newVelocityWithDrag.magnitude > maxSpeed) {
            newVelocityWithDrag = Vector3.ClampMagnitude(newVelocityWithDrag, oldVelocity.magnitude);
        }

        Vector3 totalVelocity = newVelocityWithDrag + verticalComponent;
        rb.velocity = totalVelocity;

        if (!grounded || jumpedLastUpdate) {
            jumpedLastUpdate = false;
        }


        if (!grounded || (totalVelocity.x == 0 && totalVelocity.z == 0))
            sfx.StopWalk();
        else
            sfx.PlayWalk();

    }

    private void applyGravity() {
        Vector3 velocity = rb.velocity;
        if (!grounded) {
            float verticalVelocity = velocity.y;
            velocity.y -= gravity * Time.fixedDeltaTime;
            velocity.y = Mathf.Clamp(velocity.y, -maxVerticalSpeed, maxVerticalSpeed);
            rb.velocity = velocity;
        }
    }

    private void handleJump() {
        if (playerInput.jumpQueued && grounded) {
            playerInput.jumpQueued = false;
            float jumpSpeed = Mathf.Sqrt(2 * gravity * jumpHeight);
            rb.velocity += Vector3.up * jumpSpeed;
        }
    }

    private void floatAboveGroundColliderSeamFix() {
        float floatingHeight = groundCheckDistanceFromOrigin - 0.01f;

        if (grounded) {
            Vector3 position = rb.position;
            Vector3 velocity = rb.velocity;

            position.y = groundCheckInfo.point.y + floatingHeight;
            rb.position = position;

            if (velocity.y < 0) {
                float dot = Vector3.Dot(velocity, groundNormal);
                velocity -= groundNormal * dot;
                rb.velocity = velocity;
            }
        }
    }

    struct PlayerInput {
        public Vector3 movementAxes;
        public bool jumpQueued;
        public Vector2 lookAxes;
    }

    void readInput() {
        Vector2 movementAxes = inputMap.Player.MovementAxes.ReadValue<Vector2>();
        playerInput.movementAxes = new Vector3(movementAxes.x, 0, movementAxes.y);

        if (inputMap.Player.Jump.triggered) {
            playerInput.jumpQueued = true;
        }

        playerInput.lookAxes = inputMap.Player.Look.ReadValue<Vector2>();
        playerInput.lookAxes.x *= mouseSensitivity.x;
        playerInput.lookAxes.y *= mouseSensitivity.y;
    }
}
