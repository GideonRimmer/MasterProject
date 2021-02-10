using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OnClickInteraction : MonoBehaviour
{
    public bool isClickable;
    public bool isFollowing;
    public GameObject contextualMenu;
    public GameObject popupMenuPlaceholder;
    public int leaderCharisma;

    public Material idleMaterial;
    public Material followerMaterial;
    Renderer[] children;

    void Start()
    {
        children = GetComponentsInChildren<Renderer>();
        isClickable = false;
        isFollowing = false;
    }

    private void OnMouseDown()
    {
        int loyalty = GetComponent<LoyaltyManager>().currentLoyalty;

        //Debug.Log("Clicked follower " + this.name);

        // Allow the follower to be clicked only of within the player's sphere.
        if (isClickable && loyalty < leaderCharisma)
        {
            isFollowing = true;
            GetComponent<MoveToTarget>().currentTarget.GetComponent<SphereOfInfluence>().GainFollower();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        leaderCharisma = other.gameObject.GetComponentInParent<SphereOfInfluence>().currentCharisma;

        if (other.tag == "Player")
        {
            isClickable = true;

            // Change material to indicate that follower is interactable.
            foreach (Renderer renderer in children)
            {
                var mats = new Material[renderer.materials.Length];
                for (var i = 0; i < renderer.materials.Length; i++)
                {
                    mats[i] = followerMaterial;
                }
                renderer.materials = mats;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isFollowing == false || GetComponent<MoveToTarget>().currentTarget.tag != "Player")
        {
            isClickable = false;

            // Change material back to default if not in sphere and not following player.
            foreach (Renderer renderer in children)
            {
                var mats = new Material[renderer.materials.Length];
                for (var i = 0; i < renderer.materials.Length; i++)
                {
                    mats[i] = idleMaterial;
                }
                renderer.materials = mats;
            }
        }
    }
}
