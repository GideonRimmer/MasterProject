using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAtDistance : MonoBehaviour
{
    private Rigidbody rigidbody;
    private bool isBouncing;

    [SerializeField] float moveTowardsSpeed = 12f;
    [SerializeField] float rotateSpeed = 20f;

    // Move to player parameters.
    [SerializeField] private bool isFollowingPlayer = false;
    [SerializeField] private float distanceToClosestPlayer;
    [SerializeField] private int searchPlayerRadius;
    [SerializeField] private int minDistanceFromPlayer;
    public List<string> playerTag;

    // Move to taker parameters.
    [SerializeField] private bool isMovingToTaker = false;
    [SerializeField] private float distanceToClosestTaker;
    [SerializeField] private int searchTakerRadius;
    [SerializeField] private int minDistanceFromTaker;
    private string takerTag;

    // Attack parameters.
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private int attackDamage = 1;
    public List<string> enemyTags;

    // Follow generic leader parameters.
    [SerializeField] bool isMovingTowards;
    [SerializeField] float distanceToClosestLeader;
    private List<string> leaderTag = new List<string>();
    private int searchLeaderRadius;
    private int minDistanceFromLeader;


    void Start()
    {
        // Set intial bools to false.
        isAttacking = false;
        isMovingToTaker = false;
        isFollowingPlayer = false;
        isBouncing = false;

        // Set initial parametes.
        searchTakerRadius = 400;
        minDistanceFromTaker = 50;
        takerTag = "Taker";

        searchPlayerRadius = 500;
        minDistanceFromPlayer = 50;
        playerTag = new List<string>()
        {
            "Player"
        };

        enemyTags = new List<string>()
        {
            "Taker",
            "Enemy"
        };

        // Follow generic leader parameters.
        searchLeaderRadius = 400;
        minDistanceFromLeader = 50;
        leaderTag = new List<string>()
        {
            // Entities will prioritize following the tag that's higher in the list.
            "Taker",
            "Player"
        };
    }

    void Update()
    {
        // Test: Trigger attack on key
        if (Input.GetKeyDown(KeyCode.P))
        {
            Attack("Taker");
        }

        // Dog can't be seduced by takers.
        if (this.tag == "Dog")
        {
            FindPlayerInRadius();
        }

        // "State machine" priority: Attacking -> FindTaker -> FindPlayer.
        if (isAttacking == true)
        {
            FindTakerInRadius(true);
        }
        if (isAttacking == false && this.tag == "Follower")
        {
            FindTakerInRadius(false);
            if (isMovingToTaker == false)
            {
                FindPlayerInRadius();
            }
        }

        //FindLeaderInRadius();
    }

    void FindTakerInRadius(bool attacking)
    {
        // Set the default distance to infinity, so we can revert to a value.
        distanceToClosestTaker = Mathf.Infinity;
        // Set the default closest target to null.
        GameObject closestTarget = null;

        // Create an array of all GameObjects with the the tag "target" in the scene.
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag(takerTag);

        foreach (GameObject currentTarget in allTargets)
        {
            // Find the distance between this GameObject and each target's position.
            float distanceToTarget = (currentTarget.transform.position - this.transform.position).sqrMagnitude;
            // If the distance to the target is less than the distance to other targets, set currentTarget to be closestTarget.
            if (distanceToTarget < distanceToClosestTaker)
            {
                distanceToClosestTaker = distanceToTarget;
                closestTarget = currentTarget;
            }
        }

        if (closestTarget != null && distanceToClosestTaker <= searchTakerRadius && distanceToClosestTaker > minDistanceFromTaker)
        {
            if (attacking == true || distanceToClosestTaker > searchTakerRadius)
            {
                isMovingToTaker = false;
            }
            else isMovingToTaker = true;

            // Automatically move towards the target.
            transform.position = Vector3.MoveTowards(transform.position, closestTarget.transform.position, moveTowardsSpeed * Time.deltaTime);

            // Auto rotate towards the target.
            Vector3 targetDirection = closestTarget.transform.position - transform.position;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
            Debug.DrawRay(transform.position, newDirection, Color.red);

            // Move position a step towards to the target.
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    void FindPlayerInRadius()
    {
        // Set the default distance to infinity, so we can revert to a value.
        distanceToClosestPlayer = Mathf.Infinity;
        // Set the default closest target to null.
        GameObject closestTarget = null;
        // Create an array of all GameObjects with the the tag "target" in the scene.
        foreach (string tag in playerTag)
        {
            GameObject[] allTargets = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject currentTarget in allTargets)
            {
                // Find the distance between this GameObject and each target's position.
                float distanceToTarget = (currentTarget.transform.position - this.transform.position).sqrMagnitude;
                // If the distance to the target is less than the distance to other targets, set currentTarget to be closestTarget.
                if (distanceToTarget < distanceToClosestPlayer)
                {
                    distanceToClosestPlayer = distanceToTarget;
                    closestTarget = currentTarget;
                }
            }

            if (closestTarget != null && distanceToClosestPlayer <= searchPlayerRadius && distanceToClosestPlayer > minDistanceFromPlayer)
            {
                isFollowingPlayer = true;
                // Automatically move towards the target.
                transform.position = Vector3.MoveTowards(transform.position, closestTarget.transform.position, moveTowardsSpeed * Time.deltaTime);

                // Auto rotate towards the target.
                Vector3 targetDirection = closestTarget.transform.position - transform.position;

                Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
                Debug.DrawRay(transform.position, newDirection, Color.red);

                // Move position a step towards to the target.
                transform.rotation = Quaternion.LookRotation(newDirection);
            }
            else if (closestTarget != null && distanceToClosestPlayer <= minDistanceFromPlayer)
            {
                isFollowingPlayer = false;
            }
        }
    }

    void FindLeaderInRadius()
    {
        // Set the default distance to infinity, so we can revert to a value.
        distanceToClosestLeader = Mathf.Infinity;
        // Set the default closest target to null.
        GameObject closestTarget = null;
        // Create an array of all GameObjects with the the tag "target" in the scene.
        foreach (string tag in leaderTag)
        {
            GameObject[] allTargets = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject currentTarget in allTargets)
            {
                // Find the distance between this GameObject and each target's position.
                float distanceToTarget = (currentTarget.transform.position - this.transform.position).sqrMagnitude;
                // If the distance to the target is less than the distance to other targets, set currentTarget to be closestTarget.
                if (distanceToTarget < distanceToClosestLeader)
                {
                    distanceToClosestLeader = distanceToTarget;
                    closestTarget = currentTarget;
                }
            }

            if (closestTarget != null && distanceToClosestLeader > minDistanceFromLeader && distanceToClosestLeader <= searchLeaderRadius)
            {
                Debug.DrawLine(this.transform.position, closestTarget.transform.position);

                isMovingTowards = true;
                // Automatically move towards the target.
                transform.position = Vector3.MoveTowards(transform.position, closestTarget.transform.position, moveTowardsSpeed * Time.deltaTime);

                // Auto rotate towards the target.
                Vector3 targetDirection = closestTarget.transform.position - transform.position;

                Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0.0f);
                Debug.DrawRay(transform.position, newDirection, Color.red);

                // Move position a step towards to the target.
                transform.rotation = Quaternion.LookRotation(newDirection);
            }
            else if (closestTarget != null && distanceToClosestLeader <= minDistanceFromLeader)
            {
                isMovingTowards = false;
            }
        }
    }

    public void Attack(string targetTag)
    {
        isAttacking = true;
        searchTakerRadius = 1000;
        minDistanceFromTaker = 0;

        Debug.Log(this.name + " Attack.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // On collision with enemy, inflict damage.
        if ((collision.collider.tag == "Taker" || collision.collider.tag == "Enemy") && isAttacking)
        {
            //Debug.Log(this.name + " hit taker.");
            collision.gameObject.GetComponent<HitPointsManager>().RegisterHit(attackDamage);

            // TODO: Bounce back after colliding with Taker.
        }
    }
}
