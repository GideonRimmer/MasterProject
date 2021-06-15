using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitPointsManager : MonoBehaviour
{
    public HealthBar healthBar;
    public bool useThisScriptToDestroy;
    public TextMeshProUGUI hitPointsText;
    public int maxHitPoints = 10;
    public int currentHitPoints;
    private PlayParticleEffect deathEffect;
    private Camera mainCamera;
    public TextMeshProUGUI entityName;
    //public AudioClip deathSound;

    void Start()
    {
        currentHitPoints = maxHitPoints;
        deathEffect = GetComponent<PlayParticleEffect>();
        mainCamera = Camera.main;

        // DEBUG: Show entity name.
        if (entityName != null)
        {
            entityName.text = this.name.ToString();
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHitPoints);
        }
    }

    private void Update()
    {
        // DEBUG: Show HP text in game.
        if (hitPointsText != null)
        {
            hitPointsText.text = currentHitPoints.ToString();
            hitPointsText.transform.LookAt(mainCamera.transform);
            hitPointsText.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        }

        // DEBUG: Show entity name.
        if (entityName != null)
        {
            entityName.transform.LookAt(mainCamera.transform);
            entityName.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        }
    }

        public void RegisterHit(int damage)
    {
        currentHitPoints -= damage;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHitPoints);
        }

        if (currentHitPoints <= 0 && useThisScriptToDestroy == true)
        {
            Die();
        }
    }

    public void Die()
    {
        //Debug.Log(this.name + " died.");
        //AudioSource.PlayClipAtPoint(deathSound, transform.position);

        if (deathEffect != null)
        {
            deathEffect.PlayParticleSystem();
        }
        Destroy(this.gameObject);
    }

    public void PlayParticleSystem()
    {
        deathEffect.PlayParticleSystem();
    }
}
