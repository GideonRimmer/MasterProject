using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Avoidance")]

public class AvoidanceBehavior : FlockBehavior
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // If no neighbours, don't adjust position.
        if (context.Count == 0)
            return Vector3.zero;

        // Find the middle point of all items to avoid, and move away from them.
        Vector3 avoidanceMove = Vector3.zero;

        // Create an int for number of things to avoid.
        int nAvoid = 0;
        foreach (Transform item in context)
        {
            // Get the distance between the agent and the item to avoid.
            // If item is in avoidance distance (distance to item < distance to avoid), add to items to avoid..
            if (Vector3.SqrMagnitude(item.position - agent.transform.position) < flock.SquareAvoidanceRadius)
            {
                nAvoid++;
                // Calculate the offset and move away from the item.
                avoidanceMove += agent.transform.position - item.position;
            }
        }

        if (nAvoid > 0)
        {
            avoidanceMove /= nAvoid;
        }

        return avoidanceMove;
    }
}
