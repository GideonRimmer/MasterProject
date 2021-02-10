using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SphereOfInfluence : MonoBehaviour
{
    public int startingCharisma = 10;
    public int currentCharisma;

    private SphereCollider sphereCollider;
    public float sphereInitialRadius = 10f;
    public float sphereCurrentRadius;

    public TextMeshProUGUI charismaText;

    private void Start()
    {
        //sphereCollider = GetComponent<SphereCollider>();
        sphereCollider = GetComponentInChildren<SphereCollider>();
        currentCharisma = startingCharisma;
        sphereCurrentRadius = sphereInitialRadius;
        sphereCollider.radius = sphereCurrentRadius;

        // DEBUG: Show charisma text in game.
        charismaText.text = currentCharisma.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Follower")
        {
            //Debug.Log(other.name + " entered sphere.");
            other.GetComponentInParent<MoveToTarget>().SetTarget(this.transform);
        }
    }
}
