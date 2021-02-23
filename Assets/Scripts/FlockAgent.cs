using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    Collider agentCollider;
    public Collider AgentCollider { get { return agentCollider; } }

    void Start()
    {
        agentCollider = GetComponent<Collider>();
    }

    public void Move(Vector3 velocity)
    {
        // Set the flocking agent's direction.
        transform.forward = velocity;

        // Move the flocking agent in the direction.
        transform.position += velocity * Time.deltaTime;
    }
}
