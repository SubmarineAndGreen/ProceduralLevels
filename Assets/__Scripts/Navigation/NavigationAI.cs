using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NavigationAI : MonoBehaviour {
    Rigidbody rb;
    NavigationManager navigationManager;
    Vector3 targetWorldPosition;
    RaycastHit rayHit;
    LayerMask levelMask;
    int enemyLayer;
    int ignoreDecorationsLayer;
    bool playerInSight = false;
    [SerializeField] int raycastSkippedPhysicsFramesCount = 5;
    int currentSkippedFrames = 0;
    [SerializeField] float followForce = 100f;
    [SerializeField] float navigationSpeed = 25f;
    [SerializeField] int switchToForceDistance = 2;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        rayHit = new RaycastHit();
    }

    private void Start() {
        navigationManager = NavigationManager.instance;
        levelMask = navigationManager.levelMask;
        enemyLayer = LayerMask.NameToLayer("Enemy");
        ignoreDecorationsLayer = LayerMask.NameToLayer("EnemyIgnoreDecorations");
        currentSkippedFrames = raycastSkippedPhysicsFramesCount;
    }

    bool start = true;
    private void Update() {
        if (start) {
            this.transform.position = navigationManager.gridPositionToWorldPosition(navigationManager.worldPositionToGridPosition(navigationManager.playerTransform.position));
            start = false;
        }

        Debug.DrawLine(rb.position, targetWorldPosition);
        Debug.DrawLine(rb.position, navigationManager.playerTransform.position, Color.yellow);
    }

    private void FixedUpdate() {
        if (currentSkippedFrames == raycastSkippedPhysicsFramesCount) {
            currentSkippedFrames = 0;
            if (!raycastForPlayerPosition()) {
                findFurthestCellCenterInLineOfSight();
                playerInSight = false;
            } else {
                playerInSight = true;
            }
        } else {
            currentSkippedFrames++;
        }

        Vector3 directionToTarget = (targetWorldPosition - rb.position).normalized;
        int gridDistanceToPlayer = navigationManager.getGridDistanceToPlayer(navigationManager.worldPositionToGridPosition(rb.position));

        if (playerInSight || gridDistanceToPlayer < switchToForceDistance) {
            this.gameObject.layer = enemyLayer;
            rb.AddForce(directionToTarget * followForce * Time.fixedDeltaTime, ForceMode.Force);

        } else {
            this.gameObject.layer = ignoreDecorationsLayer;
            rb.velocity = (targetWorldPosition - transform.position).normalized * navigationSpeed * Time.fixedDeltaTime;
        }
    }

    bool raycastForPlayerPosition() {
        if (Physics.Linecast(rb.position, navigationManager.playerTransform.position, out rayHit, navigationManager.ignoreDecorationsMask)) {
            if (rayHit.collider.gameObject.layer == navigationManager.playerLayer) {
                targetWorldPosition = rayHit.point;
                // Debug.Log("player hit");
                return true;
            }
        }
        return false;
    }

    void findFurthestCellCenterInLineOfSight() {
        int maxTries = 10;

        Vector3Int playerTile = navigationManager.worldPositionToGridPosition(navigationManager.playerTransform.position);
        Vector3Int currentPosition = navigationManager.worldPositionToGridPosition(rb.position);

        Vector3Int furthestCellInLineOfSight = currentPosition;
        Vector3Int nextCell = currentPosition;
        while (true) {
            if (maxTries-- == 0) {
                break;
            }

            Vector3Int nextCellOffset = Vector3Int.FloorToInt(navigationManager.getPathVector(playerTile, nextCell));
            if (nextCellOffset == Vector3Int.zero) {
                break;
            }

            nextCell += nextCellOffset;

            Vector3 nextTargetWorldPositon = navigationManager.gridPositionToWorldPosition(nextCell);
            if (Physics.Linecast(rb.position, nextTargetWorldPositon, out rayHit, levelMask)) {
                break;
            } else {
                furthestCellInLineOfSight = nextCell;
            }
        }

        // Debug.Log("tile hit," + furthestCellInLineOfSight);
        targetWorldPosition = navigationManager.gridPositionToWorldPosition(furthestCellInLineOfSight);
    }
}
