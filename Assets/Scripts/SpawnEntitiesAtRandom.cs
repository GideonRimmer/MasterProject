using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEntitiesAtRandom : MonoBehaviour
{
    [Header("Entity Prefabs")]
    public FollowerManager followerPrefab;
    public TakerManager takerPrefab;

    [Header("Spawn Limits")]
    public Transform limitMarkerMin;
    public Transform limitMarkerMax;
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    private float xPosition;
    private float yPosition = 1;
    private float zPosition;

    [Header("Entity Parameters")]
    public int numberOfFollowers;
    public int numberOfTakers = 10;
    public int followerMinChar = 7;
    public int followerMaxChar = 25;
    public int takerMinChar = 9;
    public int takerMaxChar = 20;

    void Start()
    {
        // Spawn followers.
        for (int i = 0; i < numberOfFollowers; i++)
        {
            minX = limitMarkerMin.position.x;
            minZ = limitMarkerMin.position.z;
            maxX = limitMarkerMax.position.x;
            maxZ = limitMarkerMax.position.z;

            xPosition = Random.Range(minX, maxX);
            zPosition = Random.Range(minZ, maxZ);
            
            FollowerManager newFollower = Instantiate(followerPrefab, new Vector3(xPosition, yPosition, zPosition), Quaternion.Euler(Vector3.up * Random.Range(0, 360)));
            newFollower.name = "Follower" + i;
        }

        // Spawn takers.
        for (int i = 0; i < numberOfTakers; i++)
        {
            xPosition = Random.Range(minX, maxX);
            zPosition = Random.Range(minZ, maxZ);

            TakerManager newTaker = Instantiate(takerPrefab, new Vector3(xPosition, yPosition, zPosition), Quaternion.Euler(Vector3.up * Random.Range(0, 360)));
            newTaker.name = "Taker" + i;
        }
    }
}
