using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool gameIsPaused = false;

    void Update()
    {
        // Restart by pressing R.
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

        // Pause and unpause the game by pressing P.
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (gameIsPaused == false)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public void LoadNextLevel(int levelNumber)
    {
        SceneManager.LoadScene(levelNumber);
    }

    private void PauseGame()
    {
        gameIsPaused = true;
        Time.timeScale = 0;
    }

    private void ResumeGame()
    {
        gameIsPaused = false;
        {
            Time.timeScale = 1;
        }
    }
}
