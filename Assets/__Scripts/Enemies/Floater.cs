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
                Vector3 vectorTowardsPlayer = navigationManager.playerTransform.position - transform.position;
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
        return navigationManager.getGridDistanceToPlayerWorld(transform.position) <= maxTileFollowDistance;
    }

    private bool playerInLineOfSight() {
        Ray lineOfSightRay = new Ray(transform.position, navigationManager.playerTransform.position - transform.position);
        if (Physics.Raycast(lineOfSightRay, out lineOfSightInfo, lineOfSightDistance, lineOfSightMask)) {

            return lineOfSightInfo.collider.gameObject.layer == navigationManager.playerLayer;
        }

        return false;
    }

    private enum FloaterState {
        NAVIGATING,
        FOLLOWING
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("test");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Took " + 1 + " damage!");
            collision.gameObject.GetComponent<PlayerStatus>().TakeDamage(1);
        }
    }
}
