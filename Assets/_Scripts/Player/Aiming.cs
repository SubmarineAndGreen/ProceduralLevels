using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Aiming : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    Ray aimingRay;

    private void Awake() {
        aimingRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    }

    private void FixedUpdate() {
        
    }
}
