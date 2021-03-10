﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FollowerManager : MonoBehaviour
{
    [Header("Setup")]
    public Animator animator;
    private Rigidbody rigidbody;
    private float sphereRadius = 13f;
    public GameObject player;
    public int minCharisma;
    public int maxCharisma;
    public int minStartingCharisma;
    public int maxStartingCharisma;
    public int startingCharisma;
    public int currentCharisma;
    public TextMeshProUGUI charismaText;
    private Camera mainCamera;

    [Header("State Machine")]
    public State currentState;
    [SerializeField] private bool isClickable;
    public enum State
    {
        Idle,
        FollowPlayer,
        FollowOther,
        Attack,
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
        animator = GetComponent<Animator>();

        currentState = State.Idle;
        isClickable = false;
        attackStateSpeed = moveSpeed + 2;

        // Generate random charisma.
        startingCharisma = Random.Range(minStartingCharisma, maxStartingCharisma);
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

                // Attack followers in range who are attacking your leader.
                if (currentTarget != null && agentFollower.currentTarget != null && currentTarget != agentFollower.currentTarget && agentFollower.currentState == State.Attack)
                {
                    SetAttackTarget(agent.transform);
                }
            }

            // Also attack any Innocents in range (MUWAHAHAHA).
            else if (agent.tag == "Innocent" && currentTarget != null)
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
                //ChangeMaterial(idleMaterial);
                break;

            case State.FollowPlayer:
                if (currentTarget != null)
                {
                    FollowTarget();
                    //animator.SetBool("isWalking", true);
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
                if (enemyTarget != null && currentTarget != null)
                {
                    FollowAndAttackTarget();
                    animator.SetBool("isWalking", true);
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
        if (other.gameObject.tag == "Taker")
        {
            SphereOfInfluence takerSphere = other.gameObject.GetComponentInParent<SphereOfInfluence>();
            if ((currentState == State.Idle && takerSphere.currentCharisma > currentCharisma) || ((currentState == State.FollowOther || currentState == State.FollowPlayer) && takerSphere.currentCharisma > currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma))
            {
                SetFollowTarget(other.transform);
            }

            // If is already following a leader AND walks into Taker sphere AND new taker charisma < this.currentCharisma -> Attack.
            if ((currentState == State.FollowPlayer || currentState == State.FollowOther) && takerSphere.currentCharisma < currentCharisma)
            {
                //Debug.Log(this.name + " attack " + other.transform.gameObject.name);
                SetAttackTarget(other.transform.parent.gameObject.transform);
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
            ChangeMaterial(clothes, idleMaterial);
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
            animator.SetBool("isWalking", true);
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

    private void OnCollisionEnter(Collision collision)
    {
        // On collision, inflict damage on the enemy target, only if colliding with the enemy target.
        if (enemyTarget != null && collision.gameObject.name == enemyTarget.name)
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

    /*
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
    */
}