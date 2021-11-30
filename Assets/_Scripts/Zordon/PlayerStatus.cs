using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int playerHP;
    public int playerMaxHP=10;

    // Start is called before the first frame update
    void Start()
    {
        playerHP = playerMaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        playerHP -= damage;
        Debug.Log("Player HP: "+playerHP);
        if(playerHP==0)
        {
            Debug.Log("You Died");
        }
    }
}
