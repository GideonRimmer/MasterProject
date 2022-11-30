using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class FollowerManager : MonoBehaviour
{
    [Header("Setup")]
    public bool drawGizmos;
    public GameObject popupMenu;
    private NavMeshAgent navMeshAgent;
    public Transform destination;
    public float maxDistanceToLeader = 12f;
    public float minDistanceToLeader = 5f;
    [SerializeField] private float currentDistanceToTarget;
    public float minDistanceToTarget = 5f;
    public Animator animator;
    private Rigidbody rigidbody;
    //public GameObject takerPrefab;
    public GameObject takerPrefab;
    public GameObject player;
    private Camera mainCamera;
    private SpawnEntitiesAtRandom spawnEntitiesScript;
    public bool canBeTraitor;
    public bool isTraitor;
    public bool startBetrayal;
    public bool isAttackTarget;
    public bool isConversionTarget;
    public List<FollowerManager> activeFollowers = new List<FollowerManager>();
    public TextMeshProUGUI tar;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI col;
    public TextMeshProUGUI collisionText;
    public int maxHitPoints = 10;
    private PlayRandomSound attackSound;
    public AudioClip deathSound;
    private GameManager gameManager;
    private SavePlayerData saveDataManager;

    [Header("Charisma")]
    public int minCharisma;
    public int maxCharisma;
    public int minStartingCharisma;
    public int maxStartingCharisma;
    public int startingCharisma;
    public int currentCharisma;
    public int betrayalCharismaRange;
    public int playerGainCharFromKill;
    public TextMeshProUGUI charismaText;
    [SerializeField] private bool leveledUp;

    [Header("Loyalty")]
    public int startingLoyalty = 5;
    public int currentLoyalty;
    private int maxLoyalty = 999;
    public TextMeshProUGUI loyaltyText;
    public float chanceToBetray = 0.3f;

    [Header("Violence")]
    public int startingViolence = 5;
    public int currentViolence;
    private int maxViolence = 999;
    [SerializeField] private int killCount = 0;

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
    }

    [Header("Movement Parameters")]
    public Transform currentLeader;
    public float moveSpeed;
    public float rotateSpeed;
    //public float maxDistanceToTarget;
    //[SerializeField] private float currentDistanceToTarget;

    [Header("Attack Parameters")]
    [SerializeField] private int attackDamage;
    public Collider attackTrigger;
    private int initialDamage = 1;
    public int maxDamage = 3;
    public float attackSpeedBonus = 4;
    public float attackTimer = 1.0f;
    [SerializeField] private float attackCurrentTime;
    public int convertDamage = 1;
    public Transform enemyTarget;
    [SerializeField] private float distanceToEnemy;
    public float initialConvertCooldown = 3.0f;
    [SerializeField] private float convertCooldown;
    public LayerMask characterLayers;
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    public LayerMask followerLayer;
    public LayerMask innocentLayer;

    [Header("Materials")]
    [SerializeField] private Material defaultMaterial;
    public Material idleMaterial;
    public Material skinMaterial;
    public Material clickableMaterial;
    public Material highCharismaMaterial;
    public Material traitorMaterial;
    public Material traitorFollowerMaterial;
    public Material followPlayerMaterial;
    public Material followOtherMaterial;
    public Material attackMaterial;

    [Header("Body Parts")]
    public Renderer[] skin;
    public Renderer[] clothes;

    [Header("OverlapSphere Parameters")]
    private float sphereInitialRadius = 12f;
    private float sphereCurrentRadius;
    private float sphereMaxRadius = 20f;
    private Collider agentCollider;
    //public Collider AgentCollider { get { return agentCollider; } }
    [SerializeField] private float distanceToClosest;
    [SerializeField] private Collider closestObject;
    [SerializeField] Collider[] agentsInSphere;


    void Start()
    {
        spawnEntitiesScript = FindObjectOfType<SpawnEntitiesAtRandom>();
        rigidbody = GetComponent<Rigidbody>();
        attackSound = GetComponent<PlayRandomSound>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(destination.position);
        gameManager = FindObjectOfType<GameManager>();
        saveDataManager = FindObjectOfType<SavePlayerData>();

        currentState = State.Idle;
        isClickable = false;
        overrideTarget = false;
        isTraitor = false;
        startBetrayal = false;
        isAttackTarget = false;
        isConversionTarget = false;
        leveledUp = false;
        attackCurrentTime = attackTimer;
        attackDamage = initialDamage;
        convertCooldown = initialConvertCooldown;
        attackTrigger.enabled = false;

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
        currentLoyalty = startingLoyalty;
        currentViolence = startingViolence;

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
        sphereCurrentRadius = Mathf.Min(sphereInitialRadius + currentCharisma, sphereMaxRadius);
        agentsInSphere = Physics.OverlapSphere(transform.position, sphereCurrentRadius, characterLayers);

        // Get all of the agents in the sphere in each FixedUpdate.
        foreach (Collider agent in agentsInSphere)
        {
            //if (agent.gameObject.layer == followerLayer)
            if (agent.CompareTag("Follower"))
            {
                FollowerManager agentFollower = agent.GetComponentInParent<FollowerManager>();

                // Attack conditions:
                if (currentState != State.Attack && currentLeader != null && agentFollower.gameObject != this.gameObject &&
                    // If this is a Player follower AND the agent is set as isAttackTarget.
                    ((currentLeader.CompareTag("Player") && agentFollower.isAttackTarget == true && startBetrayal == false) ||
                    // If this is a Player follower AND agent is not a player follower AND agent is set as isConversionTarget.
                    (currentLeader.CompareTag("Player") && agentFollower.currentLeader != null && agentFollower.currentLeader.tag != "Player" && agentFollower.isConversionTarget == true) ||
                    // If this is a player follower, attack Taker followers.
                    (currentLeader.CompareTag("Player") && agentFollower.currentLeader != null && agentFollower.currentLeader.CompareTag("Taker")) ||
                    // If this is a traitor that has more followers than the player, attack the player.
                    (isTraitor == true && startBetrayal == true && agentFollower.currentLeader != null && agentFollower.currentLeader.CompareTag("Player")) ||
                    // Attack any follower attacking this entity.
                    (agentFollower.enemyTarget != null && agentFollower.enemyTarget == this.gameObject) ||
                    // Attack any follower attacking this entity's leader, UNLESS this entity is a traitor.
                    (currentLeader.CompareTag("Player") && agentFollower.enemyTarget != null && agentFollower.enemyTarget == currentLeader && startBetrayal == false) ||
                    // If this is a Taker follower, attack player followers.
                    (currentLeader.CompareTag("Taker") && agentFollower.currentLeader != null && agentFollower.currentLeader.CompareTag("Player"))))
                {
                    SetAttackTarget(agent.transform);
                }
                // If this is a traitor, select a non-traitor player follower to attack.
                else if (currentState != State.Attack && currentLeader != null && agentFollower.gameObject != this.gameObject &&
                    isTraitor == true && startBetrayal == false && agentFollower.currentLeader != null && agentFollower.currentLeader.CompareTag("Player") &&
                    agentFollower.isTraitor == false && convertCooldown <= 0)
                {
                    Debug.Log(this.name + " converts " + agent.name);
                    agent.GetComponentInParent<FollowerManager>().isConversionTarget = true;
                    //SetConversionTarget(agent.transform);
                    SetAttackTarget(agent.transform);
                    //animator.SetBool("isAttacking", false);
                    //animator.SetBool("isConverting", true);
                }
                
                // If this is a traitor with higher charisma that other traitors, convert them and their followers to follow this.
                else if (isTraitor == true && agentFollower.isTraitor == true && currentCharisma > agentFollower.currentCharisma)
                {
                    agentFollower.isTraitor = false;
                    /*
                    foreach (FollowerManager traitorFollower in agentFollower.activeFollowers)
                    {
                        ConvertToFollowSelf(traitorFollower, 100);
                    }
                    ConvertToFollowSelf(agentFollower, 100);
                    */
                }
            }

            // If this is one of the player's followers, also attack any Innocents in range (MUWAHAHAHA), and enemies.
            else if (currentState != State.Attack && currentLeader != null && currentLeader.CompareTag("Player") &&
                ((agent.CompareTag("Innocent") && agent.GetComponentInParent<InnocentManager>().currentFaction == InnocentManager.Faction.Enemy)||
                agent.CompareTag("Enemy")))
            {
                Debug.Log(this.name  + " attack innocent or enemy " + agent);
                SetAttackTarget(agent.transform);
            }

            // If the traitor has more followers than the player, attack the player.
            if (player != null && isTraitor == true && currentLeader != null && activeFollowers.Count >= player.GetComponentInParent<SphereOfInfluence>().activeFollowers.Count * 0.66)
            {
                startBetrayal = true;
                // If betrayal has started and there are no other eligible targets, traitor attacks the player.
                if (currentState != State.Attack)
                {
                    SetAttackTarget(player.transform);
                }

                // All traitor followers attack the player.
                foreach (FollowerManager follower in activeFollowers)
                {
                    if (follower != null && follower.currentState != State.Attack)
                    {
                        Debug.Log(this.name  + " attack player " + agent);
                        follower.SetAttackTarget(player.transform);
                    }
                }
            }
        }
    }

    void Update()
    {
        /*
        // DEBUG: Show charisma and loyalty texts in game.
        if (charismaText != null)
        {
            charismaText.text = currentCharisma.ToString();
            charismaText.transform.LookAt(mainCamera.transform);
            charismaText.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        }
        if (loyaltyText != null)
        {
            loyaltyText.text = currentLoyalty.ToString();
            loyaltyText.transform.LookAt(mainCamera.transform);
            loyaltyText.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        }

        // DEBUG: Show colllision target and attack target in game.
        tar.transform.LookAt(mainCamera.transform);
        tar.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        targetText.transform.LookAt(mainCamera.transform);
        targetText.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        col.transform.LookAt(mainCamera.transform);
        col.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        collisionText.transform.LookAt(mainCamera.transform);
        collisionText.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        */

        if (overrideTarget == true)
        {
            currentState = State.OverrideFollow;
        }

        // Align popup menu to camera.
        popupMenu.transform.LookAt(mainCamera.transform);
        popupMenu.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);

        // State machine.
        switch (currentState)
        {
            default:
            case State.Idle:
                animator.SetBool("isWalking", false);
                animator.speed = 1;
                break;

            case State.FollowPlayer:
                if (currentLeader != null)
                {
                    animator.SetBool("isWalking", true);
                    animator.speed = 1;
                    navMeshAgent.speed = moveSpeed;
                    FollowTarget();
                }
                else if (currentLeader == null)
                {
                    currentState = State.Idle;
                }
                break;

            case State.FollowOther:
                if (currentLeader != null)
                {
                    //animator.SetBool("isWalking", true);
                    animator.speed = 1;
                    navMeshAgent.speed = moveSpeed;
                    FollowTarget();
                }
                else if (currentLeader == null)
                {
                    currentState = State.Idle;
                }
                break;

            case State.Attack:
                if (enemyTarget != null && currentLeader != null)
                {
                    /*
                    if (enemyTarget.CompareTag("Follower"))
                    {
                        animator.SetBool("isConverting", true);
                    }
                    else animator.SetBool("isAttacking", true);
                    */

                    animator.speed = 1;
                    //animator.SetBool("isWalking", true);
                    //animator.speed = 2;
                    FollowAndAttackTarget();
                    if (currentDistanceToTarget <= minDistanceToTarget)
                    {
                        navMeshAgent.isStopped = true;
                    }
                    navMeshAgent.speed = moveSpeed + attackSpeedBonus;
                    navMeshAgent.stoppingDistance = 0;
                }
                else if (currentLeader == null)
                {
                    enemyTarget = null;
                    ChangeMaterial(clothes, idleMaterial);
                    ChangeMaterial(skin, skinMaterial);
                    currentState = State.Idle;
                }
                else if (enemyTarget == null && (currentLeader.CompareTag("Player") || currentLeader.CompareTag("Follower")))
                {
                    //ChangeMaterial(clothes, followPlayerMaterial);
                    ChangeMaterial(skin, skinMaterial);
                    currentState = State.FollowPlayer;
                }
                else if (enemyTarget == null && currentLeader.CompareTag("Taker"))
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
        }

        if (isTraitor == true && convertCooldown > 0)
        {
            //Debug.Log(this.name + " convert cooldown: " + convertCooldown);
            convertCooldown -= Time.deltaTime;
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
        if (other.gameObject.CompareTag("Player") && overrideTarget == false)
        {
            SphereOfInfluence playerSphere = other.gameObject.GetComponentInParent<SphereOfInfluence>();
            if (currentState == State.Idle || (currentState == State.FollowOther && playerSphere.currentCharisma > currentLeader.GetComponentInParent<SphereOfInfluence>().currentCharisma))
            {
                //Debug.Log(this.name + " becomes clickable.");
                ChangeMaterial(clothes, clickableMaterial);
                isClickable = true;
            }

            if (currentState == State.FollowOther && currentCharisma > playerSphere.currentCharisma)
            {
                Debug.Log(this.name + " attack " + other.transform.gameObject.name);
                SetAttackTarget(other.transform.parent.gameObject.transform);
            }
        }

        // If is in Taker range AND is idle AND Taker charisma is higher than currentCharisma,
        // OR if following another entity AND Taker charisma is higher than current leader's charisma, start following the new taker.
        if (overrideTarget == false && (other.gameObject.CompareTag("Taker") || (other.gameObject.CompareTag("Player") && other.GetComponentInParent<PlayerController>().autoCollectFollowers == true)))
        {
            if (other.gameObject.GetComponentInParent<SphereOfInfluence>() != null)
            {
                SphereOfInfluence otherSphere = other.gameObject.GetComponentInParent<SphereOfInfluence>();
                if ((currentState == State.Idle && currentLeader == null && otherSphere.currentCharisma > currentCharisma)
                   || currentLeader != null && currentLeader.GetComponentInParent<SphereOfInfluence>() != null && otherSphere.currentCharisma > currentLeader.GetComponentInParent<SphereOfInfluence>().currentCharisma && (currentState == State.FollowOther || (currentState == State.FollowPlayer && currentLeader.CompareTag("Player"))))
                {
                    SetFollowLeader(other.transform.parent.gameObject.transform);
                }

                // If is already following a leader AND walks into Taker sphere AND new taker charisma < this.currentCharisma -> Attack the taker.
                if (currentState == State.FollowPlayer && other.CompareTag("Taker") && otherSphere.currentCharisma < currentCharisma)
                {
                    Debug.Log(this.name + " attack " + other.transform.gameObject.name);
                    SetAttackTarget(other.transform.parent.gameObject.transform);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isClickable == true)
        {
            isClickable = false;

            if (currentLeader == null)
            {
                ChangeMaterial(clothes, idleMaterial);
            }
            else if (currentLeader.CompareTag("Taker"))
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

    private void OnTriggerStay(Collider other)
    {
        // If the trigger is the attack trigger, then initiate attack sequence.
        if (currentState == State.Attack && enemyTarget != null && other.gameObject.layer == enemyTarget.gameObject.layer)
        {
            //Debug.Log("Start follower trigger attack");
            
            FollowerManager enemyFollower = enemyTarget.GetComponentInParent<FollowerManager>();

            attackCurrentTime -= Time.deltaTime;

            if (attackCurrentTime <= 0)
            {
                Debug.Log("Timer reset");

                if (enemyTarget.tag != "Follower")
                {
                    // Damage the target on collision.
                    animator.Play("Tall_Attack", 0, 0.0f);

                    Attack(other.gameObject.GetComponentInParent<HitPointsManager>(), attackDamage);

                    attackCurrentTime = attackTimer;

                    //Debug.Log(this.name + " attacks " + enemyTarget + ", " + enemyTarget.name);
                }
                else if (enemyTarget.CompareTag("Follower") && other.gameObject.name == enemyTarget.name && enemyFollower.isConversionTarget == false
                    //&& ((enemyFollower.currentLeader != currentLeader && enemyFollower.isConversionTarget == false)
                    && (enemyFollower.currentLeader != currentLeader
                    || (enemyFollower.currentLeader == currentLeader && enemyFollower.isAttackTarget == true)
                    || (enemyFollower.currentLeader == currentLeader && enemyFollower.isTraitor && enemyFollower.currentState == State.Attack)
                    || (enemyFollower.currentLeader == currentLeader && isTraitor == true)
                    || enemyFollower.enemyTarget == this.transform))
                {
                    // Damage the target on collision. Damage = Base damage + killCount.
                    animator.Play("Tall_Attack", 0, 0.0f);
                    Attack(other.gameObject.GetComponentInParent<HitPointsManager>(), attackDamage);
                    attackCurrentTime = attackTimer;
                    Debug.Log("Attack damage " + attackDamage);
                }

                // Attack to convert if this is a traitor and the target is in collision range.
                else if (currentState == State.Attack && enemyTarget.CompareTag("Follower") && enemyFollower.isConversionTarget == true && other.gameObject.name == enemyTarget.name)
                {
                    //Convert(collision.gameObject.GetComponentInParent<FollowerManager>(), convertDamage);
                    animator.SetBool("isConverting", true);
                    ConvertToFollowSelf(other.gameObject.GetComponentInParent<FollowerManager>(), convertDamage);
                    attackCurrentTime = attackTimer;
                    Debug.Log("Convert damage " + convertDamage);
                }

                // Reset the attack timer.
                attackCurrentTime = attackTimer;
            }
        }
        else if (enemyTarget == null && isTraitor == true && other.gameObject.CompareTag("Follower") &&
            currentLeader == other.gameObject.GetComponentInParent<FollowerManager>().currentLeader &&
            other.gameObject.GetComponentInParent<FollowerManager>().isTraitor == false)
        {
            attackCurrentTime -= Time.deltaTime;
            
            if (attackCurrentTime == attackTimer && other.gameObject.GetComponentInParent<FollowerManager>().currentLoyalty > 0)
            {
                ConvertToFollowSelf(other.gameObject.GetComponentInParent<FollowerManager>(), convertDamage);
                attackCurrentTime = attackTimer;
                //Debug.Log("Convert damage " + convertDamage);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // On collision, inflict damage on the enemy target, only if colliding with the enemy target.
        if (attackTrigger != null && currentState == State.Attack && enemyTarget != null && collision.gameObject.layer == enemyTarget.gameObject.layer)
        {
            attackTrigger.enabled = true;
        }
    }

    /*
    // Show popup menu on MouseOver, close menu on MouseExit.
    private void OnMouseOver()
    {
        popupMenu.SetActive(true);
    }
    private void OnMouseExit()
    {
        popupMenu.SetActive(false);
    }
    */

    /*
    private void OnMouseDown()
    {
        // If state is clickable and player charisma > this entity's charisma, start following the player when clicked.
        //if ((currentLeader == null && isClickable == true && currentCharisma < player.GetComponentInParent<SphereOfInfluence>().currentCharisma) || currentLeader.GetComponentInParent<SphereOfInfluence>().currentCharisma < player.GetComponentInParent<SphereOfInfluence>().currentCharisma)
        if ((currentLeader == null && isClickable == true && currentCharisma < player.GetComponentInParent<SphereOfInfluence>().currentCharisma) ||
            (currentLeader != null && currentLeader.GetComponentInParent<SphereOfInfluence>().currentCharisma < player.GetComponentInParent<SphereOfInfluence>().currentCharisma))
        {
            //Debug.Log("Clicked on " + this.name);
            SetFollowLeader(player.transform);
        }

        // DEBUG: Click on a current follower to mark it as a traitor.
        //else if (currentLeader != null && currentLeader.tag == "Player")
        //{
        //   isTraitor = true;
        //}
    }
    */


    // Acquire a new target to follow.
    public void SetFollowLeader(Transform newTarget)
    {
        if (currentLeader == null || (newTarget.GetComponentInParent<SphereOfInfluence>() != null && newTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma > currentLeader.GetComponentInParent<SphereOfInfluence>().currentCharisma))
        {
            Debug.Log(name + ": Set follow target " + newTarget.name);
            newTarget.GetComponentInParent<SphereOfInfluence>().GainFollower(this.gameObject);

            if (newTarget.CompareTag("Player"))
            {
                ChangeMaterial(clothes, followPlayerMaterial);
                ChangeMaterial(skin, skinMaterial);
                currentState = State.FollowPlayer;
            }
            else
            {
                ChangeMaterial(clothes, followOtherMaterial);
                ChangeMaterial(skin, skinMaterial);
                currentState = State.FollowOther;
            }

            // If switched targets, register that the old target lost a follower.
            if (newTarget != currentLeader && currentLeader != null)
            {
                currentLeader.GetComponentInParent<SphereOfInfluence>().LoseFollower(this.gameObject);
            }

            // Assign new target to follow.
            currentLeader = newTarget;
        }
        //else if (currentLeader != null && newTarget.tag == "Follower")
        else
        {
            Debug.Log("Follow traitor");
            isConversionTarget = false;
            ChangeMaterial(clothes, traitorFollowerMaterial);
            currentLeader = newTarget;
        }
    }

    private void FollowTarget()
    {
        navMeshAgent.isStopped = false;
        float distanceToLeader = Vector3.Distance(transform.position, currentLeader.position);
        navMeshAgent.stoppingDistance = minDistanceToLeader;

        if (navMeshAgent.velocity != Vector3.zero)
        {
            animator.SetBool("isWalking", true);
        }
        else animator.SetBool("isWalking", false);
        
        // Walk towards the leader if the leader is too far.
        if (distanceToLeader > maxDistanceToLeader)
        {
            //Debug.Log(distanceToLeader);

            destination = currentLeader;
            navMeshAgent.SetDestination(destination.position);
        }

        // Walk away from leader of the leader is too close, to avoid blocking the way.
        else if (distanceToLeader < minDistanceToLeader)
        {
            //Debug.Log(distanceToLeader);
            Vector3 directionToLeader = currentLeader.position - transform.position;
            //Vector3 newPosition = directionToLeader.normalized * -navMeshAgent.speed;
            Vector3 newPosition = directionToLeader.normalized * -minDistanceToLeader;
            navMeshAgent.destination = newPosition;
        }
        // If the leader is between min and max distance, stop walking.
        else if (distanceToLeader >= minDistanceToLeader + 2 && distanceToLeader <= maxDistanceToLeader)
        {
            //Debug.Log("STOP!");
            navMeshAgent.destination = transform.position;
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void FollowAndAttackTarget()
    {
        navMeshAgent.isStopped = false;
        currentDistanceToTarget = Vector3.Distance(transform.position, enemyTarget.position);
        destination = enemyTarget;
        navMeshAgent.SetDestination(destination.position);

        // Stop attacking when running out of eligible targets.
        if (enemyTarget.CompareTag("Follower") && enemyTarget == null)
        {
            Debug.Log(name + ": Target eliminated.");
            SetAttackTarget(null);
            ChangeMaterial(skin, skinMaterial);

            if (currentLeader != null && currentLeader.CompareTag("Player"))
            {
                currentState = State.FollowPlayer;
            }
            else if (currentLeader != null && currentLeader.tag != "Player")
            {
                currentState = State.FollowOther;
            }
            else
            {
                currentState = State.Idle;
            }
        }

        /*
        // If the enemy target has the same followTarget as this entity, or if the enemy doesn't have a follow target, stop attacking.
        if (enemyTarget.tag == "Follower" && ((enemyTarget.GetComponentInParent<FollowerManager>().currentLeader == null) || (enemyTarget.GetComponentInParent<FollowerManager>().currentLeader = this.currentLeader)))
        {
            if (currentLeader.tag == "Player")
            {
                ChangeMaterial(followPlayerMaterial);
                currentState = State.FollowPlayer;
            }
            if (currentLeader.tag == "Taker")
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
        enemyTarget = newEnemy;
        Debug.Log(this.name + " attacks " + enemyTarget.name);
        ChangeMaterial(skin, attackMaterial);
    }

    public void SetThisAsAttackTarget()
    {
        Debug.Log("Set this as attack target " + this.name);
        isAttackTarget = true;
        isConversionTarget = false;
    }

    public void SetThisAsConversionTarget()
    {
        if (currentLeader == null || currentLeader.tag != "Player")
        {
            Debug.Log("Set this as conversion target " + this.name);
            isConversionTarget = true;
            isAttackTarget = false;
        }
    }

    public void BecomeTraitor()
    {
        if (currentLeader != null && currentLeader.CompareTag("Player"))
        {
            isTraitor = true;
            //ChangeMaterial(clothes, traitorMaterial);
        }
    }

    public void Attack(HitPointsManager enemy, int damage)
    {
        Debug.Log(name + " attacks " + enemy.gameObject.name + ", for " + damage + " damage.");
        
        //attackSound.PlayRandomClip();
        enemy.RegisterHit(damage);

        // After destroying the target, gain charisma. Follower and leader gain Violence.
        if (enemyTarget.GetComponent<HitPointsManager>().currentHitPoints <= 0)
        {
            ResolveKill();
        }
    }

    // After destroying the target, gain charisma. Follower and leader gain Violence.
    public void ResolveKill()
    {
        killCount += 1;
        ModifyCharisma(5);
        ModifyViolence(2);
        attackTrigger.enabled = false;

        // Heal HP through the RegisterHit function.
        if (GetComponentInParent<HitPointsManager>().currentHitPoints < maxHitPoints)
        {
            GetComponentInParent<HitPointsManager>().RegisterHit(-1);
        }

        // Increase attackDamage.
        if (attackDamage < maxDamage)
        {
            attackDamage += 1;
        }

        if (currentLeader != null && currentLeader.GetComponentInParent<SphereOfInfluence>() != null)
        {
            currentLeader.GetComponentInParent<SphereOfInfluence>().ModifyCharisma(playerGainCharFromKill);
            currentLeader.GetComponentInParent<SphereOfInfluence>().currentViolence += 1;

            if (enemyTarget.gameObject.tag == "Follower")
            {
                saveDataManager.SaveFollowersKilled(1);
            }
            if (enemyTarget.gameObject.tag == "Enemy")
            {
                saveDataManager.SaveEnemiesKilled(1);
            }
            if (enemyTarget.gameObject.tag == "Innocent")
            {
                saveDataManager.SaveInnocentsKilled(1);
            }
        }
    }

    public void Convert(FollowerManager enemy, int damage)
    {
        enemy.ModifyLoyalty(-damage);
        //Debug.Log(name + " is converting " + enemy.gameObject.name);

        // After converting the target, gain charisma and loyalty and return to "Follow" state.
        if (enemy.currentLoyalty <= 0)
        {
            enemy.isConversionTarget = false;
            enemy.SetFollowLeader(currentLeader.transform);
            currentState = State.FollowPlayer;
            SetAttackTarget(null);
            ModifyCharisma(1);
            ModifyLoyalty(1);
            currentLeader.GetComponentInParent<SphereOfInfluence>().ModifyCharisma(1);
        }
    }

    public void ConvertToFollowSelf(FollowerManager target, int damage)
    {
        if (target.isTraitor == true && currentCharisma > target.currentCharisma)
        {
            target.isTraitor = false;
        }

        target.ModifyLoyalty(-damage);

        if (target.currentLoyalty <= 0)
        {
            enemyTarget = null;
            //Debug.Log(target.name + " follow " + transform.name + " before.");
            //target.ChangeMaterial(clothes, traitorFollowerMaterial);
            target.enemyTarget = null;
            target.SetFollowLeader(this.transform);
            //Debug.Log(target.name + " follow " + transform.name + " after.");
            ModifyCharisma(1);
            ModifyLoyalty(1);

            if (currentLeader.GetComponentInParent<SphereOfInfluence>() != null)
            {
                currentLeader.GetComponentInParent<SphereOfInfluence>().LoseFollower(target.gameObject);
            }
            else if (currentLeader.GetComponentInParent<FollowerManager>() != null)
            {
                currentLeader.GetComponentInParent<FollowerManager>().RemoveFollower(target);
            }
            //currentLeader.GetComponentInParent<SphereOfInfluence>().ModifyCharisma(-1);

            // Add follower to list of followers.
            if (activeFollowers.Contains(target) == false)
            {
                activeFollowers.Add(target);
            }

            // Reset the convert cooldown timer.
            convertCooldown = initialConvertCooldown;
        }
    }

    public void ModifyCharisma(int change)
    {
        currentCharisma += change;
        currentCharisma = Mathf.Clamp(currentCharisma, minCharisma, maxCharisma);

        if (currentLeader != null && currentLeader.CompareTag("Player") && killCount >= 1)
        {
            if (currentCharisma >= player.GetComponentInParent<SphereOfInfluence>().currentCharisma - betrayalCharismaRange)
            {
                leveledUp = true;
                //Debug.Log(this.name + " Potential traitor");
                ChangeMaterial(clothes, highCharismaMaterial);
            }

            if (canBeTraitor == true)
            {
                // Draw a number from 1 to 5. If follower charisma >= player charisma minus this number, become traitor.
                int reqCharismaForBetrayal = player.GetComponentInParent<SphereOfInfluence>().currentCharisma - Random.Range(0, betrayalCharismaRange);
                Debug.Log("Betrayal charisma: " + reqCharismaForBetrayal);
                // If charisma is close to leader charisma, become traitor.
                if (currentLeader.tag != "Follower" && currentCharisma >= reqCharismaForBetrayal)
                {
                    float betrayalRNG = Random.Range(0.0f, 1.0f);
                    Debug.Log("Betrayal RNG = " + betrayalRNG);
                    if (betrayalRNG <= chanceToBetray)
                    {
                        BecomeTraitor();
                    }
                }
            }
        }

        /*
        // If follower charisma is higher than leader charisma, the follower becomes a new leader.
        if (currentLeader != null && currentLeader.GetComponentInParent<SphereOfInfluence>() != null && currentCharisma > currentLeader.GetComponentInParent<SphereOfInfluence>().currentCharisma)
        {
            BecomeLeader();
        }
        */
    }

    public void ModifyLoyalty(int change)
    {
        currentLoyalty += change;
        currentLoyalty = Mathf.Clamp(currentLoyalty, 0, maxLoyalty);

        if (currentLoyalty > 0 && change < 0)
        {
            // Flash the material to signify the hit.
            ChangeMaterial(clothes, traitorFollowerMaterial);
            Invoke("ResetMaterial", 0.1f);
        }
    }

    public void ModifyViolence(int change)
    {
        currentViolence += change;
        currentViolence = Mathf.Clamp(currentViolence, 0, maxViolence);
    }

    public void ChangeMaterial(Renderer[] parts , Material newMaterial)
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

        // Store the default material to flash when getting attacked by a traitor,
        // unless it's a traitorFollower material.
        if (newMaterial != traitorFollowerMaterial)
        {
            defaultMaterial = newMaterial;
        }
    }

    public void ResetMaterial()
    {
        if (leveledUp == false)
        {
            ChangeMaterial(clothes, followPlayerMaterial);
        }
        if (leveledUp == true)
        {
            ChangeMaterial(clothes, highCharismaMaterial);
        }
    }
    
    private void RemoveFollower(FollowerManager activeFollower)
    {
        if (activeFollowers.Contains(activeFollower))
        {
            activeFollowers.RemoveAt(activeFollowers.IndexOf(activeFollower));
            ModifyCharisma(-1);
        }
    }

    private void StopFollower()
    {
        navMeshAgent.isStopped = true;
    }

    public void Die()
    {
        AudioSource.PlayClipAtPoint(deathSound, transform.position);

        Debug.Log(this.name + "register death");
        if (currentLeader != null)
        {
            if (currentLeader.GetComponentInParent<SphereOfInfluence>() != null)
            {
                currentLeader.GetComponentInParent<SphereOfInfluence>().RemoveDeadFollower(this.gameObject);
            }
            else if (currentLeader.GetComponentInParent<FollowerManager>() != null)
            {
                currentLeader.GetComponentInParent<FollowerManager>().RemoveFollower(this);
            }
        }

        Destroy(this.gameObject);
    }

    public void BecomeLeader()
    {
        if (currentLeader != null)
        {
            currentLeader.GetComponentInParent<SphereOfInfluence>().RemoveFollower(this.gameObject);
        }
        GameObject newLeader;
        newLeader = Instantiate(takerPrefab, transform.position, Quaternion.identity);
        GameObject[] takers = GameObject.FindGameObjectsWithTag("Taker");
        int numberOfTakers = takers.Length;
        newLeader.name = "Taker" + (numberOfTakers + 1);
        Debug.Log("newLeader " + newLeader.name);

        newLeader.GetComponent<TakerManager>().randomCharisma = false;
        newLeader.GetComponentInParent<SphereOfInfluence>().startingCharisma = currentCharisma + 1;
        Destroy(gameObject);

        Debug.Log("Become leader " + name);
    }

    private void OnMouseEnter()
    {
        Cursor.SetCursor(gameManager.knifeCursor, Vector2.zero, CursorMode.Auto);
    }
    private void OnMouseExit()
    {
        Cursor.SetCursor(gameManager.defaultCursor, Vector2.zero, CursorMode.Auto);
    }
    private void OnMouseDown()
    {
        SetThisAsAttackTarget();
    }


    private void OnDrawGizmos()
    {
        if (drawGizmos == true)
        {
            Gizmos.DrawWireSphere(transform.position, sphereCurrentRadius);
        }
    }
}