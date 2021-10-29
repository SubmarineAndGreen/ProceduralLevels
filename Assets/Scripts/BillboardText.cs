using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardText : MonoBehaviour
{
    Camera cam;

    private void Start() {
        cam = Camera.main;
    }

    void Update()
    {
        transform.rotation = cam.transform.rotation;
    }
}
