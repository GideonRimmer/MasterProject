using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    public Transform currentTarget;
    public float moveSpeed;
    public float minDistanceToTarget;
    public float maxDistanceToTarget;
    public float currentDistanceToTarget;

    public bool isAttacking;

    void Start()
    {
        isAttacking = false;
    }

    void Update()
    {
        if (currentTarget != null && GetComponent<OnClickInteraction>().isFollowing == true)
        {
            FollowTarget();
        }
    }

    // Acquire a new target to follow.
    public void SetTarget(Transform newTarget)
    {
        if (currentTarget == null || newTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma > currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma)
        {
            // If switched targets, register that the old target lost a follower.
            if (newTarget != currentTarget && currentTarget != null)
            {
                currentTarget.GetComponent<SphereOfInfluence>().LoseFollower();
            }

            // Assign new target to follow.
            currentTarget = newTarget;
            //Debug.Log(this.name + "is following" + newTarget.name);
            if (newTarget.tag != "Player")
            {
                newTarget.GetComponent<SphereOfInfluence>().GainFollower();
            }
        }
    }

    private void FollowTarget()
    {
        if (Vector3.Distance(transform.position, currentTarget.transform.position) < maxDistanceToTarget && Vector3.Distance(transform.position, currentTarget.transform.position) > minDistanceToTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, moveSpeed * Time.deltaTime);
        }
    }
}
