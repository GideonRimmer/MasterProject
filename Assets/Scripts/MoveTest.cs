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
            rigidbody.velocity = transform.forward * moveSpeed;
            //rigidbody.velocity = transform.up * moveSpeed;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rigidbody.velocity = transform.right * moveSpeed;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rigidbody.velocity = transform.right * -moveSpeed;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            rigidbody.velocity = transform.forward * -moveSpeed;
            //rigidbody.velocity = transform.up * -moveSpeed;
        }

    }
}
