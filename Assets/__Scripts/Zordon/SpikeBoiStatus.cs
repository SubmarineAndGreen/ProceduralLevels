using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBoiStatus : MonoBehaviour
{
    [SerializeField] public UI_Display ui;
    [SerializeField] public int enemyMaxHP = 3;
    public int enemyCurrentHP;
    [SerializeField] public int damageOfEnemy;
    private Transform player;
    [SerializeField] public GameObject projectile;
    public float projectileSpeed;
    public float fireRate = 1f;
    private float timeToFire;
    public bool isAbleToShoot;
    public float speed;
    [SerializeField] LayerMask layerMask;
    private Rigidbody rb;
    // Start is called before the first frame update

    void Start()
    {
        SetMaxHP();
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
        ui = GameObject.Find("Status").GetComponent<UI_Display>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) < 20)
        //if (Physics.Linecast(transform.position, player.position, layerMask))
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, (player.position - transform.position), out hit, 20, layerMask))
            {
                
                    Debug.Log("MOVING");
                    transform.LookAt(player.position);
                    rb.AddForce(transform.forward, ForceMode.Force);
                
                
            }
        }
    }

    public void TakeDamage(int damage)
    {
        enemyCurrentHP -= damage;
        Debug.Log("Enemy took " + damage + " damage");
        if (enemyCurrentHP <= 0)
        {
            //ui.AddProgress(1);
            //ui.AddEnergy(10);
            Destroy(gameObject);
        }
    }

    public void SetMaxHP()
    {
        enemyCurrentHP = enemyMaxHP;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("test");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Took " + damageOfEnemy + " damage!");
            collision.gameObject.GetComponent<PlayerStatus>().TakeDamage(1);
        }
    }
}
