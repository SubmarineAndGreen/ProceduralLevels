using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    public static InputMap inputs;

    private void Awake() {
        inputs = new InputMap();
        inputs.Global.Enable();
        inputs.Player.Enable();
    }

    private const int RMB = 1;
    void Update() {
        if (inputs.Global.CameraOn.ReadValue<float>() == 1) {
            Cursor.lockState = CursorLockMode.Locked;
            inputs.FreeCamera.Enable();
            inputs.GridEditor.Disable();
        } else {
            Cursor.lockState = CursorLockMode.None;
            inputs.GridEditor.Enable();
            inputs.FreeCamera.Disable();
        }
    }
}


public static class VecUtils {
    public static Vector2Int toVector2Int(this Vector2 v) {
        return new Vector2Int((int)v.x, (int)v.y);
    }
}

