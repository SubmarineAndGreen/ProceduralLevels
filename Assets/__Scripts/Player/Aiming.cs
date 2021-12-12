using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Aiming : MonoBehaviour {
    [SerializeField] Camera playerCamera;
    [HideInInspector] public Vector3 aimingHit;
    RaycastHit aimingHitInfo;

    private void Awake() {
        aimingHitInfo = new RaycastHit();
    }

    private void FixedUpdate() {
        Ray aimingRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(aimingRay, out aimingHitInfo, 100)) {
            aimingHit = aimingHitInfo.point;
        };
    }
}
