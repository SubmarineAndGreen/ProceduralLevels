using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour {
    [SerializeField] Transform baseCameraTransform;
    Vector3 origin;

    private void Start() {
        if (baseCameraTransform == null) {
            baseCameraTransform = Camera.main.transform;
        }
        origin = this.transform.position;
    }

    void Update() {
        this.transform.rotation = baseCameraTransform.rotation;
        this.transform.position = origin + baseCameraTransform.localPosition;
    }
}
