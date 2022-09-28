using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Filter/Same Flock")]
public class SameFlockFilter : ContextFilter
{
    public override List<Transform> Filter(FlockAgent agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();

        // Iterate through the original list, and determine if a new transform has the FlockAgent component AND is of the same flock.
        // If both are true, add to the list of filtered transforms.
        foreach (Transform item in original)
        {
            FlockAgent itemAgent = item.GetComponent<FlockAgent>();
            if (itemAgent != null && itemAgent.AgentFlock == agent.AgentFlock)
            {
                filtered.Add(item);
            }
        }
        return filtered;
    }
}
