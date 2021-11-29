using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour {
    public float springMaxForceDistance = 0f;
    public float springMinForceDistance = 0.5f;
    public float maxSpringForce = 1f;
    public float minSpringForce = 0f;
    private Ray springRay;
    private RaycastHit springRayHit;
    private Rigidbody rb;
    private int springRayMask;

    private void Awake() {
        rb = GetComponent<Rigidbody>();

        springRay = new Ray(transform.position, Vector3.down);
        springRayHit = new RaycastHit();
    }

    private void Start() {

        // for(float test =  1; test < 2; test += 0.1f) {
        //    Debug.Log(test + "remapped: " + MathUtils.remap(test, springMaxForceDistance, springMinForceDistance, 0, 1)); 
        // }
    }

    void Update() {
        Debug.DrawRay(transform.position, Vector3.down * springMinForceDistance, Color.red);
    }

    private void FixedUpdate() {
        if(Physics.Raycast(springRay, out springRayHit, springMaxForceDistance)) {
            float t = MathUtils.remap(springRayHit.distance, springMaxForceDistance, springMinForceDistance, 0, 1);
            float springForce = Mathf.Lerp(maxSpringForce, minSpringForce, t);
             
            Debug.Log(springForce);
            rb.AddForce(Vector3.up * springForce, ForceMode.Force);
        }
    }
}
