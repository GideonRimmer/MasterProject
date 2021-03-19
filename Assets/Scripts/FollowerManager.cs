using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FollowerManager : MonoBehaviour
{
    [Header("Setup")]
    public Animator animator;
    private Rigidbody rigidbody;
    public GameObject takerPrefab;
    private float sphereRadius = 13f;
    public GameObject player;
    private Camera mainCamera;
    private SpawnEntitiesAtRandom spawnEntitiesScript;
    public bool isTraitor;

    [Header("Charisma")]
    public int minCharisma;
    public int maxCharisma;
    public int minStartingCharisma;
    public int maxStartingCharisma;
    public int startingCharisma;
    public int currentCharisma;
    public TextMeshProUGUI charismaText;

    [Header("Loyalty")]
    public int currentLoyalty = 0;

    [Header("Violence")]
    public int currentViolence = 0;

    [Header("State Machine")]
    public State currentState;
    [SerializeField] private bool isClickable;
    public bool overrideTarget;
    public enum State
    {
        Idle,
        FollowPlayer,
        FollowOther,
        Attack,
        OverrideFollow,
        RunAway,
    }

    [Header("Movement Parameters")]
    public Transform currentTarget;
    public float moveSpeed;
    public float rotateSpeed;
    public float minDistanceToTarget;
    //public float maxDistanceToTarget;
    //[SerializeField] private float currentDistanceToTarget;
    public int attackDamage = 1;
    public float attackStateSpeed;
    public float attackTimer = 1.0f;
    [SerializeField] private float attackCurrentTime;
    public Transform enemyTarget;
    [SerializeField] private float distanceToEnemy;
    public float runAwaySpeed;

    [Header("Materials")]
    public Material idleMaterial;
    public Material skinMaterial;
    public Material clickableMaterial;
    public Material followPlayerMaterial;
    public Material followOtherMaterial;
    public Material attackMaterial;

    [Header("Body Parts")]
    public Renderer[] skin;
    public Renderer[] clothes;

    [Header("OverlapSphere Parameters")]
    private Collider agentCollider;
    //public Collider AgentCollider { get { return agentCollider; } }
    [SerializeField] private float distanceToClosest;
    [SerializeField] private Collider closestObject;
    [SerializeField] Collider[] agentsInSphere;


    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        spawnEntitiesScript = FindObjectOfType<SpawnEntitiesAtRandom>();

        currentState = State.Idle;
        isClickable = false;
        overrideTarget = false;
        isTraitor = false;
        attackStateSpeed = moveSpeed + 2;
        attackCurrentTime = attackTimer;

        // Generate random charisma.
        if (spawnEntitiesScript != null)
        {
            startingCharisma = Random.Range(spawnEntitiesScript.followerMinChar, spawnEntitiesScript.followerMaxChar);
        }
        else
        {
            startingCharisma = Random.Range(minStartingCharisma, maxStartingCharisma);
        }
        currentCharisma = startingCharisma;

        mainCamera = Camera.main;

        // Find the player.
        player = GameObject.FindGameObjectWithTag("Player");

        agentCollider = GetComponent<Collider>();

        ChangeMaterial(clothes, idleMaterial);
        ChangeMaterial(skin, skinMaterial);
    }

    // Use FixedUpdate for OverlapSphere.
    private void FixedUpdate()
    {
        LayerMask layerMask = LayerMask.GetMask("Characters");
        agentsInSphere = Physics.OverlapSphere(this.transform.position, sphereRadius, layerMask);

        // Get all of the agents in the sphere in each FixedUpdate.
        foreach (Collider agent in agentsInSphere)
        {

            if (agent.tag == "Follower")
            {
                FollowerManager agentFollower = agent.GetComponentInParent<FollowerManager>();

                /*
                // Attack conditions: IF agent is a follower, with a different leader, 
                // IF this has higher charisma -> attacks the lower charisma. OR if the agent is currently attacking, attack it back.
                if (currentTarget != null && agentFollower.currentTarget != null && currentTarget != agentFollower.currentTarget &&
                   (currentCharisma > agentFollower.currentCharisma || agentFollower.currentState == State.Attack))
                {
                    SetAttackTarget(agent.transform);
                }
                */

                /*
                // Attack followers in range who are attacking your leader, OR a follower of the same leader that's marked as a traitor.
                if ((currentTarget != null && agentFollower.currentTarget != null && currentTarget != agentFollower.currentTarget && agentFollower.currentState == State.Attack) ||
                    (agentFollower.isTraitor == true && agentFollower.currentTarget == currentTarget && agentFollower.gameObject != this.gameObject))
                {
                    SetAttackTarget(agent.transform);
                }
                */

                // If this is one of the player's followers, attack a taker follower, and vice versa.
                if ((currentTarget != null && agentFollower.currentTarget != null && currentTarget.tag != agentFollower.currentTarget.tag && agentFollower.currentState == State.Attack)
                    || (agentFollower.isTraitor == true && agentFollower.currentTarget == currentTarget && agentFollower.gameObject != this.gameObject))
                {
                    SetAttackTarget(agent.transform);
                }
            }

            // Also attack any Innocents in range (MUWAHAHAHA).
            else if (currentTarget != null && currentTarget.tag == "Player" && agent.tag == "Innocent")
            {
                SetAttackTarget(agent.transform);
            }
        }
    }

    void Update()
    {
        // DEBUG: Show charisma text in game.
        charismaText.text = currentCharisma.ToString();
        charismaText.transform.LookAt(mainCamera.transform);
        charismaText.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        if (overrideTarget == true)
        {
            currentState = State.OverrideFollow;
        }
        //List<Transform> context = GetNearbyObjects(this.gameObject);
        //GetNearbyObjects(this.gameObject);

        // State machine.
        switch (currentState)
        {
            default:
            case State.Idle:
                animator.SetBool("isWalking", false);
                animator.speed = 1;
                //ChangeMaterial(idleMaterial);
                break;

            case State.FollowPlayer:
                if (currentTarget != null)
                {
                    FollowTarget();
                    animator.SetBool("isWalking", true);
                    animator.speed = 1;
                }
                else if (currentTarget == null)
                {
                    currentState = State.Idle;
                }
                break;

            case State.FollowOther:

                if (currentTarget != null)
                {
                    animator.SetBool("isWalking", true);
                    animator.speed = 1;
                    FollowTarget();
                }
                else if (currentTarget == null)
                {
                    currentState = State.Idle;
                }
                break;

            case State.Attack:
                if (enemyTarget != null && currentTarget != null)
                {
                    FollowAndAttackTarget();
                    animator.SetBool("isWalking", true);
                    animator.speed = 2;
                }
                else if (currentTarget == null)
                {
                    enemyTarget = null;
                    ChangeMaterial(clothes, idleMaterial);
                    ChangeMaterial(skin, skinMaterial);
                    currentState = State.Idle;
                }
                else if (enemyTarget == null && currentTarget.tag == "Player")
                {
                    ChangeMaterial(clothes, followPlayerMaterial);
                    ChangeMaterial(skin, skinMaterial);
                    currentState = State.FollowPlayer;
                }
                else if (enemyTarget == null && currentTarget.tag == "Taker")
                {
                    ChangeMaterial(clothes, followOtherMaterial);
                    ChangeMaterial(skin, skinMaterial);
                    currentState = State.FollowOther;
                }
                break;

            case State.OverrideFollow:
                if (overrideTarget == true)
                {
                    FollowTarget();
                }
                break;

            case State.RunAway:
                RunAway();
                break;
        }

        if (GetComponent<HitPointsManager>().currentHitPoints <= 0)
        {
            Die();
        }
    }

    // Most interactions between the follower and other entities happen on OnTriggerEnter, when the follower steps into their sphere of influence.
    // Depending on the follower's currentState, currentCharisma, and (if already following a leader) the leader's charisma.
    private void OnTriggerEnter(Collider other)
    {
        // If is in player range and not already following the player,
        // OR if following another entity AND player charisma > current leader charisma, become clickable.
        if (other.gameObject.tag == "Player" && overrideTarget == false)
        {
            SphereOfInfluence playerSphere = other.gameObject.GetComponentInParent<SphereOfInfluence>();
            if (currentState == State.Idle || (currentState == State.FollowOther && playerSphere.currentCharisma > currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma))
            {
                //Debug.Log(this.name + " becomes clickable.");
                ChangeMaterial(clothes, clickableMaterial);
                isClickable = true;
            }

            if (currentState == State.FollowOther && currentCharisma > playerSphere.currentCharisma)
            {
                SetAttackTarget(other.transform.parent.gameObject.transform);
            }
        }

        // If is in Taker range AND is idle AND Taker charisma is higher than currentCharisma,
        // OR if following another entity AND Taker charisma is higher than current leader's charisma, start following the new taker.
        if (other.gameObject.tag == "Taker" &&  overrideTarget == false)
        {
            SphereOfInfluence takerSphere = other.gameObject.GetComponentInParent<SphereOfInfluence>();
            if ((currentState == State.Idle && takerSphere.currentCharisma > currentCharisma) || ((currentState == State.FollowOther || currentState == State.FollowPlayer) && takerSphere.currentCharisma > currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma))
            {
                SetFollowTarget(other.transform);
            }

            // If is already following a leader AND walks into Taker sphere AND new taker charisma < this.currentCharisma -> Attack.
            //if ((currentState == State.FollowPlayer || currentState == State.FollowOther) && takerSphere.currentCharisma < currentCharisma)
            if (currentState == State.FollowPlayer && takerSphere.currentCharisma < currentCharisma)
                {
                //Debug.Log(this.name + " attack " + other.transform.gameObject.name);
                SetAttackTarget(other.transform.parent.gameObject.transform);
            }
        }
    }

    private void OnMouseDown()
    {
        // If state is clickable and player charisma > this entity's charisma, start following the player when clicked.
        //if ((currentTarget == null && isClickable == true && currentCharisma < player.GetComponentInParent<SphereOfInfluence>().currentCharisma) || currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma < player.GetComponentInParent<SphereOfInfluence>().currentCharisma)
        if ((currentTarget == null && isClickable == true && currentCharisma < player.GetComponentInParent<SphereOfInfluence>().currentCharisma) ||
            (currentTarget != null && currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma < player.GetComponentInParent<SphereOfInfluence>().currentCharisma))
        {
            //Debug.Log("Clicked on " + this.name);
            SetFollowTarget(player.transform);
        }
        // Click on a current follower to mark it as a traitor.
        else if (currentTarget != null && currentTarget.tag == "Player")
        {
            isTraitor = true;
        }

        /*
        // DEBUG: Click on your follower to transform it into a leader.
        else if (currentTarget != null && currentTarget.tag == "Player")
        {
            Debug.Log("Click active follower " + name);
            BecomeLeader();
        }
        */

    }

    private void OnTriggerExit(Collider other)
    {
        if (isClickable == true)
        {
            isClickable = false;

            if (currentTarget == null)
            {
                ChangeMaterial(clothes, idleMaterial);
            }
            else if (currentTarget.tag == "Taker")
            {
                ChangeMaterial(clothes, followOtherMaterial);
            }
        }

        /*
        if (overrideTarget == true)
        {
            overrideTarget = false;
        }
        */
    }

    // Acquire a new target to follow.
    public void SetFollowTarget(Transform newTarget)
    {
        if (currentTarget == null || newTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma > currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma)
        {
            newTarget.GetComponentInParent<SphereOfInfluence>().GainFollower(this.gameObject);

            if (newTarget.tag == "Player")
            {
                ChangeMaterial(clothes, followPlayerMaterial);
                ChangeMaterial(skin, skinMaterial);
                currentState = State.FollowPlayer;
            }
            else if (newTarget.tag == "Taker")
            {
                ChangeMaterial(clothes, followOtherMaterial);
                ChangeMaterial(skin, skinMaterial);
                currentState = State.FollowOther;
            }

            // If switched targets, register that the old target lost a follower.
            if (newTarget != currentTarget && currentTarget != null)
            {
                currentTarget.GetComponentInParent<SphereOfInfluence>().LoseFollower(this.gameObject);
            }

            // Assign new target to follow.
            currentTarget = newTarget;
        }
    }

    private void FollowTarget()
    {
        Vector3 direction = (currentTarget.position - rigidbody.transform.position).normalized;
        if (Vector3.Distance(transform.position, currentTarget.position) >= minDistanceToTarget)
        {
            rigidbody.MovePosition(rigidbody.transform.position + direction * moveSpeed * Time.fixedDeltaTime);
            // Auto rotate towards the target.
            Vector3 targetDirection = currentTarget.position - transform.position;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
            //Debug.DrawRay(transform.position, newDirection, Color.red);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        else animator.SetBool("isWalking", false);
    }

    private void FollowAndAttackTarget()
    {
        Vector3 direction = (enemyTarget.position - rigidbody.transform.position).normalized;
        rigidbody.MovePosition(rigidbody.transform.position + direction * attackStateSpeed * Time.fixedDeltaTime);
        distanceToEnemy = Vector3.Distance(transform.position, enemyTarget.position);

        // Auto rotate towards the target.
        Vector3 targetDirection = enemyTarget.position - transform.position;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
        //Debug.DrawRay(transform.position, newDirection, Color.red);

        // Move position a step towards to the target.
        transform.rotation = Quaternion.LookRotation(newDirection);

        /*
        // If the enemy target has the same followTarget as this entity, or if the enemy doesn't have a follow target, stop attacking.
        if (enemyTarget.tag == "Follower" && ((enemyTarget.GetComponentInParent<FollowerManager>().currentTarget == null) || (enemyTarget.GetComponentInParent<FollowerManager>().currentTarget = this.currentTarget)))
        {
            if (currentTarget.tag == "Player")
            {
                ChangeMaterial(followPlayerMaterial);
                currentState = State.FollowPlayer;
            }
            if (currentTarget.tag == "Taker")
            {
                ChangeMaterial(followPlayerMaterial);
                currentState = State.FollowOther;
            }
        }
        */
    }

    public void SetAttackTarget(Transform newEnemy)
    {
        currentState = State.Attack;
        //newEnemy.gameObject.transform.parent = enemyTarget;
        enemyTarget = newEnemy;
        //Debug.Log(this.name + " attacks " + enemyTarget.name);
        ChangeMaterial(skin, attackMaterial);
    }

    private void OnCollisionStay (Collision collision)
    {
        // On collision, inflict damage on the enemy target, only if colliding with the enemy target.
        if (enemyTarget != null && collision.gameObject.name == enemyTarget.name)
        {
            // Damage the target every X seconds (attackTimer), then start a cooldown.
            attackCurrentTime -= Time.deltaTime;
            if (attackCurrentTime <= 0)
            {
                Attack(collision.gameObject.GetComponent<HitPointsManager>(), attackDamage);
                attackCurrentTime = attackTimer;
            }
        }
    }

    public void Attack(HitPointsManager enemy, int damage)
    {
        enemy.RegisterHit(damage);
        Debug.Log(name + " attacks " + enemy.gameObject.name);

        // After destroying the target, gain charisma. Follower and leader gain Violence.
        if (enemyTarget.GetComponent<HitPointsManager>().currentHitPoints <= 0)
        {
            ModifyCharisma(2);
            currentViolence += 1;
            currentTarget.GetComponentInParent<SphereOfInfluence>().ModifyCharisma(1);
            currentTarget.GetComponentInParent<SphereOfInfluence>().currentViolence += 1;
        }
    }

    private void RunAway()
    {

    }

    public void ModifyCharisma(int change)
    {
        currentCharisma += change;
        currentCharisma = Mathf.Clamp(currentCharisma, minCharisma, maxCharisma);

        // If follower charisma is higher than leader charisma, the follower becomes a new leader.
        if (currentTarget != null && currentCharisma > currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma)
        {
            BecomeLeader();
        }
    }

    private void ChangeMaterial(Renderer[] parts , Material newMaterial)
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
    
    /*
    public List<Transform> GetNearbyObjects(GameObject agent)
    {
        //distanceToClosest = Mathf.Infinity;
        LayerMask layerMask = LayerMask.GetMask("Characters");
        List<Transform> context = new List<Transform>();
        // Get an array of all colliders in a the radius, using OverlapSphere.
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, sphereRadius);
        //Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighbourRadius);

        foreach (Collider collider in contextColliders)
        {
            if ((currentState == State.FollowPlayer || currentState == State.FollowOther) && currentCharisma > agent.GetComponent<FollowerManager>().currentCharisma && currentTarget != agent.GetComponent<FollowerManager>().currentTarget)
            {
                SetAttackTarget(collider.gameObject.transform);
            }
            // Add all of the transforms of the colliders in the sphere, except this object's (agent's) transform.
            if (collider.gameObject != this)
            {
                //Debug.Log(agent.name + "in range of " + this.name);
                context.Add(collider.transform);
                float dist = Vector3.Distance(transform.position, collider.transform.position);
                if (dist < distanceToClosest)
                {
                    distanceToClosest = dist;
                    closestObject = collider;
                }
            }
        }
        return context;
    }
    */

    public void Die()
    {
        Debug.Log(this.name + "register death");
        if (currentTarget != null)
        {
            currentTarget.GetComponentInParent<SphereOfInfluence>().RemoveDeadFollower(this.gameObject);
        }

        GetComponent<HitPointsManager>().PlayParticleSystem();
        Destroy(this.gameObject);
    }

    void BecomeLeader()
    {
        if (currentTarget != null)
        {
            currentTarget.GetComponentInParent<SphereOfInfluence>().RemoveFollower(this.gameObject);
        }
        Destroy(gameObject);
        takerPrefab.GetComponent<SphereOfInfluence>().startingCharisma = currentCharisma;
        Instantiate(takerPrefab, transform.position, Quaternion.identity);

        Debug.Log("Become leader " + name);
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
    */
}