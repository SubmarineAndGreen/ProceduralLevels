using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InputManager;

public class GunController : MonoBehaviour
{
    public Camera cam;
    public GameObject projectile;
    public Transform leftHandFirePoint, rightHandFirePoint;
    public float projectileSpeed = 30;
    public float fireRate = 4;

    private Vector3 destination;
    private bool rightHand=true;
    private float timeToFire;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        if (inputs.Player.Shoot.ReadValue<float>()==1 && Time.time >=timeToFire)
        {
            timeToFire = Time.time + 1/fireRate;
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
            destination = hit.point;
        else
            destination = ray.GetPoint(1000);

        if(rightHand)
        {
            rightHand = false;
            InstantiateProjectile(rightHandFirePoint);
        }
        else
        {
            rightHand = true;
            InstantiateProjectile(leftHandFirePoint);
        }
    }

    void InstantiateProjectile(Transform firePoint)
    {
        var projectileObj = Instantiate(projectile, firePoint.position, Quaternion.identity) as GameObject;
        projectileObj.GetComponent<Rigidbody>().velocity = (destination - firePoint.position).normalized * projectileSpeed;
    }
}
