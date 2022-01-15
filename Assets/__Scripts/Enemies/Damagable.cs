using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    public float health;
    public event Action onTakeDamage;
    public void takeDamage(float damage) {
        health -= damage;
        onTakeDamage();
    }
}
