using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEntitiesAtRandom : MonoBehaviour
{
    public GameObject followerPrefab;
    public GameObject takerPrefab;

    public int minX;
    public int maxX;
    public int minZ;
    public int maxZ;
    private int xPosition;
    private int yPosition = 1;
    private int zPosition;

    public int followerCount;
    public int numberOfFollowers;
    private int takerCount;
    public int numberOfTakers = 10;

    void Start()
    {
        StartCoroutine(SpawnFollowers());
        StartCoroutine(SpawnTakers());
    }

    IEnumerator SpawnFollowers()
    {
        while (followerCount < numberOfFollowers)
        {
            xPosition = Random.Range(minX, maxX);
            zPosition = Random.Range(minZ, maxZ);
            Instantiate(followerPrefab, new Vector3(xPosition, yPosition, zPosition), Quaternion.identity);

            yield return new WaitForSeconds(0.01f);
            followerCount += 1;
        }
    }

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
}
