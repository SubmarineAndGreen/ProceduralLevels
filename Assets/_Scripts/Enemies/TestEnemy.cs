using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour {
    public float gravity = 9.8f;
    public float springMaxForceDistance = 0f;
    public float springMinForceDistance = 0.5f;
    public float maxSpringForce = 1f;
    public float minSpringForce = 0f;
    private RaycastHit springRayHit;
    private int springRayMask;
    private Rigidbody rb;
    public float pushAwayForce = 0.3f;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        springRayHit = new RaycastHit();
    }

    void Update() {
        Debug.DrawRay(transform.position, Vector3.down * springMinForceDistance, Color.red);
    }

    private void FixedUpdate() {
        if(Physics.Raycast(transform.position, Vector3.down, out springRayHit, springMinForceDistance)) {

            float t = MathUtils.remap(springRayHit.distance, springMaxForceDistance, springMinForceDistance, 0, 1);
            float springForce = Mathf.Lerp(minSpringForce, maxSpringForce, 1 - t);
             
            rb.AddForce(Vector3.up * springForce, ForceMode.Force);
        }
        rb.AddForce(Vector3.down * gravity, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision other) {
        pushAwayFromCollision(other.contacts);
    }

    private void pushAwayFromCollision(ContactPoint[] contactPoints) {
        Debug.Log("push");
        foreach(ContactPoint contact in contactPoints) {
            Vector3 collisionDirection = transform.position - contact.point;
            rb.AddForce(collisionDirection.normalized * pushAwayForce, ForceMode.Impulse);
        }
    }
}
