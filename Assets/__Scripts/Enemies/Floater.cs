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
        if(playerInFollowRange() && playerInLineOfSight()) {
            if(state == FloaterState.NAVIGATING) {
                Debug.Log("switching to following");
            }
            state = FloaterState.FOLLOWING;
        } else {
            state = FloaterState.NAVIGATING;
        }

        switch (state) {
            case FloaterState.NAVIGATING:
                rb.AddForce(navigationManager.getPathVectorToPlayer(transform.position) * navigationForce * Time.fixedDeltaTime, ForceMode.Force);
                break;
            case FloaterState.FOLLOWING:
                rb.AddForce((enemyManager.playerTransform.position - transform.position) * followForce * Time.fixedDeltaTime, ForceMode.Force);
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            ui.AddProgress(1);
            ui.AddEnergy(10);
            Destroy(gameObject);
        }
    }

    private bool playerInFollowRange() {
        return navigationManager.getGridDistanceToPlayer(transform.position) <= maxTileFollowDistance;
    }

    private bool playerInLineOfSight() {
        Ray lineOfSightRay = new Ray(transform.position, transform.position - enemyManager.playerTransform.position); 
        if(Physics.Raycast(lineOfSightRay, out lineOfSightInfo, lineOfSightDistance, lineOfSightMask)) {
            // Debug.Log(lineOfSightInfo.collider.gameObject.layer);
            return lineOfSightInfo.collider.gameObject.layer == enemyManager.playerLayer;
        }

        return false;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, (enemyManager.playerTransform.position - transform.position) * lineOfSightDistance);
    }

    private enum FloaterState {
        NAVIGATING,
        FOLLOWING
    }
}
