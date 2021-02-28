using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPointsManager : MonoBehaviour
{
    public int maxHitPoints = 10;
    public int currentHitPoints;
    private PlayParticleEffect deathEffect;

    void Start()
    {
        currentHitPoints = maxHitPoints;
        deathEffect = GetComponent<PlayParticleEffect>();
    }

    public void RegisterHit(int damage)
    {
        currentHitPoints -= damage;

        /*
        if (currentHitPoints <= 0)
        {
            Die();
        }
        */
    }

    /*
    public void Die()
    {
        Debug.Log(this.name + " died.");
        if (deathEffect != null)
        {
            deathEffect.PlayParticleSystem();
        }
        Destroy(this.gameObject);
    }
    */

    public void PlayParticleSystem()
    {
        deathEffect.PlayParticleSystem();
    }
}
