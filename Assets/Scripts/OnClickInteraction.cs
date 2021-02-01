using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OnClickInteraction : MonoBehaviour
{
    public GameObject contextualMenu;
    public GameObject popupMenuPlaceholder;

    public Material idleMaterial;
    public Material followerMaterial;
    Renderer[] children;

    void Start()
    {
        children = GetComponentsInChildren<Renderer>();
    }

    private void OnMouseDown()
    {
        // Change the materials of all children. Test for interaction, might remove later.
        foreach (Renderer renderer in children)
        {
            var mats = new Material[renderer.materials.Length];
            for (var i = 0; i < renderer.materials.Length; i++)
            {
                mats[i] = followerMaterial;
            }
            renderer.materials = mats;
        }

        //Instantiate(contextualMenu, popupMenuPlaceholder.transform.position, Quaternion.identity);
    }
}
