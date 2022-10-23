using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLevelObjects : MonoBehaviour
{
    public GameObject levelObjects;

    void Awake()
    {
        if (levelObjects != null)
        {
            Instantiate(levelObjects, transform.position, Quaternion.identity);
        }
    }
}
