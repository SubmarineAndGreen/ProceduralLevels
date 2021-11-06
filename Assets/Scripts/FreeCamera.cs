using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour
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
        isMovementEnabled = MyInput.cameraControls.isMovementEnabled;
        
        if(isMovementEnabled) {

            _movementInput.x = MyInput.cameraControls.movementInput.x;
            _movementInput.z = MyInput.cameraControls.movementInput.z;
            _mouseInput.x = MyInput.cameraControls.mouseInput.x;
            _mouseInput.y = MyInput.cameraControls.mouseInput.y;
            _mouseScroll = MyInput.cameraControls.mouseScroll;
;
            cameraSpeed = Mathf.Max(CAMERA_STEP, cameraSpeed + _mouseScroll * CAMERA_STEP);

            transform.localPosition += 
                transform.TransformDirection(_movementInput) * cameraSpeed * Time.deltaTime;


            transform.Rotate(Vector3.up, _mouseInput.x * mouseSensitivity, Space.World);
            transform.Rotate(transform.right, INVERT_CAMERA *  _mouseInput.y * mouseSensitivity, Space.World);
        }
    }
}
