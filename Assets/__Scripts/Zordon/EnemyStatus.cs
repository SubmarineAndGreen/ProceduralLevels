using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    private int enemy;
    [SerializeField] ConstructStatus constructStatus;
    [SerializeField] SpikeBoiStatus spikeBoiStatus;
    [SerializeField] Floater floaterStatus;

    private void Start()
    {
        if (floaterStatus ?? false)
        {
            enemy = 2;
        }
        if (constructStatus ?? false)
        {
            enemy = 1;
        }
        if (spikeBoiStatus ?? false)
        {
            enemy = 0;
        }
    }

    public void TakeDamage(int damage)
    {
        switch (enemy)
        {
            case 0:
                spikeBoiStatus.TakeDamage(damage);
                break;
            case 1:
                constructStatus.TakeDamage(damage);
                break;
            case 2:
                floaterStatus.TakeDamage(damage);
                break;
        }
    }
}
