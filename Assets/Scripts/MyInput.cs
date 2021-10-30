using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyInput : MonoBehaviour
{
    public static GridControls gridControls;
    public static CameraControls cameraControls;

    private const int RMB = 1;
    void Update()
    {
        cameraControls.isMovementEnabled = Input.GetMouseButton(RMB);
        
        if (cameraControls.isMovementEnabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            gridControls.disabled = true;
            cameraControls.movementInput.x = Input.GetAxisRaw("Horizontal");
            cameraControls.movementInput.z = Input.GetAxisRaw("Vertical");
            cameraControls.mouseInput.x = Input.GetAxisRaw("Mouse X");
            cameraControls.mouseInput.y = Input.GetAxisRaw("Mouse Y");
            cameraControls.mouseScroll = Input.mouseScrollDelta.y;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            gridControls.disabled = false;
            gridControls.keyW = Input.GetKeyDown(KeyCode.W);
            gridControls.keyA = Input.GetKeyDown(KeyCode.A);
            gridControls.keyS = Input.GetKeyDown(KeyCode.S);
            gridControls.keyD = Input.GetKeyDown(KeyCode.D);
            gridControls.heightToggle = Input.GetKey(KeyCode.LeftShift);
            gridControls.place = Input.GetKeyDown(KeyCode.Space);
            gridControls.nextTile = Input.GetKeyDown(KeyCode.E);
            gridControls.previousTile = Input.GetKeyDown(KeyCode.Q);
            gridControls.rotate = Input.GetKeyDown(KeyCode.R);
            gridControls.remove = Input.GetKeyDown(KeyCode.Delete);
        }
    }
}

public struct GridControls
{
    public bool disabled;
    public bool keyW;
    public bool keyA;
    public bool keyS;
    public bool keyD;
    public bool heightToggle;
    public bool nextTile;
    public bool previousTile;
    public bool place;
    public bool remove;
    public bool rotate;
}

public struct CameraControls
{
    public Vector3 movementInput;
    public Vector2 mouseInput;
    public float mouseScroll;
    public bool isMovementEnabled;
}
