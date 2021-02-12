using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Update()
    {
        // Restart by pressing R.
        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(0);
        }

        // Quit the game by pressing ESC.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
