using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Vector3 startingPosition;
    private Rigidbody rigidbody;
    public float chaseRadius;
    public Animator animator;
    private GameObject player;

    [Header("Movement Parameters")]
    public float moveSpeed;
    public float rotateSpeed;
    public int attackDamage = 1;
    public float attackStateSpeed;
    public float attackTimer = 1.0f;
    [SerializeField] private float attackCurrentTime;
    public Transform enemyTarget;
    public float maxDistanceToTarget;
    [SerializeField] private float currentDistanceToTarget;
    public LayerMask viableTargets;

    [Header("State Machine")]
    public State currentState;
    public enum State
    {
        Idle,
        Attack,
        ReturnToPosition,
    }

    [Header("Materials")]
    public Material idleMaterial;
    public Material skinMaterial;
    public Material attackMaterial;

    [Header("Body Parts")]
    public Renderer[] skin;
    public Renderer[] clothes;

    [Header("OverlapSphere Parameters")]
    public float sphereRadius = 10f;
    private Collider agentCollider;
    [SerializeField] Collider[] agentsInSphere;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        startingPosition = transform.position;
        currentState = State.Idle;
        attackStateSpeed = moveSpeed + 2;

        // Find the player.
        player = GameObject.FindGameObjectWithTag("Player");

        agentCollider = GetComponent<Collider>();

        // Start with default colors.
        ChangeMaterial(clothes, idleMaterial);
        ChangeMaterial(skin, skinMaterial);
    }

    // Use FixedUpdate for OverlapSphere.
    private void FixedUpdate()
    {
        agentsInSphere = Physics.OverlapSphere(transform.position, sphereRadius, viableTargets);

        // Get all of the agents in the sphere in each FixedUpdate.
        foreach (Collider agent in agentsInSphere)
        {
            FollowerManager agentFollower = agent.GetComponentInParent<FollowerManager>();
            // Select a new attack targt if: Not already attacking AND agent is a player, player's follower, leaderless follower, or innocent.
            if (currentState != State.Attack &&
               (agent.tag == "Player" ||
               agent.tag == "Innocent" ||
               (agent.tag == "Follower" && (agentFollower.currentLeader == null || (agentFollower.currentLeader != null && agentFollower.currentLeader.tag == "Player")))))
            {
                SetAttackTarget(agent.transform);
            }
        }
    }

    void Update()
    {
        // State machine.
        switch (currentState)
        {
            default:
            case State.Idle:
                animator.SetBool("isWalking", false);
                break;

            case State.Attack:
                if (enemyTarget != null)
                {
                    FollowAndAttackTarget();
                    animator.SetBool("isWalking", true);
                    animator.speed = 2;
                }
                break;

            case State.ReturnToPosition:
                animator.SetBool("isWalking", true);
                animator.speed = 1;
                MoveToPosition(startingPosition);
                break;
        }

        if (GetComponent<HitPointsManager>().currentHitPoints <= 0)
        {
            Die();
        }
    }

    private void FollowAndAttackTarget()
    {
        Vector3 direction = (enemyTarget.position - rigidbody.transform.position).normalized;
        rigidbody.MovePosition(rigidbody.transform.position + direction * attackStateSpeed * Time.fixedDeltaTime);
        currentDistanceToTarget = Vector3.Distance(transform.position, enemyTarget.position);

        // Auto rotate towards the target.
        Vector3 targetDirection = enemyTarget.position - transform.position;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
        //Debug.DrawRay(transform.position, newDirection, Color.red);

        // Move position a step towards to the target.
        transform.rotation = Quaternion.LookRotation(newDirection);

        if (enemyTarget.GetComponentInParent<HitPointsManager>().currentHitPoints <= 0)
        {
            enemyTarget = null;
        }
        // Stop attacking when running out of eligible targets.
        if (enemyTarget == null || currentDistanceToTarget >= maxDistanceToTarget)
        {
            enemyTarget = null;
            ChangeMaterial(skin, skinMaterial);
            currentState = State.ReturnToPosition;
        }
    }

    public void SetAttackTarget(Transform newEnemy)
    {
        ChangeMaterial(skin, attackMaterial);
        currentState = State.Attack;
        enemyTarget = newEnemy;
        Debug.Log(this.name + " attacks " + enemyTarget.name);
    }

    private void OnCollisionStay(Collision collision)
    {
        // On collision, inflict damage on the enemy target, only if colliding with the enemy target.
        if (currentState == State.Attack && enemyTarget != null && collision.gameObject.name == enemyTarget.name)
        {
            // Damage the target every X seconds (attackTimer), then start a cooldown.
            attackCurrentTime -= Time.deltaTime;

            if (attackCurrentTime <= 0)
            {
                Attack(collision.gameObject.GetComponent<HitPointsManager>(), attackDamage);
                attackCurrentTime = attackTimer;
                //Debug.Log("Attack damage " + attackDamage);
            }
        }
    }

    public void Attack(HitPointsManager enemy, int damage)
    {
        enemy.RegisterHit(damage);
        //Debug.Log(name + " attacks " + enemy.gameObject.name);
    }

    private void ChangeMaterial(Renderer[] parts, Material newMaterial)
    {
        // Change children materials to indicate a change of state.
        foreach (Renderer renderer in parts)
        {
            var mats = new Material[renderer.materials.Length];
            for (var i = 0; i < renderer.materials.Length; i++)
            {
                mats[i] = newMaterial;
            }
            renderer.materials = mats;
        }
    }

    private void MoveToPosition(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - rigidbody.transform.position).normalized;
        rigidbody.MovePosition(rigidbody.transform.position + direction * moveSpeed * Time.fixedDeltaTime);

        // Auto rotate towards the target.
        Vector3 targetDirection = targetPosition - transform.position;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
        //Debug.DrawRay(transform.position, newDirection, Color.red);

        // Move position a step towards to the target.
        transform.rotation = Quaternion.LookRotation(newDirection);

        float distanceToPosition = Vector3.Distance(transform.position, targetPosition);
        if (distanceToPosition <= 1)
        {
            currentState = State.Idle;
        }
    }

    public void Die()
    {
        Debug.Log(this.name + "register death");
        GetComponent<HitPointsManager>().PlayParticleSystem();
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}
