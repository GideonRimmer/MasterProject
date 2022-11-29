using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public bool drawGizmos;
    //private Rigidbody rigidbody;
    private Vector3 startingPosition;
    private NavMeshAgent navMeshAgent;
    public Transform destination;
    public float minDistanceToTarget = 0f;
    public float chaseRadius;
    public Animator animator;
    private GameObject player;
    private PlayRandomSound attackSound;
    public AudioClip deathSound;
    private int killCount = 0;

    [Header("Movement Parameters")]
    public float moveSpeed;
    public float rotateSpeed;
    public float maxDistanceToTarget;

    [Header("Attack Parameters")]
    public Collider attackTrigger;
    public int attackDamage = 1;
    public float attackSpeedBonus = 4f;
    public float attackTimer = 1.0f;
    [SerializeField] private float attackCurrentTime;
    public Transform enemyTarget;
    public LayerMask viableTargets;
    [SerializeField] private float currentDistanceToTarget;

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
        //rigidbody = GetComponent<Rigidbody>();
        attackSound = GetComponent<PlayRandomSound>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        startingPosition = transform.position;
        currentState = State.Idle;
        attackTrigger.enabled = false;
        attackCurrentTime = attackTimer;

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
               (agent.tag == "Innocent" && agent.GetComponentInParent<InnocentManager>().currentFaction != InnocentManager.Faction.Enemy)||
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
                    navMeshAgent.speed = moveSpeed + attackSpeedBonus;
                    animator.SetBool("isWalking", true);
                    //animator.speed = 2;

                    animator.speed = 1;
                }
                break;

            case State.ReturnToPosition:
                animator.SetBool("isWalking", true);
                animator.speed = 1;
                navMeshAgent.speed = moveSpeed;
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
        /*
        Vector3 direction = (enemyTarget.position - rigidbody.transform.position).normalized;
        rigidbody.MovePosition(rigidbody.transform.position + direction * attackStateSpeed * Time.fixedDeltaTime);
        currentDistanceToTarget = Vector3.Distance(transform.position, enemyTarget.position);

        // Auto rotate towards the target.
        Vector3 targetDirection = enemyTarget.position - transform.position;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
        //Debug.DrawRay(transform.position, newDirection, Color.red);

        // Move position a step towards to the target.
        transform.rotation = Quaternion.LookRotation(newDirection);
        */

        currentDistanceToTarget = Vector3.Distance(transform.position, enemyTarget.position);
        destination = enemyTarget;
        navMeshAgent.SetDestination(destination.position);

        if (enemyTarget.GetComponentInParent<HitPointsManager>().currentHitPoints <= 0)
        {
            ResolveKill();
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
    private void OnCollisionEnter(Collision collision)
    {
        // On collision, inflict damage on the enemy target, only if colliding with the enemy target.
        if (attackTrigger != null && currentState == State.Attack && enemyTarget != null && collision.gameObject.layer == enemyTarget.gameObject.layer)
        {
            Debug.Log(name + " attack trigger enabled.");

            // Enable the attack trigger.
            attackTrigger.enabled = true;
        }


        /*
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
        */
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(name + " trig");

        // If the trigger is the attack trigger, then initiate attack sequence.
        //if (currentState == State.Attack && enemyTarget != null && other.gameObject.GetComponentInParent<HitPointsManager>() != null && other.gameObject.layer == enemyTarget.gameObject.layer)
        if (currentState == State.Attack && enemyTarget != null && other.gameObject.GetComponentInParent<HitPointsManager>() != null)
        {
            Debug.Log("Start enemy trigger attack");
            // Damage the target every X seconds (attackTimer), then start a cooldown.
            attackCurrentTime -= Time.deltaTime;

            if (attackCurrentTime <= 0)
            {
                Debug.Log(name + " attack!!! " + enemyTarget.name);
                animator.Play("Tall_Attack", 0, 0.0f);
                Attack(other.gameObject.GetComponentInParent<HitPointsManager>(), attackDamage);

                // Reset the attack timer.
                attackCurrentTime = attackTimer;
                //Debug.Log("Attack damage " + attackDamage);
            }
        }
    }
    
    public void Attack(HitPointsManager enemy, int damage)
    {
        //attackSound.PlayRandomClip();
        Debug.Log(name + " attacks " + enemy.gameObject.name);
        enemy.RegisterHit(damage);
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
        /*
        Vector3 direction = (targetPosition - rigidbody.transform.position).normalized;
        rigidbody.MovePosition(rigidbody.transform.position + direction * moveSpeed * Time.fixedDeltaTime);

        // Auto rotate towards the target.
        Vector3 targetDirection = targetPosition - transform.position;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
        //Debug.DrawRay(transform.position, newDirection, Color.red);

        // Move position a step towards to the target.
        transform.rotation = Quaternion.LookRotation(newDirection);
        */

        navMeshAgent.SetDestination(startingPosition);

        float distanceToPosition = Vector3.Distance(transform.position, targetPosition);
        if (distanceToPosition <= 1)
        {
            currentState = State.Idle;
        }
    }

    private void ResolveKill()
    {
        enemyTarget = null;
        killCount += 1;
        attackTrigger.enabled = false;
        
        // Heal one HP.
        GetComponentInParent<HitPointsManager>().RegisterHit(-1);

        if (killCount > 0)
        {
            attackDamage = 2;
        }
    }

    public void Die()
    {
        AudioSource.PlayClipAtPoint(deathSound, transform.position);

        Debug.Log(this.name + "register death");
        GetComponent<HitPointsManager>().PlayParticleSystem();
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos == true)
        {
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
        }
    }
}
