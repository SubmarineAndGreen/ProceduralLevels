using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    private Animator anim;
    Collider swordCollider;
    // Start is called before the first frame update
    void Start()
    {
        swordCollider = GetComponent<Collider>();
        anim = gameObject.GetComponent<Animator>();
        Debug.Log(anim);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwingStart()
    {
        swordCollider.enabled = true;
        Debug.Log("start swing");
    }

    public void SwingStop()
    {
        swordCollider.enabled = false;
        Debug.Log("stop swing");
    }

    public void SwordAttack(bool side)
    {
        if (side)
        {
            //anim.GetCurrentAnimatorStateInfo(0).IsName
            anim.Play("Swings.swingleft");
        }
        else
        {
            anim.Play("Swings.swingright");
        }
    }

    public GameObject impactVFX;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Entered collision");
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Enemy hit with a sword!");
            collision.gameObject.GetComponent<EnemyStatus>().TakeDamage(5);
            var impact = Instantiate(impactVFX, collision.contacts[0].point, Quaternion.identity) as GameObject;
            Destroy(impact, 2);
        }
        if (collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "Player")
        {
            Debug.Log("Something hit with a sword!");
            var impact = Instantiate(impactVFX, collision.contacts[0].point, Quaternion.identity) as GameObject;
            Destroy(impact, 2);
        }
    }
}
