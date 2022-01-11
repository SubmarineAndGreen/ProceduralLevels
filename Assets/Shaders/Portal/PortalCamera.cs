using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour {
    [SerializeField] Transform baseCameraTransform;
    Vector3 origin;
    Camera thisCamera;

    int texWidth, texHeight;
    [SerializeField] Material portalMaterial;
    RenderTexture renderTexture;

    private void Start() {
        thisCamera = GetComponent<Camera>();
        if (baseCameraTransform == null) {
            baseCameraTransform = Camera.main.transform;
        }
        origin = this.transform.position;

        if (thisCamera.targetTexture != null) {
            thisCamera.targetTexture.Release();
        }

        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        thisCamera.targetTexture = renderTexture;
        thisCamera.aspect = ((float)Screen.width) / Screen.height;
        setMaterialTexture();
    }

    void Update() {
        this.transform.rotation = baseCameraTransform.rotation;
        this.transform.position = origin + baseCameraTransform.localPosition;

        if (thisCamera.targetTexture.width != Screen.width || thisCamera.targetTexture.height != Screen.height) {
            renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            thisCamera.targetTexture = renderTexture;
            thisCamera.aspect = ((float)Screen.width) / Screen.height;
            setMaterialTexture();
        }
    }

    void setMaterialTexture() {
        portalMaterial.SetTexture("_RenderTexture", renderTexture);
    }
}
