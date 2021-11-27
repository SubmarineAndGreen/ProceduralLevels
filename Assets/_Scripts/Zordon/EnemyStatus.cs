using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    [SerializeField] public int enemyMaxHP=3;
    public int enemyCurrentHP;
    [SerializeField] public int damageOfEnemy;
    // Start is called before the first frame update
    void Start()
    {
        SetMaxHP();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(int damage)
    {
        enemyCurrentHP -= damage;
        Debug.Log("Enemy took " + damage + " damage");
        if (enemyCurrentHP<=0)
        {
            //gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
    public void SetMaxHP()
    {
        enemyCurrentHP = enemyMaxHP;
    }
}
