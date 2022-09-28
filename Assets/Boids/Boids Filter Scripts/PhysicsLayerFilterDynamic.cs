using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Filter Dynamic/Physics Layer")]
public class PhysicsLayerFilterDynamic : ContextFilterDynamic
{
    public LayerMask mask;

    public override List<Transform> Filter(FlockAgentDynamic agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();

        // Check if a transform is on the filtered physics layer.
        foreach (Transform item in original)
        {
            // If this is true, then the item we're checking is on the same layer as the mask is checking.
            if (mask == (mask | (1 << item.gameObject.layer)))
            {
                filtered.Add(item);
            }
        }
        return filtered;
    }
}
