using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineBullet : MonoBehaviour, IBullet {

    [SerializeField] float launchSpeed, gravity, startingToruqe;
    [SerializeField] float launchAngle = 10f;
    [SerializeField] float despawnTime = 100f;
    [SerializeField] SphereCollider mineDetectionCollider;

    Rigidbody rb;
    
    private void Awake() {
        mineDetectionCollider.enabled = false;
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        despawnTime -= Time.deltaTime;
        if(despawnTime <= 0) {
            for(int i = 0; i < transform.parent.childCount; i++) {
                Destroy(transform.parent.GetChild(i).gameObject);
            }
            Destroy(transform.parent.gameObject);
        }    
    }

    private void FixedUpdate() {
        rb.AddForce(-transform.up * gravity * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    public void setTarget(Transform target) {
        rb.isKinematic = false;

        Vector3 toTarget = target.position - transform.position;
        toTarget = Quaternion.AngleAxis(launchAngle, Vector3.Cross(toTarget, transform.up)) * toTarget;
        rb.AddForce(toTarget.normalized * launchSpeed, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other) {
        rb.isKinematic = true;
        mineDetectionCollider.enabled = true;
    }
}
