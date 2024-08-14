using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Idle,
    Play,
    Over
}

public class GameManager : MonoBehaviour
{
    public static event Action OnCubeSpawned = delegate { };
    // public static event Action OnGameOver = delegate { };

    [SerializeField] private GameObject gameOverPanel;

    private GameState state = GameState.Idle;
    private CubeSpawner[] _spawners;
    private int _spawnerIndex;
    private CubeSpawner _currentSpawner;
    
    private void Awake()
    {
        _spawners = FindObjectsOfType<CubeSpawner>();
    }
    
    private void OnEnable()
    {
        GameEvents.GameOver += MovingCube_OnGameOver;
        GameEvents.GameOver += GameEvents_GameOver;
    }
    
    private void OnDisable()
    {
        GameEvents.GameOver -= MovingCube_OnGameOver;
        GameEvents.GameOver -= GameEvents_GameOver;
        
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (state == GameState.Play || state == GameState.Idle)
            {
                if (state == GameState.Idle)
                {
                    GameEvents.GameStart();
                    state = GameState.Play;
                }
                if (MovingCube.CurrentCube)
                {
                    MovingCube.CurrentCube.Stop();
                }

                if (MovingCube.LastCube && MovingCube.CurrentCube)
                {
                    _spawnerIndex = _spawnerIndex == 0 ? 1 : 0;
                    _currentSpawner = _spawners[_spawnerIndex];
                    _currentSpawner.SpawnCube();
                    OnCubeSpawned();    
                }
            }
            else if (state == GameState.Over)
            {
                GameOver();
            }
        }
    }

    private void GameEvents_GameOver()
    {
        state = GameState.Over;
    }
    
    private void MovingCube_OnGameOver()
    {
        state = GameState.Over;
    }
    
    public void GameOver()
    {
        StartCoroutine(GameOverCo());
    }

    private IEnumerator GameOverCo()
    {
        yield return new WaitForSeconds(0.05f);
        SceneManager.LoadScene(0);
    }
}
