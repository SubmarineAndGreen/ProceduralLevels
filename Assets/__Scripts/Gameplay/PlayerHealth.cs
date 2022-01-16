using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] Damagable damagablePlayer;
    [SerializeField] float regenerationCooldown = 5f, regenerationPerSecond = 20f;
    Timer regenerationCooldownTimer;
    bool canRegenerate = false;

    const float maxHealth = 100f;

    private void Update() {
        if(canRegenerate && damagablePlayer.health < maxHealth) {
            damagablePlayer.health += regenerationPerSecond * Time.deltaTime;
            if(damagablePlayer.health > maxHealth) {
                damagablePlayer.health = maxHealth;
            }
        }
    }

    private void Awake() {
        regenerationCooldownTimer = TimerManager.getInstance().CreateAndRegisterTimer(regenerationCooldown, false, false, () => canRegenerate = true);
        damagablePlayer.afterTakeDamage += playerTookDamage;
    }

    void playerTookDamage() {
        canRegenerate = false;
        regenerationCooldownTimer.resetTime();
        regenerationCooldownTimer.run();
    }
}
