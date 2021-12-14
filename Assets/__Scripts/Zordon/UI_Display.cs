using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Display : MonoBehaviour
{
    [SerializeField] public Slider healthSlider;
    [SerializeField] public Slider progressSlider;
    [SerializeField] public Slider energySlider;
    //[SerializeField] public TextMeshProUGUI healthText;
    [SerializeField] public TextMeshProUGUI weaponText;
    
    public void UpdateHealth(int hp)
    {
        //healthText.text = "Health: " + hp;
        healthSlider.value = hp;
    }
    public void UpdateWeapon(int weapon)
    {
        switch (weapon)
        {
            case 0:
                weaponText.text = "Magic Missiles";
                break;
            case 1:
                weaponText.text = "Claymore";
                break;
        }
    }
    public void AddProgress(int progress)
    {
        progressSlider.value += progress;
    }
    public void AddEnergy(int energy)
    {
        energySlider.value += energy;
    }
}
