﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContextFilterDynamic : ScriptableObject
{
    // Compare a new transform against the original list of transforms.
    public abstract List<Transform> Filter(FlockAgentDynamic agent, List<Transform> original);
}
