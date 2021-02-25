using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContextFilter : ScriptableObject
{
    // Compare a new transform against the original list of transforms.
    public abstract List<Transform> Filter(FlockAgent agent, List<Transform> original);
}
