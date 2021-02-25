using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlockAgentDynamic : MonoBehaviour
{
    [SerializeField] private FlockDynamic agentFlock;
    public FlockDynamic AgentFlock { get { return agentFlock; } }

    Collider agentCollider;
    public Collider AgentCollider { get { return agentCollider; } }

    // Test: Manually decide which flock will be Dynamic on click.
    public FlockDynamic newFlock;

    void Start()
    {
        agentCollider = GetComponent<Collider>();
    }

    /*
    // When the flock creates the agent, link the agent to the flock.
    public void Initialize(FlockDynamic flock)
    {
        agentFlock = flock;
    }
    */

    // Assign this agent to a new flock.
    public void AssignToFlock(FlockDynamic flock)
    {
        agentFlock = flock;
        flock.AddAgent(this);
    }

    private void OnMouseDown()
    {
        AssignToFlock(newFlock);
    }

    public void Move(Vector3 velocity)
    {
        // Set the flocking agent's direction.
        transform.forward = velocity;

        // Move the flocking agent in the direction.
        transform.position += velocity * Time.deltaTime;
    }
}
