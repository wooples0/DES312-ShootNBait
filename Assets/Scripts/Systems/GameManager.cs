using UnityEngine;
using System;

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
                StartCoroutine(GridManager.Instance.GenerateGrid());
                break;
            case GameState.SpawnEnemies:
                StartCoroutine(EnemyManager.Instance.SpawnNextWave());
                Debug.Log("Spawn Enemies");
                break;
            case GameState.PlayerPhase:
                Debug.Log("Player Phase");
                break;
            case GameState.MovePhase:
                Debug.Log("Move Phase");
                break;
            case GameState.ShootPhase:
                if (GunManager.Instance.targetTile != null) { EnemyManager.Instance.KillEnemiesOnTile(GunManager.Instance.targetTile); }
                break;
            case GameState.EndPhase:
                if (EnemyManager.Instance.spawnedEnemies.Count == 0) { ChangeState(GameState.SpawnEnemies); } else { ChangeState(GameState.PlayerPhase); }
                    break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
}

public enum GameState
{
    InitialiseLevel = 0,
    SpawnEnemies = 1,
    PlayerPhase = 2,
    MovePhase = 3,
    ShootPhase = 4,
    EndPhase = 5
}