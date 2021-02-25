using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Stay In Radius")]
public class StayInRadiusBehavior : FlockBehavior
{
    public Vector3 center;
    public float circleRadius = 15f;

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // Calculate the position of the agent in relation to the cneter.
        Vector3 centerOffset = center - agent.transform.position;

        // t is the magnitude of distance from the center.
        // t = 0 -> agent is at the center, t = 1 -> agent is at the radius parameter.
        float t = centerOffset.magnitude / circleRadius;

        // If not close to the radiu edge, continue as normal.
        if (t < 0.9f)
        {
            return Vector3.zero;
        }

        // If close to the radius edge, start moving towards the center.
        return centerOffset * t * t;
    }
}
