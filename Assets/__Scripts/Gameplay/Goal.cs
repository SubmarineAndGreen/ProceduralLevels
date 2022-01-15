using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public event Action onGoalReached;
    LayerMask playerMask;

    private void Start() {
        playerMask = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter(Collider other) {
        // Debug.Log(LayerMask.LayerToName(other.gameObject.layer));
        
        if(other.gameObject.layer == playerMask) {
            onGoalReached();
            Destroy(this.transform.parent.gameObject);
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy() {
        onGoalReached = null;
    }
}
