using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructStatus : MonoBehaviour
{
    [SerializeField] public UI_Display ui;
    [SerializeField] public int enemyMaxHP = 3;
    public int enemyCurrentHP;
    [SerializeField] public int damageOfEnemy;
    [SerializeField] public Transform player;
    [SerializeField] public GameObject projectile;
    public float projectileSpeed;
    public float fireRate = 1f;
    private float timeToFire;
    public bool isAbleToShoot;
    public float speed;
    [SerializeField] LayerMask layerMask;
    // Start is called before the first frame update

    void Start()
    {
        SetMaxHP();
        ui = GameObject.Find("Status").GetComponent<UI_Display>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) < 20)
            if (Physics.Linecast(transform.position, player.position, layerMask))
            {
                //Debug.Log("blocked");
            }
            else
            {
                //Debug.Log("targeting");
                if (isAbleToShoot && Time.time >= timeToFire)
                {
                    timeToFire = Time.time + 1 * fireRate;
                    InstantiateMagicMissile(transform);
                }
            }
    }

    public void TakeDamage(int damage)
    {
        enemyCurrentHP -= damage;
        Debug.Log("Enemy took " + damage + " damage");
        if (enemyCurrentHP <= 0)
        {
            ui.AddProgress(5);
            ui.AddEnergy(10);
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

    void InstantiateMagicMissile(Transform firePoint)
    {
        var projectileObj = Instantiate(projectile, firePoint.position, Quaternion.identity) as GameObject;
        projectileObj.GetComponent<Rigidbody>().velocity = (player.position - firePoint.position).normalized * projectileSpeed;
    }
}
