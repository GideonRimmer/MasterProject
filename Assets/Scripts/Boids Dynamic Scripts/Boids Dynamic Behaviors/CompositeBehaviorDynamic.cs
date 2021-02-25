using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior Dynamic/Composite")]
public class CompositeBehaviorDynamic : FlockBehaviorDynamic
{
    public FlockBehaviorDynamic[] behaviors;
    public float[] weights;

    public override Vector3 CalculateMove(FlockAgentDynamic agent, List<Transform> context, FlockDynamic flock)
    {
        // Throw an error if the number of behaviors doesn't match the number of weights, and don't move the agent.
        if (weights.Length != behaviors.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector3.zero;
        }

        // Set up the combined move of all behaviors.
        Vector3 move = Vector3.zero;

        // Iterate through behaviors. Use a 'for' loop and not 'foreach' to match the index of the bahavior to the index of the weight.
        for (int i = 0; i < behaviors.Length; i++)
        {
            // Call on the CalculateMove of each behavior based on its weight.
            Vector3 partialMove = behaviors[i].CalculateMove(agent, context, flock) * weights[i];

            // Confirm that the partial move is limited to the extent of the weight.
            // If there is some movement...
            if (partialMove != Vector3.zero)
            {
                // If the partial movement exceeds the weight...
                if (partialMove.sqrMagnitude > weights[i] * weights[i])
                {
                    // Then normalize the partial move to a magnitude of one and multiply by the weight, to set it to the max of the weight.
                    partialMove.Normalize();
                    partialMove *= weights[i];
                }

                // If the partial move doesn't exceed the weight, conduct the partial move.
                move += partialMove;
            }
        }

        return move;
    }
}
