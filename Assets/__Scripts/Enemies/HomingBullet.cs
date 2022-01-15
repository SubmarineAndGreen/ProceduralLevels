using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : MonoBehaviour, IBullet
{
    public float velocity = 5f;
    public float angularVelocity = 5f;
    public Transform target;

    public void setTarget(Transform target) {
        this.target = target;
        transform.LookAt(target);
    }

    private void Update() {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - this.transform.position);
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRotation, angularVelocity * Time.deltaTime);
        this.transform.position += this.transform.forward * velocity * Time.deltaTime;
    }
}
