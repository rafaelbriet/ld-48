using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    private GameObject player;

    private void Awake()
    {
        Transform playerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;

        player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);

        player.GetComponent<Character>().CharacterDied += OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        RestartLevel();
    }

    public void LoadNextLevel(string nextLevelName)
    {
        SceneManager.LoadScene(nextLevelName);
    }

    private void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.name);
    }
}
