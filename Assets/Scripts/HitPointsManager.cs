using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPointsManager : MonoBehaviour
{
    [SerializeField] private int maxHitPoints = 10;
    [SerializeField] private int currentHitPoints;

    void Start()
    {
        currentHitPoints = maxHitPoints;
    }

    public void RegisterHit(int damage)
    {
        currentHitPoints -= damage;

        if (currentHitPoints <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log(this.name + " died.");
        Destroy(this.gameObject);
    }
}
