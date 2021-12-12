using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileContoller : MonoBehaviour
{
    public GameObject impactVFX;
    private bool collided;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="Enemy" && !collided)
        {
            Debug.Log("Enemy hit!");
            collided = true;
            collision.gameObject.GetComponent<EnemyStatus>().TakeDamage(1);
            var impact = Instantiate(impactVFX, collision.contacts[0].point, Quaternion.identity) as GameObject;
            Destroy(impact, 2);
            Destroy(gameObject);
        }
        if(collision.gameObject.tag != "Weapons" && collision.gameObject.tag!="Bullet" && collision.gameObject.tag!="Player" && !collided)
        {
            collided = true;
            var impact = Instantiate(impactVFX, collision.contacts[0].point, Quaternion.identity) as GameObject;
            Destroy(impact, 2);
            Destroy(gameObject);
        }
    }
}
