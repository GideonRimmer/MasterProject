using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior Dynamic/Alignment")]
public class AlignmentBehaviorDynamic : FilteredFlockBehaviorDynamic
{
    public override Vector3 CalculateMove(FlockAgentDynamic agent, List<Transform> context, FlockDynamic flock)
    {
        // If no neighbours, maintain current alignment.
        if (context.Count == 0)
            return agent.transform.forward;

        // Find the alignment of all neighbours and align agent according to their average alignment.
        // 1. Add all context points together.
        Vector3 alignmentMove = Vector3.zero;

        // Check if a filter is applied. If the filter is null, use normal context. If there is a filter, use the filter.
        List<Transform> filteredContex = (filter == null) ? context : filter.Filter(agent, context);

        foreach (Transform item in filteredContex)
        {
            alignmentMove += item.transform.forward;
        }
        // 2. Average all points.
        alignmentMove /= context.Count;

        // The alignment is independent of the position, so there is no need to create an offset.
        return alignmentMove;
    }
}
