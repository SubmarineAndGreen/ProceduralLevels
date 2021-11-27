using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InputManager;

public class PlayerRotate : MonoBehaviour
{
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    [SerializeField] Transform playerCamera;
    [SerializeField] Transform orientation;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float rotationX;
    float rotationY;

    private void Start()
    {
        //camera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        MyInput();
    }

    void MyInput()
    {
        var mouseInput = inputs.Player.Look.ReadValue<Vector2>();
        mouseX = mouseInput.x;
        mouseY = mouseInput.y;

        rotationY += mouseX * sensX * multiplier;
        rotationX -= mouseY * sensY * multiplier;

        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
        orientation.transform.rotation = Quaternion.Euler(0, rotationY, 0);
    }
}
