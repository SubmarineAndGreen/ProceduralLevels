using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingGun : MonoBehaviour
{
    Transform playerTransform;
    private void Start() {
        playerTransform = NavigationManager.instance.playerTransform;
    }

    private void Update() {
        transform.LookAt(playerTransform);
    }
}
