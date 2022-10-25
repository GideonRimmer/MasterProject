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
    private Camera mainCamera;

    void Start()
    {
        startingLoyalty = Random.Range(minLoyalty, maxLoyalty);
        currentLoyalty = startingLoyalty;

        mainCamera = Camera.main;
    }

    private void Update()
    {
        // DEBUG: Show loyalty text in game.
        loyaltyText.text = currentLoyalty.ToString();
        loyaltyText.transform.LookAt(mainCamera.transform);
        loyaltyText.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);

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
