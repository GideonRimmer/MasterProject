using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object Spawner")] 
public class ObjectSpawner : ScriptableObject
{
    // I want to create an array of prefab + its position.
    public string spawner;
    public LayerMask objectTags;
    public GameObject objectPositions;

    void GetObjectPositions()
    {

    }

    void SpawnObjects()
    {

    }
}
