using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineBullet : MonoBehaviour, IBullet {

    [SerializeField] float launchSpeed, gravity, startingToruqe;
    [SerializeField] float launchAngle = 10f;
    [SerializeField] float despawnTime = 100f;

    Rigidbody rb;
    
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        despawnTime -= Time.deltaTime;
        if(despawnTime <= 0) {
            Destroy(transform.GetChild(0).gameObject);
            Destroy(this.gameObject);
        }    
    }

    private void FixedUpdate() {
        rb.AddForce(-transform.up * gravity * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    public void setTarget(Transform target) {
        rb.isKinematic = false;

        Vector3 toTarget = target.position - transform.position;
        toTarget = Quaternion.AngleAxis(launchAngle, Vector3.Cross(toTarget, transform.up)) * toTarget;
        rb.AddForce(toTarget * launchSpeed, ForceMode.VelocityChange);
        rb.AddTorque(Random.insideUnitSphere * startingToruqe, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other) {
        rb.isKinematic = true;
        // Debug.Log("mine hit");
    }
}
