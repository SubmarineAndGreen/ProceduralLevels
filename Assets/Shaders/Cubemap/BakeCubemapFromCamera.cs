using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class BakeCubemapFromCamera : MonoBehaviour {
    Camera thisCamera;
    [SerializeField] Cubemap resultCubemap;

    private void Awake() {
        thisCamera = GetComponent<Camera>();
    }
    void Start() {
        thisCamera.RenderToCubemap(resultCubemap);
    }
}
