using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleBullet : MonoBehaviour, IBullet
{
    public float velocity;

    public void setTarget(Transform target) {
        transform.LookAt(target);
    }

    private void Update() {
        transform.position += transform.forward * velocity * Time.deltaTime;
    }
}
