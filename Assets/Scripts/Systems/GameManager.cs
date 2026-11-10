using UnityEngine;
using System;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState GameState;

    [SerializeField] private List<ScriptableLevel> loadedLevels;
    [SerializeField] private static int levelIndex;
    [SerializeField] private bool resetLevelsOnLoad;
    [SerializeField] private ScriptableLevel levelToLoad;
    public ScriptableLevel level => levelToLoad;
    [SerializeField] private static List<ScriptableLevel> levels = new List<ScriptableLevel>();


    private void Awake()
    {
        Instance = this;
        
    }
    void Start()
    {
        Debug.Log($"Loading levelIndex {levelIndex}");
        levels = Resources.LoadAll<ScriptableLevel>("Levels").ToList();
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
                GridManager.Instance.InitialiseValues();
                StartCoroutine(GridManager.Instance.GenerateGridFromLevel(loadedLevels[levelIndex]));
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
                if(SettingsManager.Instance.mode == GameMode.Limited) { Tile.hoveredTile = Tile.goalTile; }
                if (Tile.hoveredTile != null) { EnemyManager.Instance.KillEnemiesOnTile(Tile.hoveredTile); }
                break;
            
            case GameState.EndPhase:
                if(levelIndex < loadedLevels.Count - 1) { levelIndex++; }
                else { levelIndex = 0; }
                SceneManager.LoadScene(0);
                ChangeState(EnemyManager.Instance.spawnedEnemies.Count == 0 ? GameState.InitialiseLevel : GameState.PlayerPhase);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private ScriptableLevel GetLevelByName(string levelName)
    {
        foreach(ScriptableLevel level in levels)
        {
            if(level.name == levelName) return level;
        }

        Debug.Log($"Could not find level of name: {levelName}");
        return null;
    }

    private ScriptableLevel GetLevelByID(int levelID)
    {
        foreach (ScriptableLevel level in levels)
        {
            if (level.id == levelID) return level;
        }

        Debug.Log($"Could not find level of ID: {levelID}");
        return null;
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