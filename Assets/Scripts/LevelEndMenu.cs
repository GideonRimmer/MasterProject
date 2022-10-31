using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndMenu : MonoBehaviour
{
    public int previousLevel;
    public GameObject nextLevelButton;

    private void Start()
    {
        if (previousLevel >= SceneManager.sceneCount + 1)
        {
            nextLevelButton.SetActive(false);
        }

        //Debug.Log("Scene count: " + SceneManager.sceneCount);
    }

    public void PlayNextLevel()
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
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
