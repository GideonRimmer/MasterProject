using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class SavePlayerData : MonoBehaviour
{
    [Header ("Text Objects")]
    public TMP_Text currentLevelNumberText;
    public TMP_Text enemiesKilledText;
    public TMP_Text followersRecruitedText;
    public TMP_Text followersKilledText;
    public TMP_Text innocentsKilledText;

    [Header ("Data Numbers")]
    public int currentLevelNumber;
    public int enemiesKilled;
    public int followersRecruited;
    public int followersKilled;
    public int innocentsKilled;

    private void Awake()
    {
        currentLevelNumber = PlayerPrefs.GetInt("CurrentLevel", 1);
        enemiesKilled = PlayerPrefs.GetInt("EnemiesKilled", 0);
        followersRecruited = PlayerPrefs.GetInt("FollowersRecruited", 0);
        followersKilled = PlayerPrefs.GetInt("FollowersKilled", 0);
        innocentsKilled = PlayerPrefs.GetInt("InnocentsKilled", 0);

        currentLevelNumberText.text = currentLevelNumber.ToString();
        enemiesKilledText.text = enemiesKilled.ToString();
        followersRecruitedText.text = followersRecruited.ToString();
        followersKilledText.text = followersKilled.ToString();
        innocentsKilledText.text = innocentsKilled.ToString();

        SaveLevelNumber(SceneManager.GetActiveScene().buildIndex);
    }

    public void SaveLevelNumber(int currentLevel)
    {
        currentLevelNumber = currentLevel - 1;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        currentLevelNumberText.text = currentLevelNumber.ToString();
    }

    public void SaveEnemiesKilled(int number)
    {
        enemiesKilled += number;
        PlayerPrefs.SetInt("EnemiesKilled", enemiesKilled);
        enemiesKilledText.text = enemiesKilled.ToString();
    }

    public void SaveFollowersRecruited(int number)
    {
        followersRecruited += number;
        PlayerPrefs.SetInt("FollowersRecruited", followersRecruited);
        followersRecruitedText.text = followersRecruited.ToString();
    }

    public void SaveFollowersKilled(int number)
    {
        followersKilled += number;
        PlayerPrefs.SetInt("FollowersKilled", followersKilled);
        followersKilledText.text = followersKilled.ToString();
    }

    public void SaveInnocentsKilled(int number)
    {
        innocentsKilled += number;
        PlayerPrefs.SetInt("InnocentsKilled", innocentsKilled);
        innocentsKilledText.text = innocentsKilled.ToString();
    }

    public void ResetAllData()
    {
        Debug.Log("Reset all data");
        PlayerPrefs.DeleteAll();

        currentLevelNumberText.text = "0";
        enemiesKilledText.text = "0";
        followersRecruitedText.text = "0";
        followersKilledText.text = "0";
        innocentsKilledText.text = "0";
    }
}
