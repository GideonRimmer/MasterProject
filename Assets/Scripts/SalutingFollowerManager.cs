﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalutingFollowerManager : MonoBehaviour
{
    [Header("Setup")]
    public GameObject player;
    public Animator animator;
    [SerializeField] private bool isSaluting;
    public float delayTime = 0.3f;
    private float delayCurrentTime;

    [Header("Overlap sphere")]
    public float sphereRadius = 5;
    [SerializeField] Collider[] agentsInSphere;
    [SerializeField] private float distanceToLeader;
    public LayerMask SaluteToLayers;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        isSaluting = false;
        animator.SetBool("isSaluting", false);
        delayCurrentTime = delayTime;
    }

    void FixedUpdate()
    {
        agentsInSphere = Physics.OverlapSphere(this.transform.position, sphereRadius, SaluteToLayers);

        // Get all of the agents in the sphere in each FixedUpdate.
        foreach (Collider agent in agentsInSphere)
        {
            if (agent.CompareTag("Player") ||
                (agent.CompareTag("FakeFollower") && agent.GetComponentInParent<SalutingFollowerManager>() != null && agent.GetComponentInParent<SalutingFollowerManager>().isSaluting == true))
            {
                delayCurrentTime -= Time.deltaTime;
                if (delayCurrentTime <= 0)
                {
                    isSaluting = true;
                    animator.SetBool("isSaluting", true);
                    delayCurrentTime = delayTime;
                }
            }
        }
    }
}
