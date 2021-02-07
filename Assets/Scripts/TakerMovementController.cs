using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakerMovementController : MonoBehaviour
{
    public int startingCharisma = 7;
    [SerializeField] private int currentCharisma;
    // Start is called before the first frame update
    void Start()
    {
        currentCharisma = startingCharisma;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
