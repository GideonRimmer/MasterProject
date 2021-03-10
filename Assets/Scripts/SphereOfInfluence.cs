using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SphereOfInfluence : MonoBehaviour
{
    public Transform followPoint;
    public int minCharisma;
    public int maxCharisma;
    public int startingCharisma = 10;
    public int currentCharisma;

    private SphereCollider sphereCollider;
    public float sphereInitialRadius;
    public float sphereCurrentRadius;

    public TextMeshProUGUI charismaText;
    private Camera mainCamera;

    public List<GameObject> activeFollowers = new List<GameObject>();

    private void Start()
    {
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

    public void ModifyCharisma(int change)
    {
        currentCharisma += change;
        currentCharisma = Mathf.Clamp(currentCharisma, minCharisma, maxCharisma);
    }

    public void GainFollower(GameObject followerName)
    {
        //Debug.Log(this. name + " Gained follower");
        ModifyCharisma(1);

        
        if (activeFollowers.Contains(followerName) == false)
        {
            activeFollowers.Add(followerName);
        }
        
        ChangeAllCharisma(1);
    }

    public void LoseFollower(GameObject followerName)
    {
        //Debug.Log(this.name + " Lost follower");
        ModifyCharisma(-1);
        activeFollowers.Remove(followerName);
    }

    public void RemoveDeadFollower(GameObject followerName)
    {
        Debug.Log(followerName + "index " + activeFollowers.IndexOf(followerName));
        activeFollowers.RemoveAt(activeFollowers.IndexOf(followerName));
        ModifyCharisma(-1);
    }

    private void ChangeAllCharisma(int change)
    {
        foreach (GameObject follower in activeFollowers)
        {
            //follower.GetComponent<LoyaltyManager>().ModifyLoyalty(change);
            follower.GetComponent<FollowerManager>().ModifyCharisma(change);
        }
    }
}
