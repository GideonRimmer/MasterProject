using UnityEngine;

public class DogMovementController : MonoBehaviour
{
    public Transform Target;
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] float minDistanceFromTarget = 5f;
    [SerializeField] float currentDistance;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentDistance = Vector3.Distance(this.transform.position, Target.transform.position);

        if (currentDistance >= minDistanceFromTarget)
        {
            transform.LookAt(Target.position);
            transform.Translate(0.0f, 0.0f, moveSpeed * Time.deltaTime);

        }
    }
}
