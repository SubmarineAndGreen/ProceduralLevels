using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    [SerializeField] private float cameraSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;
    private Vector3 _movementInput;
    private Vector2 _mouseInput;
    private float _mouseScroll;
    private bool isMovementEnabled;

    private const int RMB = 1;
    private const float INVERT_CAMERA = -1f;
    private const float CAMERA_STEP = 0.1f;
    void Update()
    {
        isMovementEnabled = Input.GetMouseButton(RMB);
        
        if(isMovementEnabled) {
            Cursor.lockState = CursorLockMode.Locked;

            _movementInput.x = Input.GetAxisRaw("Horizontal");
            _movementInput.z = Input.GetAxisRaw("Vertical");

            _mouseInput.x = Input.GetAxisRaw("Mouse X");
            _mouseInput.y = Input.GetAxisRaw("Mouse Y");
            _mouseScroll = Input.mouseScrollDelta.y;

            cameraSpeed = Mathf.Max(CAMERA_STEP, cameraSpeed + _mouseScroll * CAMERA_STEP);

            transform.localPosition += 
                transform.TransformDirection(_movementInput) * cameraSpeed * Time.deltaTime;


            transform.Rotate(Vector3.up, _mouseInput.x * mouseSensitivity, Space.World);
            transform.Rotate(transform.right, INVERT_CAMERA *  _mouseInput.y * mouseSensitivity, Space.World);
        } else {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
