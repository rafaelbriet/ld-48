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
    [SerializeField]
    private Canvas pauseCanvas;

    private GameObject player;
    private Satan satan;
    private bool isGamePaused;

    public bool IsGamePaused { get => isGamePaused; private set => isGamePaused = value; }

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

        pauseCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                Time.timeScale = 1;
                isGamePaused = false;
                pauseCanvas.gameObject.SetActive(false);

                if (Input.GetKeyDown(KeyCode.M))
                {
                    SceneManager.LoadScene("MainMenu");
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    Application.Quit();
                }
            }
            else
            {
                Time.timeScale = 0;
                isGamePaused = true;
                pauseCanvas.gameObject.SetActive(true);
            }
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
