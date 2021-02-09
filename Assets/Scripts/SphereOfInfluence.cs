using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereOfInfluence : MonoBehaviour
{
    public int startingCharisma = 10;
    public int currentCharisma;

    public float sphereInitialRadius = 10f;
    public float sphereCurrentRadius;

    Collider[] followersInSphere;

    private void Start()
    {
        currentCharisma = startingCharisma;
        sphereCurrentRadius = sphereInitialRadius;
    }

    private void FixedUpdate()
    {
        // Detect when a collider enters the sphere.
        // Note that this happens in every frame for evert collider (including terrain!).
        followersInSphere = Physics.OverlapSphere(this.transform.position, sphereCurrentRadius);

        foreach(var follower in followersInSphere)
        {
            if (follower.tag == "Follower")
            {
                //Debug.Log(follower.name + " entered sphere.");
                follower.GetComponentInParent<MoveToTarget>().SetTarget(this.transform);
            }
        }
    }

    // Show sphere radius in scene view.
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, sphereCurrentRadius);
    }
}
