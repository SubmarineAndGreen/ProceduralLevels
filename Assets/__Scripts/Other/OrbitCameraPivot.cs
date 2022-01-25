using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCameraPivot : MonoBehaviour
{
    [SerializeField] float angularVelocity;

    private void Update() {
        transform.Rotate(0, angularVelocity * Time.deltaTime, 0, Space.Self);
    }
}
