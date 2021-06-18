using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public bool gameOver;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public float gameOverCountdown = 3.0f;
    [SerializeField] private float currentCountdown;
    public float maxIdleSeconds = 300f;
    [SerializeField] private float currentIdleSeconds;

    public Texture2D knifeCursor;
    public Texture2D defaultCursor;

    private void Start()
    {
        #if UNITY_EDITOR
                Debug.Log("Unity Editor");
        #endif

        #if UNITY_IOS
              Debug.Log("Iphone");
        #endif

        #if UNITY_STANDALONE_OSX
            Debug.Log("Stand Alone OSX");
        #endif

        #if UNITY_STANDALONE_WIN
                Debug.Log("Stand Alone Windows");
#endif

        gameOver = false;
        currentCountdown = gameOverCountdown;
        currentIdleSeconds = 0f;
    }

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
            if (gameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
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

        if (gameOver == true)
        {
            currentCountdown -= Time.deltaTime;
            if (currentCountdown <= 0)
            {
                GameOver();
            }
        }

        // If the game hasn't been active for X time, go back to main menu.
        if (Input.anyKey)
        {
            currentIdleSeconds = 0f;
        }
        else
        {
            // If no input, start countdown.
            currentIdleSeconds += Time.deltaTime;
            if (currentIdleSeconds >= maxIdleSeconds)
            {
                currentIdleSeconds = 0f;
                LoadNextLevel(0);
            }
        }
    }

    public void LoadNextLevel(int levelNumber)
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelNumber);
    }

    private void PauseGame()
    {
        gameIsPaused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void RestartLevel()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        //Application.Quit();
    }

    public void GameOver()
    {
        gameOver = false;
        //gameIsPaused = true;
        //Time.timeScale = 0f;
        gameOverMenu.SetActive(true);
    }
}
