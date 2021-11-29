using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InputManagerGameplay;

public class WeaponsController : MonoBehaviour
{
    [SerializeField] GameObject sword;
    public SwordController swordController;

    public Camera cam;
    public GameObject projectile;
    public GameObject leftHandObject, rightHandObject;
    public Transform leftHandFirePoint, rightHandFirePoint;
    public float projectileSpeed = 30;
    public float fireRate0 = 0.25f;
    public float fireRate1 = 1f;

    private Vector3 destination;
    private bool rightHand = true;
    private float timeToFire;
    private int weapon;
    //0-magic missiles, 1-sword
    private int maxWeapons=2;

    void Start() {
        weapon = 0;
        swordController = sword.GetComponent<SwordController>();
        sword.SetActive(false);
    }

    void Update()
    {
        if (inputGameplay.Player.Shoot.ReadValue<float>() == 1 && Time.time >= timeToFire) {
            switch (weapon)
            {
                case 0:
                    timeToFire = Time.time + 1 * fireRate0;
                    ShootMagicMissile();
                    break;
                case 1:
                    timeToFire = Time.time + 1 * fireRate1;
                    swingSword();
                    break;
            }
        }
        if(inputGameplay.Player.ChangeWeaponUP.ReadValue<float>() == 1) {
            ChangeWeapon(true);
        }
        if (inputGameplay.Player.ChangeWeaponDOWN.ReadValue<float>() == 1)
        {
            ChangeWeapon(false);
        }
    }

    private void ChangeWeapon(bool change) {
        rightHand = true;
        if(change) {
            weapon++;
        }
        else {
            weapon--;
        }
        if (weapon>=maxWeapons) {
            weapon = 0;
        }
        if(weapon<0) {
            weapon = maxWeapons;
        }
        if(weapon == 0)
        {
            sword.SetActive(false);
            leftHandObject.SetActive(true);
            rightHandObject.SetActive(true);
        }
        if(weapon == 1)
        {
            sword.SetActive(true);
            leftHandObject.SetActive(false);
            rightHandObject.SetActive(false);
        }
        
    }

    void ShootMagicMissile() {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
            destination = hit.point;
        else
            destination = ray.GetPoint(1000);

        if (rightHand) {
            rightHand = false;
            InstantiateMagicMissile(rightHandFirePoint);
        }
        else {
            rightHand = true;
            InstantiateMagicMissile(leftHandFirePoint);
        }
    }

    void swingSword()
    {
        if (rightHand)
        {
            rightHand = false;
            swordController.SwordAttack(true);
        }
        else
        {
            rightHand = true;
            swordController.SwordAttack(false);
        }
    }

    void InstantiateMagicMissile(Transform firePoint) {
        var projectileObj = Instantiate(projectile, firePoint.position, Quaternion.identity) as GameObject;
        projectileObj.GetComponent<Rigidbody>().velocity = (destination - firePoint.position).normalized * projectileSpeed;
    }
}