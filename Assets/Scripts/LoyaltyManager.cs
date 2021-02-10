using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoyaltyManager : MonoBehaviour
{
    public int maxLoyalty = 20;
    public int minLoyalty = 3;
    public int autoAttackLoyalty = 10;
    public int startingLoyalty;
    public int currentLoyalty;
    public GameObject leaderEntity;

    public TextMeshProUGUI loyaltyText;

    void Start()
    {
        startingLoyalty = Random.Range(minLoyalty, maxLoyalty);
        currentLoyalty = startingLoyalty;

        loyaltyText.text = currentLoyalty.ToString();
    }

    private void Update()
    {    
        // Test: Auto attack if loyalty > something.
        if (currentLoyalty >= autoAttackLoyalty)
        {

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
}
