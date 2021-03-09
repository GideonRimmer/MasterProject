using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivalsGameManager : MonoBehaviour
{
    private PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.gameObject.tag == "Follower" && hit.transform.GetComponentInParent<FollowerManager>().currentState == FollowerManager.State.FollowOther)
                {
                    FollowerManager followerManager = hit.transform.GetComponentInParent<FollowerManager>();
                    Debug.Log("Clicked on " + hit.transform.gameObject.name);

                    followerManager.SetFollowTarget(player.transform);

                    // TODO:
                    // 1. Create a new function OverrideFollowTarget, that ignores all other parameters to set the follow target.
                    // 2. Create an indication of follower who is in transition. Test before, might not be necessary.
                    // 3. Simple AI for when the taker "clicks" on one of the player's followers.
                    // 4. Change follower color to indicate currentTarget (player or taker).
                    // 5. Traitors?
                }
            }
        }
    }
}
