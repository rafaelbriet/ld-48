using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private bool canDoubleJump = false;

    private GameObject player;
    private Satan satan;

    public bool CanDoubleJump => canDoubleJump;

    private void Awake()
    {
        Transform playerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;

        player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);

        player.GetComponent<Character>().CharacterDied += OnPlayerDeath;

        satan = FindObjectOfType<Satan>();

        if (satan != null)
        {
            satan.SatanDied += OnSataDeath;
        }
    }

    private void OnSataDeath()
    {
        SceneManager.LoadScene("GameOver");
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
