using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Fan : MonoBehaviour {
    [SerializeField] private float angularVelocity = 5;
    Rigidbody rb;
    float rotation = -180;
    Transform parentTransform; 
    Quaternion initialRotation;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        parentTransform = this.transform.parent;
        transform.LookAt(parentTransform.position + parentTransform.forward, parentTransform.up);
        initialRotation = transform.rotation;
    }
    private void FixedUpdate() {
        rotation += angularVelocity * Time.fixedDeltaTime;

        if (rotation >= 180) {
            rotation = rotation - 360;
        }
        
        rb.MoveRotation(parentTransform.rotation * Quaternion.Euler(rotation, 0, 0));
    }
}
