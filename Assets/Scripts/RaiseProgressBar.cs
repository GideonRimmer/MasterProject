using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseProgressBar : MonoBehaviour
{
    public GameObject playerProgressBar;
    public float playerRaiseAmount;
    public float playerDropRate;

    public GameObject enemyProgressBar;
    public float enemyRaiseAmount;
    private float enemyNextClick = 0.0f;
    public float enemyClickRate;
    public float enemyDropRate;

    void Start()
    {

    }

    void Update()
    {
        // Raise player progress bar when clicking Space.
        //if (Input.GetKeyDown(KeyCode.Space))
        if (Input.anyKeyDown)
            {
            playerProgressBar.GetComponent<ProgressBar>().currentProgress += playerRaiseAmount;
        }

        // Lower player progress bar at a steady rate.
        playerProgressBar.GetComponent<ProgressBar>().currentProgress -= playerDropRate;

        // Raise enemy progress bar at a steady rate.
        if (Time.time > enemyNextClick)
        {
            enemyNextClick += enemyClickRate;
            enemyProgressBar.GetComponent<ProgressBar>().currentProgress += enemyRaiseAmount;
        }
        // Lower enemy progress bar at a steady rate.
        enemyProgressBar.GetComponent<ProgressBar>().currentProgress -= enemyDropRate;

    }
}
