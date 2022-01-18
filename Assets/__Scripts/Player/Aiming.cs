using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Aiming : MonoBehaviour {
    [SerializeField] Camera playerCamera;
    [HideInInspector] public Vector3 aimingRayHit;
    RaycastHit aimingHitInfo;

    private void Awake() {
        aimingHitInfo = new RaycastHit();
    }

    private void FixedUpdate() {
        Ray aimingRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        // Debug.DrawRay(aimingRay.origin, aimingRay.direction);
        if (Physics.Raycast(aimingRay, out aimingHitInfo)) {
            // Debug.Log("hit");
            aimingRayHit = aimingHitInfo.point;
        };
        Debug.DrawLine(aimingRay.origin, aimingHitInfo.point);
    }
}
