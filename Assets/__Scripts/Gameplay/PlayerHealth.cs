using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour {
    [SerializeField] Damagable damagablePlayer;
    [SerializeField] float regenerationCooldown = 5f, regenerationPerSecond = 20f;
    Timer regenerationCooldownTimer;
    bool canRegenerate = false;

    const float maxHealth = 100f;
    Image heartImage;
    TextMeshProUGUI hpText;

    private void Awake() {
        UI_Display ui = FindObjectOfType<UI_Display>();
        
        heartImage = ui.heartImageBar;
        heartImage.material.SetFloat("_Health", damagablePlayer.health);

        hpText = ui.hpText;
        regenerationCooldownTimer = TimerManager.getInstance().CreateAndRegisterTimer(regenerationCooldown, false, false, () => canRegenerate = true);
        damagablePlayer.afterTakeDamage += playerTookDamage;
    }

    private void Update() {
        if (canRegenerate && damagablePlayer.health < maxHealth) {
            damagablePlayer.health += regenerationPerSecond * Time.deltaTime;
            if (damagablePlayer.health > maxHealth) {
                damagablePlayer.health = maxHealth;
            }
            updateUI();
        }
    }

    void playerTookDamage() {
        updateUI();
        canRegenerate = false;
        regenerationCooldownTimer.resetTime();
        regenerationCooldownTimer.run();
    }

    void updateUI() {
        heartImage.material.SetFloat("_Health", damagablePlayer.health);
        hpText.text = Mathf.FloorToInt(damagablePlayer.health).ToString();
    }
}
