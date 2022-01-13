using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] public UI_Display ui;
    public int playerHP;
    public int playerMaxHP=10;

    // Start is called before the first frame update
    void Start()
    {
        playerHP = playerMaxHP;
        // ui.UpdateHealth(playerMaxHP);
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
            ui.DeathScreen();
        }
        ui.UpdateHealth(playerHP);
    }
}
