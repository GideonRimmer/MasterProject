using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakerManager : MonoBehaviour
{
    public int minCharisma;
    public int maxCharisma;
    [SerializeField] private int randomCharisma;

    void Awake()
    {
        randomCharisma = Random.Range(minCharisma, maxCharisma + 1);
        this.GetComponent<SphereOfInfluence>().startingCharisma = randomCharisma;
    }
}
