using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileController : MonoBehaviour
{
    public GameObject impactVFX;
    private bool collided;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Enemy" && collision.gameObject.tag == "Player" && !collided)
        {
            Debug.Log("Player hit!");
            collided = true;
            collision.gameObject.GetComponent<PlayerStatus>().TakeDamage(2);
            var impact = Instantiate(impactVFX, collision.contacts[0].point, Quaternion.identity) as GameObject;
            Destroy(impact, 2);
            Destroy(gameObject);
        }
        if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Enemy" && !collided)
        {
            collided = true;
            var impact = Instantiate(impactVFX, collision.contacts[0].point, Quaternion.identity) as GameObject;
            Destroy(impact, 2);
            Destroy(gameObject);
        }
    }
}
