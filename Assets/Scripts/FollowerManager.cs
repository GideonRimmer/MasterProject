using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FollowerManager : MonoBehaviour
{
    private Animator animator;
    private enum State
    {
        Idle,
        FollowPlayer,
        FollowOther,
        Attack,
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

    public TextMeshProUGUI charismaText;
    private Camera mainCamera;

    public Material idleMaterial;
    public Material followerMaterial;
    Renderer[] children;

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
    }

    void Update()
    {
        // DEBUG: Show charisma text in game.
        charismaText.text = currentCharisma.ToString();
        charismaText.transform.LookAt(mainCamera.transform);
        charismaText.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);


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
        if (isClickable == true && (currentCharisma < player.GetComponent<SphereOfInfluence>().currentCharisma || currentTarget.GetComponentInParent<SphereOfInfluence>().currentCharisma < player.GetComponent<SphereOfInfluence>().currentCharisma))
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If attacking, damage attack target.
        if (collision.collider.tag == "Taker" && currentState == State.Attack)
        {
            collision.gameObject.GetComponent<HitPointsManager>().RegisterHit(attackDamage);
        }
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
}