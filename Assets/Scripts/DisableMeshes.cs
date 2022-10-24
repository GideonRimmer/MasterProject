using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMeshes : MonoBehaviour
{
    public  string tagToDisable;
    public GameObject[] meshes;

    void Start()
    {
        meshes = GameObject.FindGameObjectsWithTag(tagToDisable);
        DisableMeshRenderes();
    }

    void DisableMeshRenderes()
    {
        foreach (GameObject obj in meshes)
        {
            obj.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
