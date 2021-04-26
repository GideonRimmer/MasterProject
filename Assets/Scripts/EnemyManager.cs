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
    public float minDistanceToTarget;
    //public float maxDistanceToTarget;
    //[SerializeField] private float currentDistanceToTarget;
    public int attackDamage = 1;
    public float attackStateSpeed;
    public float attackTimer = 1.0f;
    [SerializeField] private float attackCurrentTime;
    public Transform enemyTarget;
    public float maxDistanceToTarget;
    [SerializeField] private float currentDistanceToTarget;

    [Header("State Machine")]
    public State currentState;
    public enum State
    {
        Idle,
        Attack,
        Return,
    }

    [Header("Materials")]
    public Material idleMaterial;
    public Material skinMaterial;
    public Material attackMaterial;

    [Header("Body Parts")]
    public Renderer[] skin;
    public Renderer[] clothes;

    [Header("OverlapSphere Parameters")]
    private Collider agentCollider;
    [SerializeField] Collider[] agentsInSphere;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        startingPosition = transform.position;
        currentState = State.Idle;
        attackStateSpeed = moveSpeed + 4;

        // Find the player.
        player = GameObject.FindGameObjectWithTag("Player");

        agentCollider = GetComponent<Collider>();

        // Start with default colors.
        ChangeMaterial(clothes, idleMaterial);
        ChangeMaterial(skin, skinMaterial);
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        
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

        // Stop attacking when running out of eligible targets.
        if (enemyTarget.tag == "Follower" && (enemyTarget == null || enemyTarget.GetComponentInParent<FollowerManager>().currentLeader == currentLeader))
        {
            //Debug.Log(name + ": Target eliminated.");
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
    }

    public void SetAttackTarget(Transform newEnemy)
    {
        currentState = State.Attack;
        enemyTarget = newEnemy;
        //Debug.Log(this.name + " attacks " + enemyTarget.name);
        ChangeMaterial(skin, attackMaterial);
    }

    private void OnCollisionStay(Collision collision)
    {
        // On collision, inflict damage on the enemy target, only if colliding with the enemy target.
        if (enemyTarget != null && collision.gameObject.name == enemyTarget.name)
        {
            // Damage the target every X seconds (attackTimer), then start a cooldown.
            attackCurrentTime -= Time.deltaTime;

            //if (attackCurrentTime <= 0 && (enemyTarget.tag == "Player" || (enemyTarget.tag == "Follower" && (enemyTarget.GetComponentInParent<FollowerManager>().currentLeader.tag == "Player" || enemyTarget.GetComponentInParent<FollowerManager>().currentLeader.tag == null))))
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
    public void Die()
    {
        Debug.Log(this.name + "register death");
        GetComponent<HitPointsManager>().PlayParticleSystem();
        Destroy(this.gameObject);
    }

}
