using System.Collections;
using System.Collections.Generic;
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

    public void SetTarget(Transform newTarget)
    {
        if (currentTarget == null || newTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma > currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma)
        {
            currentTarget = newTarget;
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
