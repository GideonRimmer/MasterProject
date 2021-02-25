using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    Flock agentFlock;
    public Flock AgentFlock { get { return agentFlock; } }

    Collider agentCollider;
    public Collider AgentCollider { get { return agentCollider; } }

    void Start()
    {
        agentCollider = GetComponent<Collider>();
    }

    // When the flock creates the agent, link the agent to the flock.
    public void Initialize(Flock flock)
    {
        agentFlock = flock;
    }

    public void Move(Vector3 velocity)
    {
        // Set the flocking agent's direction.
        transform.forward = velocity;

        // Move the flocking agent in the direction.
        transform.position += velocity * Time.deltaTime;
    }
}
