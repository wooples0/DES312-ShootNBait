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
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.SpawnEnemies:
                Debug.Log("Spawn Enemies");
                ChangeState(GameState.PlayerPhase);
                break;
            case GameState.PlayerPhase:
                Debug.Log("Player Phase");
                break;
            case GameState.MovePhase:
                Debug.Log("Move Phase");
                break;
            case GameState.EndPhase:
                Debug.Log("End Phase");
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
    EndPhase = 4,
}