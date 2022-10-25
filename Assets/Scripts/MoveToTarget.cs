//using UnityEditorInternal;
//using UnityEditor;
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
    public float rotateSpeed;

    public bool isAttacking;
    public int attackDamage = 1;
    public Transform enemyTarget;
    [SerializeField] private float distanceToEnemy;

    void Start()
    {
        sphereCurrentRadius = sphereInitialRadius;

        currentState = State.Idle;
    }

    void Update()
    {
        switch (currentState)
        {
            default:
            case State.Idle:
                //sphereCollider.enabled = false;

                break;

            case State.Follow:
                if (currentTarget != null && currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma > GetComponent<LoyaltyManager>().currentLoyalty && (GetComponent<OnClickInteraction>().isFollowing == true || currentTarget.tag == "Taker"))
                {
                    FollowTarget();
                }
                break;

            case State.Attack:
                if (enemyTarget != null && currentState == State.Follow)
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

            // Auto rotate towards the target.
            Vector3 targetDirection = currentTarget.transform.position - transform.position;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
            Debug.DrawRay(transform.position, newDirection, Color.red);

            // Move position a step towards to the target.
            transform.rotation = Quaternion.LookRotation(newDirection);
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

        // Auto rotate towards the target.
        Vector3 targetDirection = enemyTarget.transform.position - transform.position;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Move position a step towards to the target.
        transform.rotation = Quaternion.LookRotation(newDirection);

        if (enemyTarget == null)
        {
            Debug.Log("Target destroyed");
            currentState = State.Follow;
        }
    }

    // Detect enemy. Conditions: tag = Taker, loyalty > Taker charisma, Taker is in range (enter sphere trigger), follower is not neutral (already following someone else).
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.tag == "Enemy")
        if (other.gameObject.tag == "Taker" && this.GetComponent<LoyaltyManager>().currentLoyalty > other.GetComponentInParent<SphereOfInfluence>().currentCharisma && currentState == State.Follow)
        {
            SetAttackTarget(other.gameObject.GetComponentInParent<Transform>());
        }
    }

    public void OnTriggerStay(Collider other)
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
