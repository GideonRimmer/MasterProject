using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditorInternal;
using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    public SphereCollider sphereCollider;
    private float sphereInitialRadius = 10f;
    private float sphereCurrentRadius;

    private enum State
    {
        Idle,
        Follow,
        Attack,
    }
    [SerializeField] private State currentState;

    public Transform currentTarget;
    public float moveSpeed;
    public float minDistanceToTarget;
    public float maxDistanceToTarget;
    public float currentDistanceToTarget;

    public bool isAttacking;
    public int attackDamage = 1;
    public Transform enemyTarget;
    [SerializeField] private float distanceToEnemy;

    void Start()
    {
        //sphereCollider.GetComponentInChildren<SphereCollider>();
        sphereCurrentRadius = sphereInitialRadius;

        currentState = State.Idle;
        //isAttacking = false;
    }

    void Update()
    {
        switch (currentState)
        {
            default:
            case State.Idle:

                break;

            case State.Follow:
                if (currentTarget != null && GetComponent<OnClickInteraction>().isFollowing == true)
                {
                    FollowTarget();
                }
                break;

            case State.Attack:
                if (enemyTarget != null)
                {
                    FollowAndAttackTarget();
                }
                else if (currentTarget != null)
                {
                    currentState = State.Follow;
                }
                break;
        }
    }

    // Acquire a new target to follow.
    public void SetTarget(Transform newTarget)
    {
        if (currentTarget == null || newTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma > currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma)
        {
            currentState = State.Follow;
            // If switched targets, register that the old target lost a follower.
            if (newTarget != currentTarget && currentTarget != null)
            {
                currentTarget.GetComponent<SphereOfInfluence>().LoseFollower(this.gameObject);
            }

            // Assign new target to follow.
            currentTarget = newTarget;
            //Debug.Log(this.name + "is following" + newTarget.name);
            if (newTarget.tag != "Player")
            {
                newTarget.GetComponent<SphereOfInfluence>().GainFollower(this.gameObject);
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

    public void SetAttackTarget(Transform newEnemy)
    {
        currentState = State.Attack;
        enemyTarget = newEnemy;
    }

    private void FollowAndAttackTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, enemyTarget.transform.position, moveSpeed * Time.deltaTime);
        distanceToEnemy = Vector3.Distance(transform.position, enemyTarget.transform.position);

        if (enemyTarget == null)
        {
            Debug.Log("Target destroyed");
            currentState = State.Follow;
        }
    }

    // Detect enemy in sphere
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.tag == "Enemy")
        if (other.gameObject.tag == "Taker" && this.GetComponent<LoyaltyManager>().currentLoyalty > other.GetComponentInParent<SphereOfInfluence>().currentCharisma)
        {
            SetAttackTarget(other.gameObject.GetComponentInParent<Transform>());
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        // On collision with enemy, inflict damage.
        if (collision.collider.tag == "Taker" && currentState == State.Attack)
        {
            collision.gameObject.GetComponent<HitPointsManager>().RegisterHit(attackDamage);
        }
    }
}
