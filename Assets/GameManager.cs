using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void Update()
    {
        // Restart by pressing R.
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("TestLevel_01");
        }

        // Quit the game by pressing ESC.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Go to the next level by pressing L.
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void LoadNextLevel(int levelNumber)
    {
        SceneManager.LoadScene(levelNumber);
    }
}
