using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InputManager;

public class FreeCamera : MonoBehaviour {
    [SerializeField] private float cameraSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;

    private float _mouseScroll;
    private bool isMovementEnabled;
    private const float INVERT_CAMERA = -1f;
    void Update() {

        if (inputs.FreeCamera.enabled) {

            Vector2 movementInput = inputs.FreeCamera.KeyboardMovement.ReadValue<Vector2>();
            Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y);

            Vector2 mouseInput = inputs.FreeCamera.MouseRotation.ReadValue<Vector2>() * Time.unscaledDeltaTime;


            transform.localPosition +=
                transform.TransformDirection(movement) * cameraSpeed * Time.deltaTime;


            transform.Rotate(Vector3.up, mouseInput.x * mouseSensitivity, Space.World);
            transform.Rotate(transform.right, INVERT_CAMERA * mouseInput.y * mouseSensitivity, Space.World);
        }
    }
}
