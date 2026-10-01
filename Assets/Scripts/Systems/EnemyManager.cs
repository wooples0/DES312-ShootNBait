using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private List<ScriptableEnemy> enemies = new List<ScriptableEnemy>();
    public List<BaseEnemy> spawnedEnemies = new List<BaseEnemy>();

    private void Awake()
    {
        Instance = this;

        enemies = Resources.LoadAll<ScriptableEnemy>("Enemies").ToList();
    }
    private void Start()
    {
        
    }

    public void SpawnEnemy(int i)
    {
        var spawnPos = GridManager.Instance.GetRandomTile().GetPosition();
        var spawnedEnemy = Instantiate(enemies[0].enemyPrefab, spawnPos, enemies[0].enemyPrefab.transform.rotation);
        spawnedEnemy.GetComponent<BaseEnemy>().currentTile = GridManager.Instance.GetTileAtPosition(spawnPos);
        spawnedEnemies.Add(spawnedEnemy.GetComponent<BaseEnemy>());
        spawnedEnemy.GetComponent<BaseEnemy>().currentTile.AddEnemyToTile(spawnedEnemy.GetComponent<BaseEnemy>());
    }

    public IEnumerator SpawnNextWave()
    {
        for(int i = 0; i < 10; i++)
        {
            SpawnEnemy(0);
            yield return new WaitForSeconds(0.005f);
        }
        GameManager.Instance.ChangeState(GameState.PlayerPhase);
        yield return null;
    }

    public IEnumerator SetEnemiesOnTiles(Tile targetTile, List<Tile> highlightedTiles)
    {
        List<BaseEnemy> enemiesToMove = new List<BaseEnemy>();
        foreach(BaseEnemy enemy in spawnedEnemies)
        {
            foreach(Tile tile in highlightedTiles)
            {
                if(enemy.currentTile == tile)
                {
                    enemy.SetTargetTile(targetTile);
                    enemiesToMove.Add(enemy);
                }
            }
        }
        if(enemiesToMove.Count == 0)
        {
            GameManager.Instance.ChangeState(GameState.PlayerPhase);
            yield break;
        }
        var allEnemiesMoved = false;
        while (!allEnemiesMoved)
        {
            foreach (BaseEnemy enemy in enemiesToMove)
            {
                allEnemiesMoved = true;
                if(enemy.targetTile != null) { allEnemiesMoved = false; }
            }
            yield return null;
        }
        GameManager.Instance.ChangeState(GameState.PlayerPhase);
        yield return null;
    }

    public void KillEnemiesOnTile(Tile targetTile)
    {
        List<BaseEnemy> enemiesToKill = new List<BaseEnemy>();
        foreach(BaseEnemy enemy in spawnedEnemies)
        {
            if(enemy.currentTile == targetTile) { enemiesToKill.Add(enemy); }
        }

        for(int i = 0; i < enemiesToKill.Count; i++)
        {
            enemiesToKill[i].OnDeath();
        }
        if(spawnedEnemies.Count > 0) { GameManager.Instance.ChangeState(GameState.MovePhase); }
        GameManager.Instance.ChangeState(GameState.EndPhase);
    }

    public void OnDebug_SpawnEnemy(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) { return; }
        SpawnEnemy(0);

    }

    public void OnDebug_MoveAllEnemies(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) { return; }
        foreach (BaseEnemy enemy in spawnedEnemies)
        {
            var randomTile = GridManager.Instance.GetRandomTile();
            enemy.SetTargetTile(randomTile);
        }
    }

    

}
