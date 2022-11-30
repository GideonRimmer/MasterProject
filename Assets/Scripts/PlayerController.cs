//using UnityEditor.Timeline;
//using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameManager gameSettings;

    [Header("Movement")]
    public CharacterController controller;
    public float speed = 9f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Gravity")]
    public Transform groundCheck;
    public float gravityModifier = -9.81f;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    [SerializeField] private bool isGrounded;
    private Vector3 velocity;

    [SerializeField] private bool isIdle;
    [SerializeField] private bool isWalking;
    public Animator animator;

    //[SerializeField] float moveSpeed = 4f;
    //private Vector3 forward, right;

    [Header("Charisma")]
    public int startingCharisma = 5;
    [SerializeField] private int currentCharisma;
    private HitPointsManager hitPointsManager;
    private PlayParticleEffect playParticleEffect;
    public bool autoCollectFollowers;

    public AudioClip deathSound;

    void Start()
    {
        gameSettings = FindObjectOfType<GameManager>();
        isIdle = true;
        isWalking = false;
        //animator = GetComponentInChildren<Animator>();

        /*
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        */

        currentCharisma = startingCharisma;

        hitPointsManager = GetComponent<HitPointsManager>();
        playParticleEffect = GetComponent<PlayParticleEffect>();
    }

    private void Update()
    {
        // Movement axes.
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Movement and facing.
        if (direction.magnitude >= 0.1f)
        {
            isIdle = false;
            isWalking = true;
            animator.SetBool("isWalking", true);
            
            // Move the player using that CharacterController's Move function.
            controller.Move(direction * speed * Time.deltaTime);

            // Rotate the player to face the correct direction.
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            // Smooth the turning angle.
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
            isIdle = true;
            isWalking = false;
            animator.SetBool("isWalking", false);
        }

        // Set up the ground check, and check if the player is grounded.
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded == true && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Implement non-physics gravity.
        velocity.y += gravityModifier * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Kill the player if HP is 0 or below.
        if (hitPointsManager.currentHitPoints <= 0)
        {
            Die();
        }
    }



    private void Die()
    {
        AudioSource.PlayClipAtPoint(deathSound, transform.position);

        gameSettings.gameOver = true;
        Destroy(this.gameObject);
    }
}
