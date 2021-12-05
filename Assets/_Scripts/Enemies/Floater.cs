using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour {
    Rigidbody rb;
    NavigationManager navigationManager;
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        navigationManager = NavigationManager.instance;
    }

    private void FixedUpdate() {
        rb.AddForce(navigationManager.getPathVectorToPlayer(transform.position));
    }
}
