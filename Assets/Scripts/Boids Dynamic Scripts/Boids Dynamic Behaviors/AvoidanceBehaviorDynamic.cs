using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior Dynamic/Avoidance")]
public class AvoidanceBehaviorDynamic : FilteredFlockBehaviorDynamic
{
    public override Vector3 CalculateMove(FlockAgentDynamic agent, List<Transform> context, FlockDynamic flock)
    {
        // If no neighbours, don't adjust position.
        if (context.Count == 0)
            return Vector3.zero;

        // Find the middle point of all items to avoid, and move away from them.
        Vector3 avoidanceMove = Vector3.zero;

        // Create an int for number of things to avoid.
        int nAvoid = 0;

        // Check if a filter is applied. If the filter is null, use normal context. If there is a filter, use the filter.
        List<Transform> filteredContex = (filter == null) ? context : filter.Filter(agent, context);

        foreach (Transform item in filteredContex)
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
