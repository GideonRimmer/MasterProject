using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class InnocentManager : MonoBehaviour
{
    //private Rigidbody rigidbody;
    public NavMeshAgent navMeshAgent;
    public float moveSpeed = 4f;
    public float rotateSpeed = 8f;
    [SerializeField] private float runAwaySpeed;
    public float sphereRadius = 10f;
    public float minDistanceFromHostile = 10f;
    public float maxDistanceFromHostile = 15f;
    [SerializeField] private float currentDistanceFromHostile;

    private enum State
    {
        Idle,
        RunAway,
    }
    [SerializeField] private State currentState;
    [SerializeField] Collider[] agentsInSphere;
    public Transform currentHostile;
    public LayerMask hostileLayers;

    private void Start()
    {
        //rigidbody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentState = State.Idle;
        currentDistanceFromHostile = maxDistanceFromHostile;
        runAwaySpeed = moveSpeed + 1f;
    }

    private void FixedUpdate()
    {
        agentsInSphere = Physics.OverlapSphere(this.transform.position, sphereRadius, hostileLayers);

        // Get all of the agents in the sphere in each FixedUpdate.
        foreach (Collider agent in agentsInSphere)
        {
            //Debug.Log(agent.name + " is in sphere.");
            currentHostile = agent.transform;
            currentState = State.RunAway;
        }
    }

    void Update()
    {
        // State machine.
        switch (currentState)
        {
            default:
            case State.Idle:
                break;

            case State.RunAway:
                if (currentHostile != null)
                {
                    currentDistanceFromHostile = Vector3.Distance(currentHostile.transform.position, transform.position);
                    if (currentDistanceFromHostile < maxDistanceFromHostile)
                    {
                        RunAway(currentHostile);
                    }
                    else if (currentDistanceFromHostile >= maxDistanceFromHostile)
                    {
                        currentHostile = null;
                    }
                }
                // If there is no target, or if the target is out of range, stop moving and reset target.
                if (currentHostile == null)
                {
                    //currentHostile = null;
                    navMeshAgent.destination = transform.position;
                    currentState = State.Idle;
                }
                break;
        }

        if (GetComponent<HitPointsManager>().currentHitPoints <= 0)
        {
            Die();
        }
    }

    private void RunAway(Transform hostile)
    {
        /*
        currentDistanceFromTarget = Vector3.Distance(transform.position, target.position);
        Vector3 direction = (currentTarget.position - rigidbody.transform.position).normalized;
        rigidbody.MovePosition(rigidbody.transform.position + direction * moveSpeed * -1 * Time.fixedDeltaTime);


        // Auto rotate towards the target.
        Vector3 targetDirection = target.position - transform.position;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
        //Debug.DrawRay(transform.position, newDirection, Color.red);
        transform.rotation = Quaternion.LookRotation(newDirection);
        */

        currentDistanceFromHostile = Vector3.Distance(hostile.transform.position, transform.position);
        // Run away from a hostile if it gets in range.
        if (currentDistanceFromHostile <= maxDistanceFromHostile)
        {
            Vector3 directionToHostile = transform.position - hostile.position;
            Vector3 newPosition = transform.position + directionToHostile;
            navMeshAgent.SetDestination(newPosition);

            //animator.SetBool("isWalking", false);

            //Debug.Log(newPosition);
            //Debug.Log(currentDistanceFromHostile);
            //Debug.Log(this.name + " runs away.");
        }
    }

    public void Die()
    {
        GetComponent<HitPointsManager>().PlayParticleSystem();
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}
