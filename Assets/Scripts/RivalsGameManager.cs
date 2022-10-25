using System.Collections.Generic;
using UnityEngine;

public class RivalsGameManager : MonoBehaviour
{
    public bool minigameActive;
    private PlayerController player;
    public TakerManager rivalTaker;
    public float takerClickMaxTime = 0.2f;
    [SerializeField] private float takerClickTime;
    [SerializeField] private List<GameObject> playerFollowers;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        takerClickTime = takerClickMaxTime;
        playerFollowers = player.GetComponentInParent<SphereOfInfluence>().activeFollowers;
    }

    void Update()
    {
        if (minigameActive == true)
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

                        followerManager.overrideTarget = true;
                        followerManager.SetFollowLeader(player.transform);
                    }
                }
            }

            takerClickTime -= Time.deltaTime;
            if (takerClickTime <= 0)
            {
                //Debug.Log("Click");

                if (playerFollowers.Count > 0)
                {
                    int indexNumber = Random.Range(0, playerFollowers.Count);
                    Debug.Log(indexNumber);
                    TakerOverride(indexNumber);
                }

                takerClickTime = takerClickMaxTime;
            }
        }
    }

    void TakerOverride(int index)
    {
        FollowerManager followerManager = playerFollowers[index].GetComponentInParent<FollowerManager>();

        followerManager.overrideTarget = true;
        followerManager.SetFollowLeader(rivalTaker.transform);
    }
}
