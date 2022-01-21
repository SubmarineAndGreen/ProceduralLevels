using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {
    public Slider healthSlider;
    [SerializeField] private GameObject dashSlider;
    [HideInInspector] public Image dashSliderImage;
    public GameObject[] dashImage;
    public Image heartImageBar;
    public TextMeshProUGUI hpText;

    private void Awake() {
        dashSliderImage = dashSlider.GetComponent<Image>();
    }
}
