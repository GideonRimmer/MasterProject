using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEntitiesAtRandom : MonoBehaviour
{
    public FollowerManager followerPrefab;
    public TakerManager takerPrefab;

    public int minX;
    public int maxX;
    public int minZ;
    public int maxZ;
    private int xPosition;
    private int yPosition = 1;
    private int zPosition;

    public int numberOfFollowers;
    public int numberOfTakers = 10;

    void Start()
    {
        //StartCoroutine(SpawnFollowers());
        //StartCoroutine(SpawnTakers());

        // Spawn followers.
        for (int i = 0; i < numberOfFollowers; i++)
        {
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

    /*
    IEnumerator SpawnTakers()
    {
        while (takerCount < numberOfTakers)
        {
            xPosition = Random.Range(minX, maxX);
            zPosition = Random.Range(minZ, maxZ);
            Instantiate(takerPrefab, new Vector3(xPosition, yPosition, zPosition), Quaternion.identity);

            yield return new WaitForSeconds(0.01f);
            takerCount += 1;
        }
    }
    */
}
