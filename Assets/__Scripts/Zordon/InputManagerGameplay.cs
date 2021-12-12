using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerGameplay : MonoBehaviour
{
    public static InputMap inputGameplay;

    private void Awake()
    {
        inputGameplay = new InputMap();
        //inputs.Global.Enable();
        inputGameplay.Player.Enable();
    }

    //private const int RMB = 1;
    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}

