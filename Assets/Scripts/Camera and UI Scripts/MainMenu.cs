using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public int previousLevel;
    public GameObject continueButton;

    private void Start()
    {
        // If this is the first time the game is played, disable the "Continue" button.
        if (previousLevel == 0)
        {
            continueButton.SetActive(false);
        }
        else continueButton.SetActive(true);


        //Debug.Log("Scene count: " + SceneManager.sceneCount);
    }

    public void PlayGame()
    {
        previousLevel = PlayerPrefs.GetInt("CurrentLevel");
        Debug.Log("Previous level is " + previousLevel);

        // If this is a new game, load level 01 (build index 2).
        if (previousLevel == 0)
        {
            SceneManager.LoadScene(2);
        }
        // Else, load the next level.
        else
        {
            SceneManager.LoadScene(previousLevel + 1);
        }

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }
}