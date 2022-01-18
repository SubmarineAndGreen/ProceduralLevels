using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour {
    public float health;
    public event Action afterTakeDamage;
    public void takeDamage(float damage) {
        health -= damage;
        if (afterTakeDamage != null) {
            afterTakeDamage();
        }
    }
}
