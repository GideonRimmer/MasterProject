﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SphereOfInfluence : MonoBehaviour
{
    [Header("Charisma")]
    public bool drawGizmos;
    public int minCharisma;
    public int maxCharisma;
    public int startingCharisma = 10;
    public int currentCharisma;

    [Header("Energy")]
    public int startingEnergy = 10;
    public int currentEnergy;
    public int maxEnergy;
    public float energyRegenMaxTime = 0.5f;
    [SerializeField] private float energyRegenCurrentTime;

    [Header("Violence level")]
    public int startingViolence = 0;
    public int currentViolence;

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

        currentEnergy = startingEnergy;
        maxEnergy = startingEnergy;
        energyRegenCurrentTime = energyRegenMaxTime;
    }

    private void Update()
    {
        // DEBUG: Show charisma text in game.
        charismaText.text = currentCharisma.ToString();
        charismaText.transform.LookAt(mainCamera.transform);
        charismaText.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);

        /*
        // Manifest sphere when pressing a key.
        if (Input.GetKeyDown(KeyCode.P))
        {
            ConvinceInRadius(sphereCurrentRadius);
        }

        // Regenerate energy at a fixed rate.
        if (currentEnergy < maxEnergy)
        {
            energyRegenCurrentTime -= Time.deltaTime;
            if (energyRegenCurrentTime <= 0)
            {
                currentEnergy += 1;
                energyRegenCurrentTime = energyRegenMaxTime;
            }
        }
        */
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
    public void RemoveFollower(GameObject followerName)
    {
        Debug.Log(followerName + "index " + activeFollowers.IndexOf(followerName));
        activeFollowers.RemoveAt(activeFollowers.IndexOf(followerName));
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
            if (follower != null)
            {
                //follower.GetComponent<LoyaltyManager>().ModifyLoyalty(change);
                follower.GetComponent<FollowerManager>().ModifyCharisma(change);
            }
        }
    }

    void ConvinceInRadius(float radius)
    {
        /*
        if (currentEnergy >= 1)
        {
            currentEnergy -= 1;
        }
        */

        LayerMask layerMask = LayerMask.GetMask("Characters");
        Collider[] agentsInSphere = Physics.OverlapSphere(transform.position, radius, layerMask);
        foreach (Collider agent in agentsInSphere)
        {
            FollowerManager followerScript = agent.GetComponentInParent<FollowerManager>();

            if (agent.tag == "Follower" && currentEnergy > 0 && followerScript.currentCharisma < currentCharisma)
            {
                followerScript.SetFollowLeader(this.transform);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos == true)
        {
            Gizmos.DrawWireSphere(transform.position, sphereCurrentRadius);
        }
    }
}
