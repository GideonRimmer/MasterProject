using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    // Setup parameters.
    public FlockAgent agentPrefab;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehavior behavior;

    // Generate number of agents.
    [Range(10, 500)]
    public int startingCount = 250;
    //const float AgentDensity = 1f;
    public float AgentDensity = 1f;

    // Agent parameters.
    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 10f)]
    public float neighbourRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    // Utility parameters.
    private float squareMaxSpeed;
    private float squareNeighbourRadius;
    private float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }
    private Vector3 newPosition;
    
    void Start()
    {
        // Use square parameters to enhance performance.
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighbourRadius = neighbourRadius * neighbourRadius;
        squareAvoidanceRadius = squareNeighbourRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        // Instantiate the flock.
        for (int i = 0; i < startingCount; i++)
        {
            
            // Create a circle with the correct alignment in 3D space: assign the y value of the 2D circle to the z axis.
            Vector3 spawnCircle = Random.insideUnitCircle * startingCount * AgentDensity;
            newPosition = new Vector3(spawnCircle.x, 0, spawnCircle.y) + transform.position;

            // Instantiate the agents inside the circle, with a random y rotation.
            FlockAgent newAgent = Instantiate(
                agentPrefab,
                newPosition,
                //Random.insideUnitCircle * startingCount * AgentDensity,
                Quaternion.Euler(Vector3.forward * Random.Range(0, 360f)),
                transform
                );
            newAgent.name = "Agent" + i;
            newAgent.Initialize(this);
            agents.Add(newAgent);
        }
    }

    void Update()
    {
        foreach (FlockAgent agent in agents)
        {
            // Make a list of everything in the context of the agent's radius.
            List<Transform> context = GetNearbyObjects(agent);

            // DEBUG, FOR DEMO ONLY: Change agent color based on how many agents are near it (0 = white, max 6 = red).
            agent.GetComponentInChildren<Renderer>().material.color = Color.Lerp(Color.white, Color.red, context.Count / 6f);

            // Calculate how the agent should move based on nearby objects.
            // The calculation is done in FlockBehavior.CalculateMove.
            Vector3 move = behavior.CalculateMove(agent, context, this);
            move *= driveFactor;

            // Cap the speed (bring it back to maxSpeed if greater).
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }
            agent.Move(move);
        }
    }

    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();

        // Get an array of all colliders in a the radius, using OverlapSphere.
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighbourRadius);
        //Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighbourRadius);

        foreach (Collider collider in contextColliders)
        {
            // Add all of the transforms of the colliders in the sphere, except this object's (agent's) transform.
            if (collider != agent.AgentCollider && collider.tag != "Ground")
            {
                context.Add(collider.transform);
            }
        }
        return context;
    }

    // Show overlapSpheres for every agent in scene view.
    private void OnDrawGizmos()
    {
        foreach (FlockAgent agent in agents)
        {
            Gizmos.DrawWireSphere(agent.transform.position, neighbourRadius);
        }
    }
}
