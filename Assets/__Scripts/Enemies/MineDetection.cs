using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineDetection : MonoBehaviour
{
    int playerLayer;
    [SerializeField] float damage, pushBackForce;
    [SerializeField] Rigidbody mineRigidbody;

    private void FixedUpdate() {
        transform.position = mineRigidbody.position;
    }

    private void Awake() {
        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == playerLayer) {
            Damagable player = other.gameObject.GetComponent<Damagable>();
            Rigidbody playerRb = other.gameObject.GetComponent<Rigidbody>();
            player.takeDamage(damage);
            playerRb.AddForce((playerRb.position - transform.position).normalized * pushBackForce, ForceMode.Impulse);
        }
    }
}
