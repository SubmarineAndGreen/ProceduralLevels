using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Door : MonoBehaviour
{
    [SerializeField] private TextMeshPro doorNumberText;

    public void setDoorNumber(int number) {
        doorNumberText.text = number.ToString();
    }
}
