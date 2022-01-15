using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour {
    public float damage;

    private void OnTriggerEnter(Collider other) {
        Damagable damagable = other.gameObject.GetComponentInParent<Damagable>();
        if (damagable != null) {
            damagable.takeDamage(damage);
        }

        Destroy(this.gameObject);
    }
}
