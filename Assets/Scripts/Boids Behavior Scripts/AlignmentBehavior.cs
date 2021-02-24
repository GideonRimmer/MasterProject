using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Alignment")]

public class AlignmentBehavior : FlockBehavior
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // If no neighbours, maintain current alignment.
        if (context.Count == 0)
            return agent.transform.forward;

        // Find the alignment of all neighbours and align agent according to their average alignment.
        // 1. Add all context points together.
        Vector3 alignmentMove = Vector3.zero;
        foreach (Transform item in context)
        {
            alignmentMove += item.transform.forward;
        }
        // 2. Average all points.
        alignmentMove /= context.Count;

        // The alignment is independent of the position, so there is no need to create an offset.
        return alignmentMove;
    }
}
