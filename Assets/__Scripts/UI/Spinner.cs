using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spinner : MonoBehaviour
{
    [SerializeField] Transform spinner;
    [SerializeField] float angularVelocity;
    private void Update() {
        float rotation = spinner.rotation.eulerAngles.z;
        spinner.rotation = Quaternion.Euler(0, 0, rotation + angularVelocity * Time.deltaTime);
    }
}
