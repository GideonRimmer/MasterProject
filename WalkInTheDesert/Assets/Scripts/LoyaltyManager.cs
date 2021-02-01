using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoyaltyManager : MonoBehaviour
{
    [SerializeField] private int maxLoyalty = 20;
    [SerializeField] private int minLoyalty = 0;
    [SerializeField] private int startingLoyalty = 5;
    [SerializeField] private int autoAttackLoyalty = 10;
    [SerializeField] private int currentLoyalty;
    [SerializeField] private GameObject leaderEntity;

    void Start()
    {
        currentLoyalty = startingLoyalty;
    }

    private void Update()
    {    
        // Test: Auto attack if loyalty > something.
        if (currentLoyalty >= autoAttackLoyalty)
        {
            this.GetComponent<FollowAtDistance>().Attack("Taker");

            Debug.Log(this.name + " Auto attack.");
        }
    }

    public void ModifyLoyalty(int change)
    {
        currentLoyalty += change;
        currentLoyalty = Mathf.Clamp(currentLoyalty, minLoyalty, maxLoyalty);
    }

    public void SwitchTag(string newTag)
    {
        this.tag = newTag;
    }

    // Test: Increase loyalty on mouse click.
    private void OnMouseDown()
    {
        ModifyLoyalty(1);
        Debug.Log("Modify loyalty");
    }
}
