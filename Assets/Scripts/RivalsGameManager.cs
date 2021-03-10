using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivalsGameManager : MonoBehaviour
{
    public bool minigameActive = false;
    private PlayerController player;
    public TakerManager rivalTaker;
    public float takerClickMaxTime = 0.2f;
    [SerializeField] private float takerClickTime;
    [SerializeField] private List<GameObject> playerFollowers;
    [SerializeField] List<GameObject> playerSphere;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        takerClickTime = takerClickMaxTime;
        playerSphere = player.GetComponentInParent<SphereOfInfluence>().activeFollowers;
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
                        followerManager.SetFollowTarget(player.GetComponentInParent<SphereOfInfluence>().followPoint);
                    }
                }
            }

            takerClickTime -= Time.deltaTime;
            if (takerClickTime <= 0)
            {
                //Debug.Log("Click");

                if (playerSphere.Count > 0)
                {
                    int indexNumber = Random.Range(0, playerSphere.Count);
                    Debug.Log(indexNumber);
                    TakerOverride(indexNumber);
                }

                takerClickTime = takerClickMaxTime;
            }
        }
    }

    void TakerOverride(int index)
    {
        FollowerManager followerManager = playerSphere[index].GetComponentInParent<FollowerManager>();

        followerManager.overrideTarget = true;
        followerManager.SetFollowTarget(rivalTaker.GetComponentInParent<SphereOfInfluence>().followPoint);
    }
}
