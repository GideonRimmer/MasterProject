﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitPointsManager : MonoBehaviour
{
    public bool useThisScriptToDestroy;
    public TextMeshProUGUI hitPointsText;
    public int maxHitPoints = 10;
    public int currentHitPoints;
    private PlayParticleEffect deathEffect;
    private Camera mainCamera;

    void Start()
    {
        currentHitPoints = maxHitPoints;
        deathEffect = GetComponent<PlayParticleEffect>();
        mainCamera = Camera.main;
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
    }

    public void RegisterHit(int damage)
    {
        currentHitPoints -= damage;

        if (currentHitPoints <= 0 && useThisScriptToDestroy == true)
        {
            Die();
        }
    }

    public void Die()
    {
        //Debug.Log(this.name + " died.");
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
