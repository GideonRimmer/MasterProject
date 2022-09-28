using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevelOnTrigger : MonoBehaviour
{
    GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Active scene is " + SceneManager.GetActiveScene().buildIndex);
            gameManager.LoadLevelEndMenu(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
