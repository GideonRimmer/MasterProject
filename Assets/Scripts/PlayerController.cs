using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Timeline;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 4f;
    private Vector3 forward, right;
    public int startingCharisma = 5;
    [SerializeField] private int currentCharisma;

    void Start()
    {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

        currentCharisma = startingCharisma;
    }

    void Update()
    {
        if (Input.anyKey)
        {
            Move();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Clicked space.");
        }
    }

    void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxis("HorizontalKey"), 0, Input.GetAxis("VerticalKey"));
        Vector3 rightMovement = right * moveSpeed * Time.deltaTime * Input.GetAxis("HorizontalKey");
        Vector3 upMovement = forward * moveSpeed * Time.deltaTime * Input.GetAxis("VerticalKey");

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
    }
}
