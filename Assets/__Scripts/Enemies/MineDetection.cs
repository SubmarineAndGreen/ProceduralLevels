using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MineDetection : MonoBehaviour {

    [SerializeField] Color flashColor;
    int playerLayer;
    [SerializeField] float explosionDelay = 1f, damageRange = 10, explosionRange = 20;
    bool triggered = false;
    [SerializeField] float damage, maxPushBackForce = 250f, minPushBackForce = 100f;
    [SerializeField] Rigidbody mineRigidbody;

    Damagable player;
    Rigidbody playerRb;
    [SerializeField] Renderer _renderer;

    private void Awake() {
        playerLayer = LayerMask.NameToLayer("Player");
        transform.localScale = new Vector3(damageRange, damageRange, damageRange);
    }

    private void Update() {
        if (triggered) {
            explosionDelay -= Time.deltaTime;


        }
    }

    private void FixedUpdate() {
        transform.position = mineRigidbody.position;
    }

    private void OnTriggerEnter(Collider other) {
        if(triggered) {
            return;
        }

        if (other.gameObject.layer == playerLayer) {
            DOTween.To(
            () => {
                return _renderer.material.GetColor("_Color");
            },
            color => {
                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                _renderer.GetPropertyBlock(propertyBlock, 0);
                propertyBlock.SetColor("_Color", color);
                _renderer.SetPropertyBlock(propertyBlock, 0);
            },
            flashColor, explosionDelay).SetEase(Ease.Flash, 5).OnComplete(() => {
                Vector3 toPlayer = playerRb.position - transform.position;
                float distanceToPlayer = toPlayer.magnitude;
                if (distanceToPlayer <= damageRange) {
                    player.takeDamage(damage);
                }

                if (distanceToPlayer <= explosionRange) {
                    float pushBackForce = Mathf.Lerp(maxPushBackForce, minPushBackForce, distanceToPlayer / explosionRange);
                    playerRb.AddForce(toPlayer.normalized * pushBackForce, ForceMode.Impulse);
                }


                for (int i = 0; i < transform.parent.childCount; i++) {
                    Destroy(transform.parent.GetChild(i).gameObject);
                }
                Destroy(transform.parent.gameObject);
            });

            triggered = true;

            player = other.gameObject.GetComponent<Damagable>();
            playerRb = other.gameObject.GetComponent<Rigidbody>();

            
        }
    }
}
