using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FollowerManager : MonoBehaviour
{
    private Animator animator;
    private float sphereRadius = 10f;
    private enum State
    {
        Idle,
        FollowPlayer,
        FollowOther,
        Attack,
        RunAway,
    }
    [SerializeField] private State currentState;
    [SerializeField] private bool isClickable;

    public int minCharisma;
    public int maxCharisma;
    public int minStartingCharisma;
    public int maxStartingCharisma;
    public int startingCharisma;
    public int currentCharisma;

    public GameObject player;

    public Transform currentTarget;
    public float moveSpeed;
    public float minDistanceToTarget;
    //public float maxDistanceToTarget;
    //[SerializeField] private float currentDistanceToTarget;
    public float rotateSpeed;

    public int attackDamage = 1;
    public float attackStateSpeed;
    public Transform enemyTarget;
    [SerializeField] private float distanceToEnemy;

    public float runAwaySpeed;

    public TextMeshProUGUI charismaText;
    private Camera mainCamera;

    public Material idleMaterial;
    public Material followerMaterial;
    Renderer[] children;


    private Collider agentCollider;
    //public Collider AgentCollider { get { return agentCollider; } }
    [SerializeField] private float distanceToClosest;
    [SerializeField] private Collider closestObject;

    [SerializeField] Collider[] agentsInSphere;


    void Start()
    {
        animator = GetComponent<Animator>();

        currentState = State.Idle;
        isClickable = false;
        attackStateSpeed = moveSpeed + 2;

        // Generate random charisma.
        startingCharisma = Random.Range(minStartingCharisma, maxStartingCharisma);
        currentCharisma = startingCharisma;

        mainCamera = Camera.main;

        children = GetComponentsInChildren<Renderer>();
        player = GameObject.FindGameObjectWithTag("Player");

        agentCollider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        LayerMask layerMask = LayerMask.GetMask("Characters");
        agentsInSphere = Physics.OverlapSphere(this.transform.position, sphereRadius, layerMask);
        foreach (Collider agent in agentsInSphere)
        {
            if (currentTarget != null && agent.tag == "Follower" && currentCharisma > agent.GetComponent<FollowerManager>().currentCharisma)
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

        //List<Transform> context = GetNearbyObjects(this.gameObject);
        //GetNearbyObjects(this.gameObject);

        // State machine.
        switch (currentState)
        {
            default:
            case State.Idle:
                animator.SetBool("isWalking", false);
                break;

            case State.FollowPlayer:
                if (currentTarget != null)
                {
                    FollowTarget();
                    animator.SetBool("isWalking", true);
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
                    FollowTarget();
                }
                else if (currentTarget == null)
                {
                    currentState = State.Idle;
                }
                break;

            case State.Attack:
                if (enemyTarget != null)
                {
                    FollowAndAttackTarget();
                    animator.SetBool("isWalking", true);
                }
                else if (enemyTarget == null && currentTarget.tag == "Player")
                {
                    currentState = State.FollowPlayer;
                }
                else if (enemyTarget == null && currentTarget.tag == "Taker")
                {
                    currentState = State.FollowOther;
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
        if (other.gameObject.tag == "Player")
        {
            if (currentState == State.Idle || (currentState == State.FollowOther && other.gameObject.GetComponentInParent<SphereOfInfluence>().currentCharisma > currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma))
            {
                //Debug.Log(this.name + " becomes clickable.");
                ChangeMaterial(followerMaterial);
                isClickable = true;
            }
        }

        // If is in Taker range AND is idle AND Taker charisma is higher than currentCharisma,
        // OR if following another entity AND Taker charisma is higher than current leader's charisma, start following the new taker.
        if (other.gameObject.tag == "Taker")
        {
            if ((currentState == State.Idle && other.gameObject.GetComponentInParent<SphereOfInfluence>().currentCharisma > currentCharisma) || ((currentState == State.FollowOther || currentState == State.FollowPlayer) && other.gameObject.GetComponentInParent<SphereOfInfluence>().currentCharisma > currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma))
            {
                SetFollowTarget(other.transform);
            }

            // If is already following a leader AND walks into Taker sphere AND new taker charisma < this.currentCharisma -> Attack.
            if ((currentState == State.FollowPlayer || currentState == State.FollowOther) && other.gameObject.GetComponentInParent<SphereOfInfluence>().currentCharisma < currentCharisma)
            {
                //Debug.Log(this.name + " attack " + other.gameObject.name);
                SetAttackTarget(other.transform);
            }
        }
    }

    private void OnMouseDown()
    {
        // If state is clickable and player charisma > this entity's charisma, start following the player when clicked.
        if (isClickable == true && (currentCharisma < player.GetComponentInParent<SphereOfInfluence>().currentCharisma || currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma < player.GetComponent<SphereOfInfluence>().currentCharisma))
        {
            //Debug.Log("Clicked on " + this.name);
            SetFollowTarget(player.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isClickable == true && (currentTarget == null || currentTarget.tag != "Player"))
        {
            isClickable = false;
            ChangeMaterial(idleMaterial);
        }
    }

    // Acquire a new target to follow.
    public void SetFollowTarget(Transform newTarget)
    {
        if (currentTarget == null || newTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma > currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma)
        {
            newTarget.GetComponentInParent<SphereOfInfluence>().GainFollower(this.gameObject);

            if (newTarget.tag == "Player")
            {
                currentState = State.FollowPlayer;
            }
            else if (newTarget.tag == "Taker")
            {
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
        if (Vector3.Distance(transform.position, currentTarget.transform.position) > minDistanceToTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, moveSpeed * Time.deltaTime);

            // Auto rotate towards the target.
            Vector3 targetDirection = currentTarget.transform.position - transform.position;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
            //Debug.DrawRay(transform.position, newDirection, Color.red);

            // Move position a step towards to the target.
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    private void FollowAndAttackTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, enemyTarget.transform.position, attackStateSpeed * Time.deltaTime);
        distanceToEnemy = Vector3.Distance(transform.position, enemyTarget.transform.position);

        // Auto rotate towards the target.
        Vector3 targetDirection = enemyTarget.transform.position - transform.position;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
        //Debug.DrawRay(transform.position, newDirection, Color.red);

        // Move position a step towards to the target.
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    public void SetAttackTarget(Transform newEnemy)
    {
        currentState = State.Attack;
        //newEnemy.gameObject.transform.parent = enemyTarget;
        enemyTarget = newEnemy;
        //Debug.Log(this.name + " attacks " + enemyTarget.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If attacking, damage attack target.
        if (currentState == State.Attack && (collision.collider.tag == "Taker" || (collision.collider.tag == "Follower" && collision.gameObject.GetComponent<FollowerManager>().currentTarget != null && currentTarget != collision.gameObject.GetComponent<FollowerManager>().currentTarget)))
        {
            collision.gameObject.GetComponent<HitPointsManager>().RegisterHit(attackDamage);
        }
    }

    private void RunAway()
    {

    }

    public void ModifyCharisma(int change)
    {
        currentCharisma += change;
        currentCharisma = Mathf.Clamp(currentCharisma, minCharisma, maxCharisma);
    }

    private void ChangeMaterial(Material newMaterial)
    {
        // Change children materials to indicate a change of state.
        foreach (Renderer renderer in children)
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

    private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
        }
}