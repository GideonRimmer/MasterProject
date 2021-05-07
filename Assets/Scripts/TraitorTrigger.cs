using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // When a Player Follower enters this trigger, turn on canBeTraitor.
        if (other.CompareTag("Follower") && other.GetComponentInParent<FollowerManager>() != null && other.GetComponentInParent<FollowerManager>().currentLeader.CompareTag ("Player"))
        {
            Debug.Log(other.gameObject.name + " Can be traitor.");
            other.GetComponentInParent<FollowerManager>().canBeTraitor = true;
        }
    }
}
