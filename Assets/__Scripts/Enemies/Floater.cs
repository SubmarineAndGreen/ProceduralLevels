using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour {
    [SerializeField] float navigationForce = 5;
    [SerializeField] float followForce = 5;
    [SerializeField] float followDistance = 50;
    int playerLayer;
    Rigidbody rb;
    NavigationManager navigationManager;
    EnemyManager enemyManager;
    FloaterState state;
    RaycastHit lineOfSightInfo;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        lineOfSightInfo = new RaycastHit();

    }

    private void Start() {
        navigationManager = NavigationManager.instance;
        enemyManager = EnemyManager.instance;
    }

    private void FixedUpdate() {
        // if(playerInFollowRange() && playerInLineOfSight()) {
        //     state = FloaterState.FOLLOWING;
        // } else {
        //     state = FloaterState.NAVIGATING;
        // }

        switch (state) {
            case FloaterState.NAVIGATING:
                rb.AddForce(navigationManager.getPathVectorToPlayer(transform.position) * navigationForce * Time.fixedDeltaTime, ForceMode.Force);
                break;
            // case FloaterState.FOLLOWING:
            //     rb.AddForce((enemyManager.playerTransform.position - transform.position) * followForce * Time.fixedDeltaTime, ForceMode.Force);
            //     break;
        }
    }

    private bool playerInFollowRange() {
        return (enemyManager.playerTransform.position - transform.position).magnitude < followDistance;
    }

    private bool playerInLineOfSight() {
        if(Physics.Raycast(transform.position, enemyManager.playerTransform.position, out lineOfSightInfo, followDistance)) {
            return lineOfSightInfo.collider.gameObject.layer == enemyManager.playerLayer;
        }

        return false;
    }

    private enum FloaterState {
        NAVIGATING,
        FOLLOWING
    }
}
