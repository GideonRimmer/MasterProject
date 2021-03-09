using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform Player;
    private Vector3 cameraOffset;

    [Range (0.01f, 1.0f)]
    public float smoothFactor = 0.5f;

    public bool lookAtPlayer = false;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        cameraOffset = this.transform.position - Player.transform.position;
    }

    // LateUpdate is called after all Update functions have been called
    void LateUpdate()
    {
        if (Player != null)
        {
            Vector3 newCameraPosition = Player.transform.position + cameraOffset;

            this.transform.position = Vector3.Slerp(transform.position, newCameraPosition, smoothFactor);

            // Rotate camera in the direction the player is walking
            if (lookAtPlayer == true)
            {
                transform.LookAt(Player);
            }
        }
    }
}
