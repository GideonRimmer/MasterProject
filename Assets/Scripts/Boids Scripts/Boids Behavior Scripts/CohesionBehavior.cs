using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Cohesion")]
public class CohesionBehavior : FilteredFlockBehavior
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // If no neighbours, don't adjust position.
        if (context.Count == 0)
            return Vector2.zero;

        // Find the middle point of all neighbours, and move there.
        // 1. Add all context points together.
        Vector3 cohesionMove = Vector3.zero;

        // Check if a filter is applied. If the filter is null, use normal context. If there is a filter, use the filter.
        List<Transform> filteredContex = (filter == null) ? context : filter.Filter(agent, context);

        foreach (Transform item in filteredContex)
        {
            cohesionMove += item.position;
        }
        // 2. Average all points.
        cohesionMove /= context.Count;

        // 3. Calculate the offset from the agent's position.
        cohesionMove -= agent.transform.position;
        return cohesionMove;
    }
}
