using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FollowerManager : MonoBehaviour
{
    [Header("Setup")]
    public GameObject popupMenu;
    public Animator animator;
    private Rigidbody rigidbody;
    //public string[] charClass = { "Citizen", "Soldier", "Intelligentsia" };
    //public GameObject takerPrefab;
    public GameObject takerPrefab;
    private float sphereInitialRadius = 10f;
    private float sphereCurrentRadius;
    public GameObject player;
    private Camera mainCamera;
    private SpawnEntitiesAtRandom spawnEntitiesScript;
    public bool isTraitor;
    public bool isAttackTarget;
    public bool isConversionTarget;
    public List<GameObject> activeFollowers = new List<GameObject>();

    [Header("Charisma")]
    public int minCharisma;
    public int maxCharisma;
    public int minStartingCharisma;
    public int maxStartingCharisma;
    public int startingCharisma;
    public int currentCharisma;
    public TextMeshProUGUI charismaText;

    [Header("Loyalty")]
    public int startingLoyalty = 5;
    public int currentLoyalty;
    private int maxLoyalty = 999;
    public TextMeshProUGUI loyaltyText;

    [Header("Violence")]
    public int startingViolence = 5;
    public int currentViolence;
    private int maxViolence = 999;

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
    public Transform currentLeader;
    public float moveSpeed;
    public float rotateSpeed;
    public float minDistanceToTarget;
    //public float maxDistanceToTarget;
    //[SerializeField] private float currentDistanceToTarget;
    public int attackDamage = 1;
    public float attackStateSpeed;
    public float attackTimer = 1.0f;
    public int convertDamage = 1;
    [SerializeField] private float attackCurrentTime;
    public Transform enemyTarget;
    [SerializeField] private float distanceToEnemy;
    public float runAwaySpeed;

    [Header("Materials")]
    public Material idleMaterial;
    public Material skinMaterial;
    public Material clickableMaterial;
    public Material traitorMaterial;
    public Material traitorFollowerMaterial;
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
        popupMenu.SetActive(true);

        currentState = State.Idle;
        isClickable = false;
        overrideTarget = false;
        isTraitor = false;
        isAttackTarget = false;
        isConversionTarget = false;
        attackStateSpeed = moveSpeed + 4;
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
        LayerMask layerMask = LayerMask.GetMask("Characters");
        sphereCurrentRadius = sphereInitialRadius + currentCharisma;
        agentsInSphere = Physics.OverlapSphere(transform.position, sphereCurrentRadius, layerMask);

        // Get all of the agents in the sphere in each FixedUpdate.
        foreach (Collider agent in agentsInSphere)
        {
            if (agent.tag == "Follower")
            {
                FollowerManager agentFollower = agent.GetComponentInParent<FollowerManager>();

                /*
                // Attack conditions: IF agent is a follower, with a different leader, 
                // IF this has higher charisma -> attacks the lower charisma. OR if the agent is currently attacking, attack it back.
                if (currentLeader != null && agentFollower.currentLeader != null && currentLeader != agentFollower.currentLeader &&
                   (currentCharisma > agentFollower.currentCharisma || agentFollower.currentState == State.Attack))
                {
                    SetAttackTarget(agent.transform);
                }
                */

                /*
                // Attack followers in range who are attacking your leader, OR a follower of the same leader that's marked as a traitor.
                if ((currentLeader != null && agentFollower.currentLeader != null && currentLeader != agentFollower.currentLeader && agentFollower.currentState == State.Attack) ||
                    (agentFollower.isTraitor == true && agentFollower.currentLeader == currentLeader && agentFollower.gameObject != this.gameObject))
                {
                    SetAttackTarget(agent.transform);
                }
                */

                // If this is one of the player's followers, attack a taker follower, and vice versa.
                if ((currentLeader != null && agentFollower.currentLeader != null && currentLeader.tag != agentFollower.currentLeader.tag && agentFollower.currentState == State.Attack && isTraitor == false)
                    || (agentFollower.isAttackTarget == true && agentFollower.gameObject != this.gameObject && currentLeader != null && currentLeader.tag == "Player")
                    || (agentFollower.isConversionTarget == true && agentFollower.gameObject != this.gameObject && currentLeader != null && currentLeader.tag == "Player"))
                {
                    SetAttackTarget(agent.transform);
                }
            }

            // Also attack any Innocents in range (MUWAHAHAHA).
            else if (currentLeader != null && currentLeader.tag == "Player" && agent.tag == "Innocent")
            {
                SetAttackTarget(agent.transform);
            }

            // If the traitor has more followers than the player, attack the player.
            if (isTraitor == true && currentLeader != null && activeFollowers.Count > currentLeader.GetComponentInParent<SphereOfInfluence>().activeFollowers.Count * 0.5)
            {
                foreach (GameObject follower in activeFollowers)
                {
                    if (follower != null)
                    {
                        follower.GetComponentInParent<FollowerManager>().SetAttackTarget(player.transform);
                    }
                }
            }
        }
    }

    void Update()
    {
        // DEBUG: Show charisma and loyalty texts in game.
        charismaText.text = currentCharisma.ToString();
        charismaText.transform.LookAt(mainCamera.transform);
        charismaText.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        loyaltyText.text = currentLoyalty.ToString();
        loyaltyText.transform.LookAt(mainCamera.transform);
        loyaltyText.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);

        if (overrideTarget == true)
        {
            currentState = State.OverrideFollow;
        }
        // Align popup menu to camera.
        popupMenu.transform.LookAt(mainCamera.transform);
        popupMenu.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);

        //List<Transform> context = GetNearbyObjects(this.gameObject);
        //GetNearbyObjects(this.gameObject);

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
                    FollowTarget();
                    //animator.SetBool("isWalking", true);
                    animator.speed = 1;
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
                    FollowAndAttackTarget();
                    animator.SetBool("isWalking", true);
                    animator.speed = 2;
                }
                else if (currentLeader == null)
                {
                    enemyTarget = null;
                    ChangeMaterial(clothes, idleMaterial);
                    ChangeMaterial(skin, skinMaterial);
                    currentState = State.Idle;
                }
                else if (enemyTarget == null && currentLeader.tag == "Player")
                {
                    ChangeMaterial(clothes, followPlayerMaterial);
                    ChangeMaterial(skin, skinMaterial);
                    currentState = State.FollowPlayer;
                }
                else if (enemyTarget == null && currentLeader.tag == "Taker")
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
            if (currentState == State.Idle || (currentState == State.FollowOther && playerSphere.currentCharisma > currentLeader.GetComponentInParent<SphereOfInfluence>().currentCharisma))
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
            if ((currentState == State.Idle && takerSphere.currentCharisma > currentCharisma) || ((currentState == State.FollowOther || currentState == State.FollowPlayer) && takerSphere.currentCharisma > currentLeader.GetComponentInParent<SphereOfInfluence>().currentCharisma))
            {
                SetFollowLeader(other.transform);
            }

            // If is already following a leader AND walks into Taker sphere AND new taker charisma < this.currentCharisma -> Attack thr taker.
            //if ((currentState == State.FollowPlayer || currentState == State.FollowOther) && takerSphere.currentCharisma < currentCharisma)
            if (currentState == State.FollowPlayer && takerSphere.currentCharisma < currentCharisma)
                {
                //Debug.Log(this.name + " attack " + other.transform.gameObject.name);
                SetAttackTarget(other.transform.parent.gameObject.transform);
            }
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

        /*
        // DEBUG: Click on a current follower to mark it as a traitor.
        else if (currentLeader != null && currentLeader.tag == "Player")
        {
            isTraitor = true;

        }
        */
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
            else if (currentLeader.tag == "Taker")
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
    public void SetFollowLeader(Transform newTarget)
    {
        if (currentLeader == null || (newTarget.GetComponentInParent<SphereOfInfluence>() != null && newTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma > currentLeader.GetComponentInParent<SphereOfInfluence>().currentCharisma))
        {
            Debug.Log(name + ": Set target " + newTarget.name);
            newTarget.GetComponentInParent<SphereOfInfluence>().GainFollower(this.gameObject);

            if (newTarget.tag == "Player")
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
            ChangeMaterial(clothes, traitorFollowerMaterial);
            currentLeader = newTarget;
        }
    }

    private void FollowTarget()
    {
        Vector3 direction = (currentLeader.position - rigidbody.transform.position).normalized;
        if (Vector3.Distance(transform.position, currentLeader.position) >= minDistanceToTarget)
        {
            rigidbody.MovePosition(rigidbody.transform.position + direction * moveSpeed * Time.fixedDeltaTime);
            // Auto rotate towards the target.
            Vector3 targetDirection = currentLeader.position - transform.position;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
            //Debug.DrawRay(transform.position, newDirection, Color.red);
            transform.rotation = Quaternion.LookRotation(newDirection);
            animator.SetBool("isWalking", true);
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

        // Stop attacking when running out of eligible targets.
        if (enemyTarget.tag == "Follower" && (enemyTarget == null || enemyTarget.GetComponentInParent<FollowerManager>().currentLeader == currentLeader))
        {
            Debug.Log(name + ": Target eliminated.");
            SetAttackTarget(null);
            ChangeMaterial(skin, skinMaterial);
            if (currentLeader != null && currentLeader.tag == "Player")
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
        //Debug.Log(this.name + " attacks " + enemyTarget.name);
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
        if (currentLeader != null && currentLeader.tag == "Player")
        {
            isTraitor = true;
            ChangeMaterial(clothes, traitorMaterial);
        }
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
                if (enemyTarget.tag != "Follower"
                    || (enemyTarget.tag == "Follower" && enemyTarget.GetComponentInParent<FollowerManager>().currentLeader != currentLeader && enemyTarget.GetComponentInParent<FollowerManager>().isConversionTarget == false)
                    || (enemyTarget.tag == "Follower" && enemyTarget.GetComponentInParent<FollowerManager>().currentLeader == currentLeader && enemyTarget.GetComponentInParent<FollowerManager>().isTraitor == true))
                {
                    Attack(collision.gameObject.GetComponent<HitPointsManager>(), attackDamage);
                    attackCurrentTime = attackTimer;
                    //Debug.Log("Attack damage " + attackDamage);
                }
                else if (enemyTarget.tag == "Follower" && enemyTarget.GetComponentInParent<FollowerManager>().isConversionTarget == true)
                {
                    Convert(collision.gameObject.GetComponentInParent<FollowerManager>(), convertDamage);
                    attackCurrentTime = attackTimer;
                    //Debug.Log("Convert damage " + convertDamage);
                }
            }
        }
        else if (enemyTarget == null && isTraitor == true && collision.gameObject.tag == "Follower" && currentLeader == collision.gameObject.GetComponentInParent<FollowerManager>().currentLeader)
        {
            attackCurrentTime -= Time.deltaTime;
            if (attackCurrentTime <= 0 && collision.gameObject.GetComponentInParent<FollowerManager>().currentLoyalty > 0)
            {
                ConvertToFollowSelf(collision.gameObject.GetComponentInParent<FollowerManager>(), convertDamage);
                attackCurrentTime = attackTimer;
                //Debug.Log("Convert damage " + convertDamage);
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
            ModifyViolence(1);
            if (currentLeader.GetComponentInParent<SphereOfInfluence>() != null)
            {
                currentLeader.GetComponentInParent<SphereOfInfluence>().ModifyCharisma(1);
                currentLeader.GetComponentInParent<SphereOfInfluence>().currentViolence += 1;
            }
        }
    }

    public void Convert(FollowerManager enemy, int damage)
    {
        enemy.ModifyLoyalty(-damage);
        Debug.Log(name + " is converting " + enemy.gameObject.name);

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
        target.ModifyLoyalty(-damage);

        if (target.currentLoyalty <= 0)
        {
            Debug.Log(target.name + " follow " + transform.name + " before.");
            //target.ChangeMaterial(clothes, traitorFollowerMaterial);
            target.SetFollowLeader(this.transform);
            Debug.Log(target.name + " follow " + transform.name + " after.");
            ModifyCharisma(1);
            ModifyLoyalty(1);
            currentLeader.GetComponentInParent<SphereOfInfluence>().ModifyCharisma(-1);

            // Add follower to list of followers.
            if (activeFollowers.Contains(target.gameObject) == false)
            {
                activeFollowers.Add(target.gameObject);
            }
        }
    }

    private void RunAway()
    {

    }

    public void ModifyCharisma(int change)
    {
        currentCharisma += change;
        currentCharisma = Mathf.Clamp(currentCharisma, minCharisma, maxCharisma);

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
    }
    public void ModifyViolence(int change)
    {
        currentViolence += change;
        currentViolence = Mathf.Clamp(currentViolence, 0, maxViolence);
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
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, sphereCurrentRadius);
        //Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighbourRadius);

        foreach (Collider collider in contextColliders)
        {
            if ((currentState == State.FollowPlayer || currentState == State.FollowOther) && currentCharisma > agent.GetComponent<FollowerManager>().currentCharisma && currentLeader != agent.GetComponent<FollowerManager>().currentLeader)
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
        if (currentLeader != null && currentLeader.GetComponentInParent<SphereOfInfluence>() != null)
        {
            currentLeader.GetComponentInParent<SphereOfInfluence>().RemoveDeadFollower(this.gameObject);
        }

        GetComponent<HitPointsManager>().PlayParticleSystem();
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sphereCurrentRadius);
    }
}