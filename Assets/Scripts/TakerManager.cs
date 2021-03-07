﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakerManager : MonoBehaviour
{
    public Animator animator;
    private Rigidbody rigidbody;
    public int minStartingCharisma;
    public int maxStartingCharisma;
    [SerializeField] private int startingCharisma;

    public float moveSpeed = 5.0f;
    public float rotateSpeed = 5.0f;
    public float roamingRangeMin = 10.0f;
    public float roamingRangeMax = 30.0f;

    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private Vector3 roamingPosition;

    private HitPointsManager hitPointsManager;
    private PlayParticleEffect playParticleEffect;

    void Awake()
    {
        startingCharisma = Random.Range(minStartingCharisma, maxStartingCharisma);
        GetComponent<SphereOfInfluence>().startingCharisma = startingCharisma;
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        startingPosition = transform.position;
        roamingPosition = GetRoamingPosition();

        hitPointsManager = GetComponent<HitPointsManager>();
        playParticleEffect = GetComponent<PlayParticleEffect>();
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, roamingPosition, moveSpeed * Time.deltaTime);
        // Auto rotate towards the target.
        Vector3 targetDirection = roamingPosition - transform.position;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
        //Debug.DrawRay(transform.position, newDirection, Color.red);

        // Move position a step towards to the target.
        transform.rotation = Quaternion.LookRotation(newDirection);
        animator.SetBool("isWalking", true);

        // If reached roaming position, set new target position.
        if (Vector3.Distance(transform.position, roamingPosition) < 0.5f)
        {
            // Reached roaming position.
            roamingPosition = GetRoamingPosition();
        }

        if (hitPointsManager.currentHitPoints <= 0)
        {
            Die();
        }
    }

    private Vector3 GetRoamingPosition()
    {
        //Debug.Log(this.name + " start: " + transform.position + ", target: " + roamingPosition);
        return startingPosition + GetRandomDirection() * Random.Range(roamingRangeMin, roamingRangeMax);
    }

    private static Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "Ground")
        {
            roamingPosition = GetRoamingPosition();
        }
    }

    private void Die()
    {
        playParticleEffect.PlayParticleSystem();
        Destroy(this.gameObject);
    }
}
