using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InnocentManager : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float rotateSpeed = 8f;
    [SerializeField] private float runAwaySpeed;
    public float sphereRadius = 10f;
    public float maxDistanceFromTarget = 15f;
    [SerializeField] private float currentDistanceFromTarget = 15f;

    private enum State
    {
        Idle,
        RunAway,
    }
    [SerializeField] private State currentState;
    [SerializeField] Collider[] agentsInSphere;
    public Transform currentTarget;

    private void Start()
    {
        currentState = State.Idle;
        runAwaySpeed = moveSpeed + 1f;
    }

    private void FixedUpdate()
    {
        LayerMask layerMask = LayerMask.GetMask("Characters");
        agentsInSphere = Physics.OverlapSphere(this.transform.position, sphereRadius, layerMask);

        // Get all of the agents in the sphere in each FixedUpdate.
        foreach (Collider agent in agentsInSphere)
        {
            if (agent.tag == "Follower")
            {
                currentTarget = agent.transform;
                currentState = State.RunAway;
            }
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
                if (currentTarget != null)
                {
                    RunAway(currentTarget);
                }
                // If there is no target, or if the target is out of range, stop moving and reset target.
                if (currentTarget == null || currentDistanceFromTarget > maxDistanceFromTarget)
                {
                    currentTarget = null;
                    currentState = State.Idle;
                }
                break;

        }

        if (GetComponent<HitPointsManager>().currentHitPoints <= 0)
        {
            Die();
        }
    }

    private void RunAway(Transform target)
    {
        currentDistanceFromTarget = Vector3.Distance(transform.position, target.position);
        transform.position = Vector3.MoveTowards(transform.position, target.position, runAwaySpeed * -1 * Time.deltaTime);

        // Auto rotate towards the target.
        Vector3 targetDirection = target.position - transform.position;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
        //Debug.DrawRay(transform.position, newDirection, Color.red);
        transform.rotation = Quaternion.LookRotation(newDirection);
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
