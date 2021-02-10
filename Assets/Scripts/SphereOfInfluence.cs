using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SphereOfInfluence : MonoBehaviour
{
    public int minCharisma;
    public int maxCharisma;
    public int startingCharisma = 10;
    public int currentCharisma;

    private SphereCollider sphereCollider;
    public float sphereInitialRadius = 10f;
    public float sphereCurrentRadius;

    public TextMeshProUGUI charismaText;
    private Camera mainCamera;

    public List<GameObject> activeFollowers = new List<GameObject>();

    private void Start()
    {
        //sphereCollider = GetComponent<SphereCollider>();
        sphereCollider = GetComponentInChildren<SphereCollider>();
        currentCharisma = startingCharisma;
        sphereCurrentRadius = sphereInitialRadius;
        sphereCollider.radius = sphereCurrentRadius;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // DEBUG: Show charisma text in game.
        charismaText.text = currentCharisma.ToString();
        charismaText.transform.LookAt(mainCamera.transform);
        charismaText.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Follower")
        {
            //Debug.Log(other.name + " entered sphere.");
            other.GetComponentInParent<MoveToTarget>().SetTarget(this.transform);
        }
    }

    public void ModifyCharisma(int change)
    {
        currentCharisma += change;
        currentCharisma = Mathf.Clamp(currentCharisma, minCharisma, maxCharisma);
    }

    public void GainFollower(GameObject followerName)
    {
        Debug.Log("Gain follower");
        ModifyCharisma(1);
        activeFollowers.Add(followerName);
        ChangeAllLoyalty(1);
    }

    public void LoseFollower(GameObject followerName)
    {
        Debug.Log("Lose follower");
        ModifyCharisma(-1);
        activeFollowers.Remove(followerName);
    }

    private void ChangeAllLoyalty(int change)
    {
        foreach (GameObject follower in activeFollowers)
        {
            follower.GetComponent<LoyaltyManager>().ModifyLoyalty(change);
        }
    }
}
