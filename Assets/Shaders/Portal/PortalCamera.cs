using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour {
    [SerializeField] Transform baseCameraTransform;

    private void Start() {
        if (baseCameraTransform == null) {
            baseCameraTransform = Camera.main.transform;
        }
    }

    void Update() {
        // if (baseCameraTransform == null) {
        //     baseCameraTransform = Camera.main.transform;
        // } else {
            this.transform.rotation = baseCameraTransform.rotation;
        // }
    }
}
