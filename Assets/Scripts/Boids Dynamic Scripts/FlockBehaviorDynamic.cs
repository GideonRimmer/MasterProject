using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlockBehaviorDynamic : ScriptableObject
{
    public abstract Vector3 CalculateMove(FlockAgentDynamic agent, List<Transform> context, FlockDynamic flock);
}
