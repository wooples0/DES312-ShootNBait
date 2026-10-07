using UnityEngine;
using System;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState GameState;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        ChangeState(GameState.InitialiseLevel);
    }

    void Update()
    {
        
    }

    public void ChangeState(GameState newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameState.InitialiseLevel:
                StartCoroutine(GridManager.Instance.GenerateGrid
                    (
                    GridManager.Instance.size, 
                    GridManager.Instance.size)
                    );
                break;

            case GameState.SpawnEnemies:
                ScoreManager.Instance.ResetAllValues();
                GunManager.Instance.lastBaitUsed = Axis.None;
                StartCoroutine(EnemyManager.Instance.SpawnNextWave());
                break;
            case GameState.PlayerPhase:
                break;
            
            case GameState.MovePhase:
                break;

            case GameState.ShootPhase:
                if (GunManager.Instance.targetTile != null) { EnemyManager.Instance.KillEnemiesOnTile(GunManager.Instance.targetTile); }
                break;
            
            case GameState.EndPhase:
                ChangeState(EnemyManager.Instance.spawnedEnemies.Count == 0 ? GameState.SpawnEnemies : GameState.PlayerPhase);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

public enum GameState
{
    InitialiseLevel = 0,
    SpawnEnemies = 1,
    PlayerPhase = 2,
    MovePhase = 3,
    ShootPhase = 4,
    EndPhase = 5,
    SaveDataPhase = 6,
    LoadDataPhase = 7,
}