using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NavigationVisualisationNode : MonoBehaviour
{
    MeshRenderer meshRenderer;
    TextMeshPro distanceText;
    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        distanceText = GetComponentInChildren<TextMeshPro>();
    }
    public void setColor(Color color) {
        meshRenderer.material.color = color;
    }
    public void setText(string text) {
        distanceText.text = text;
    }
}
