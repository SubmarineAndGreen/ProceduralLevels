using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonTextHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Color defaultColor = Color.white;
    Color hoverColor = Color.yellow;
    TextMeshProUGUI buttonText;

    public void OnPointerEnter(PointerEventData eventData) {
        buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData) {
        buttonText.color = defaultColor;
    }

    private void Awake() {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }
}
