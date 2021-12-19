using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour {

    [SerializeField] LayerMask lineOfSightMask;
    [SerializeField] float navigationForce = 300;
    [SerializeField] float followForce = 300;
    [SerializeField] float lineOfSightDistance = 100;
    [SerializeField] int maxTileFollowDistance = 2;
    [SerializeField] float ySpinTorque = 10;
    int playerLayer;
    Rigidbody rb;
    NavigationManager navigationManager;
    EnemyManager enemyManager;
    FloaterState state;
    RaycastHit lineOfSightInfo;
    private int hp;
    public UI_Display ui;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        lineOfSightInfo = new RaycastHit();
        ui = GameObject.Find("Status").GetComponent<UI_Display>();
    }

    private void Start() {
        navigationManager = NavigationManager.instance;
        enemyManager = EnemyManager.instance;
        state = FloaterState.NAVIGATING;
        hp = 1;
    }

    private void FixedUpdate() {
        if (playerInFollowRange() && playerInLineOfSight()) {
            // Debug.Log("following");
            state = FloaterState.FOLLOWING;
        } else {
            state = FloaterState.NAVIGATING;
            // Debug.Log("navigating");
        }
        //spin around y axis
        rb.AddTorque(Vector3.up * ySpinTorque * Time.fixedDeltaTime, ForceMode.Force);

        switch (state) {
            case FloaterState.NAVIGATING:
                rb.AddForce(navigationManager.getPathVectorToPlayer(transform.position) * navigationForce * Time.fixedDeltaTime, ForceMode.Force);
                break;
            case FloaterState.FOLLOWING:
                Vector3 vectorTowardsPlayer = enemyManager.playerTransform.position - transform.position;
                rb.AddForce(vectorTowardsPlayer.normalized * followForce * Time.fixedDeltaTime, ForceMode.Force);
                break;
        }
    }

    public void TakeDamage(int damage) {
        hp -= damage;
        if (hp <= 0) {
            ui.AddProgress(1);
            ui.AddEnergy(10);
            Destroy(gameObject);
        }
    }

    private bool playerInFollowRange() {
        return navigationManager.getGridDistanceToPlayer(transform.position) <= maxTileFollowDistance;
    }

    private bool playerInLineOfSight() {
        Ray lineOfSightRay = new Ray(transform.position, enemyManager.playerTransform.position - transform.position);
        if (Physics.Raycast(lineOfSightRay, out lineOfSightInfo, lineOfSightDistance, lineOfSightMask)) {

            return lineOfSightInfo.collider.gameObject.layer == enemyManager.playerLayer;
        }

        return false;
    }

    // private void OnDrawGizmos() {
    //     Gizmos.DrawLine(transform.position, transform.position + (enemyManager.playerTransform.position - transform.position) * lineOfSightDistance);
    // }

    private enum FloaterState {
        NAVIGATING,
        FOLLOWING
    }
}
