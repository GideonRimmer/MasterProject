using UnityEngine;

public class MoveTest : MonoBehaviour
{
    public float moveSpeed = 5f;
    Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rigidbody.linearVelocity = transform.forward * moveSpeed;
            //rigidbody.velocity = transform.up * moveSpeed;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rigidbody.linearVelocity = transform.right * moveSpeed;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rigidbody.linearVelocity = transform.right * -moveSpeed;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            rigidbody.linearVelocity = transform.forward * -moveSpeed;
            //rigidbody.velocity = transform.up * -moveSpeed;
        }

    }
}
