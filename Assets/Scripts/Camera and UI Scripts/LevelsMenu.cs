﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsMenu : MonoBehaviour
{
    public void LoadLevel(int levelNumber)
    {
        SceneManager.LoadScene(levelNumber + 1);
    }
}