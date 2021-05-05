using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Timeline;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool isIdle;
    [SerializeField] private bool isWalking;
    public Animator animator;

    [SerializeField] float moveSpeed = 4f;
    private Vector3 forward, right;
    public int startingCharisma = 5;
    [SerializeField] private int currentCharisma;

    private HitPointsManager hitPointsManager;
    private PlayParticleEffect playParticleEffect;

    public bool autoCollectFollowers;

    void Start()
    {
        isIdle = true;
        isWalking = false;
        //animator = GetComponentInChildren<Animator>();

        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

        currentCharisma = startingCharisma;

        hitPointsManager = GetComponent<HitPointsManager>();
        playParticleEffect = GetComponent<PlayParticleEffect>();
    }

    void Update()
    {
        if (Input.anyKey)
        {
            if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
            {
                return;
            }
            else Move();
        }
        else
        {
            isIdle = true;
            isWalking = false;
            animator.SetBool("isWalking", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Clicked space.");
        }

        if (hitPointsManager.currentHitPoints <= 0)
        {
            Die();
        }
    }

    void Move()
    {
        isIdle = false;
        isWalking = true;
        animator.SetBool("isWalking", true);

        Vector3 direction = new Vector3(Input.GetAxis("HorizontalKey"), 0, Input.GetAxis("VerticalKey"));
        Vector3 rightMovement = right * moveSpeed * Time.deltaTime * Input.GetAxis("HorizontalKey");
        Vector3 upMovement = forward * moveSpeed * Time.deltaTime * Input.GetAxis("VerticalKey");

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
    }

    private void Die()
    {
        playParticleEffect.PlayParticleSystem();
        Destroy(this.gameObject);
    }
}
