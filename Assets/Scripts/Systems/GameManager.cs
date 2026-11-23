using UnityEngine;
using System;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEditor;
using Abertay.Analytics;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState GameState;

    [SerializeField] private List<ScriptableLevel> loadedLevels;
    [SerializeField] private int levelIndex = 0;
    [SerializeField] private bool resetLevelsOnLoad;
    [SerializeField] private ScriptableLevel levelToLoad;
    public ScriptableLevel level => loadedLevels[levelIndex];
    //[SerializeField] private static List<ScriptableLevel> levels = new List<ScriptableLevel>();

    private float gameTime;
    private float levelTime;

    private void Awake()
    {
        Instance = this;
        
    }
    void Start()
    {
        
        Debug.Log($"Loading levelIndex {levelIndex}");
        //levels = Resources.LoadAll<ScriptableLevel>("Levels").ToList();
        ChangeState(GameState.UserInputPhase);
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
        if(GameState != GameState.EndPhase && GameState != GameState.SpawnEnemies && GameState != GameState.InitialiseLevel && GameState != GameState.UserInputPhase)
        {
            levelTime += Time.deltaTime;
        }
    }

    public void ChangeState(GameState newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameState.UserInputPhase:
                if (!SettingsManager.Instance.recordPlayerData) { ChangeState(GameState.InitialiseLevel); }
                break;
            case GameState.InitialiseLevel:
                InitialiseLevel();
                StartCoroutine(GridManager.Instance.GenerateGridFromLevel(loadedLevels[levelIndex]));
                break;

            case GameState.SpawnEnemies:
                StartCoroutine(EnemyManager.Instance.SpawnNextWave());
                break;

            case GameState.PlayerPhase:
                UIManager.Instance.EnableControlsUI(true);
                UIManager.Instance.UpdateEnemiesOnTargetUI();
                UIManager.Instance.UpdateBaitUI(GunManager.Instance.equippedBait, GunManager.Instance.lastBaitUsed);
                GridManager.Instance.UpdateTileHighlights();
                break;
            
            case GameState.MovePhase:
                break;

            case GameState.ShootPhase:
                
                if(SettingsManager.Instance.mode == GameMode.Limited) { Tile.lastHoveredTile = Tile.hoveredTile; Tile.hoveredTile = Tile.goalTile; }
                UIManager.Instance.UpdateLevelTitle(string.Empty);
                UIManager.Instance.ResetBaitUI();
                if (Tile.hoveredTile != null) { EnemyManager.Instance.KillEnemiesOnTile(Tile.hoveredTile); }
                break;
            
            case GameState.EndPhase:
                
                if(EnemyManager.Instance.spawnedEnemies.Count == 0)
                {
                    if (SettingsManager.Instance.recordPlayerData) { RecordLevelAnalytics(); }
                    if (levelIndex < loadedLevels.Count - 1) { levelIndex++; }
                    else { levelIndex = 0; }
                    ChangeState(GameState.InitialiseLevel);
                }
                else
                {
                    ChangeState(GameState.PlayerPhase);
                }
                
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    public void OnReloadLevel(InputAction.CallbackContext ctx)
    {
        if (GameState != GameState.PlayerPhase) { return; }
        if (!ctx.started) {  return; }
        InitialiseLevel();
    }

    public void RecordLevelAnalytics()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        data.Add("elapsedTotalTime", gameTime);
        data.Add("elapsedLevelTime", levelTime);
        data.Add("totalMovesPerformed", ScoreManager.Instance.baitUsed);

        Debug.Log($"" +
            $"Recording data for [{loadedLevels[levelIndex]}" +
            $"\n\t\t\telapsedLevelTime: {gameTime}" +
            $"\n\t\t\telapsedLevelTime: {levelTime}" +
            $"\n\t\t\ttotalMovesPerformed: {ScoreManager.Instance.baitUsed}"
            );

        AnalyticsManager.SendCustomEvent(loadedLevels[levelIndex].levelName, data);

    }

    public void InitialiseLevel()
    {
        EnemyManager.Instance.ResetEnemyList();
        GridManager.Instance.ResetGrid();
        GunManager.Instance.lastBaitUsed = Axis.None;
        AudioManager.Instance.StopSound("bgm");
        ScoreManager.Instance.ResetAllValues();
        GunManager.Instance.pullsInLevel = 0;
        UIManager.Instance.UpdateBaitTextUI(GunManager.Instance.pullsInLevel);

        levelTime = 0;
        if(GameState != GameState.InitialiseLevel) { ChangeState(GameState.InitialiseLevel); }
        
    }
    public void ReloadSceneAndLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //private ScriptableLevel GetLevelByName(string levelName)
    //{
    //    foreach(ScriptableLevel level in levels)
    //    {
    //        if(level.name == levelName) return level;
    //    }

    //    Debug.Log($"Could not find level of name: {levelName}");
    //    return null;
    //}

    //private ScriptableLevel GetLevelByID(int levelID)
    //{
    //    foreach (ScriptableLevel level in levels)
    //    {
    //        if (level.id == levelID) return level;
    //    }

    //    Debug.Log($"Could not find level of ID: {levelID}");
    //    return null;
    //}
}

public enum GameState
{
    UserInputPhase = 0,
    InitialiseLevel = 1,
    SpawnEnemies = 2,
    PlayerPhase = 3,
    MovePhase = 4,
    ShootPhase = 5,
    EndPhase = 6,
    SaveDataPhase = 7,
    LoadDataPhase = 8,
}