using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private List<ScriptableEnemy> enemies = new List<ScriptableEnemy>();
    public List<BaseEnemy> spawnedEnemies = new List<BaseEnemy>();

    private int noOfEnemiesToSpawn = 0;

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
        var spawnedEnemy = Instantiate(enemies[Random.Range(0, enemies.Count)].enemyPrefab, spawnPos, enemies[0].enemyPrefab.transform.rotation);
        spawnedEnemy.GetComponent<BaseEnemy>().currentTile = GridManager.Instance.GetTileAtPosition(spawnPos);
        spawnedEnemies.Add(spawnedEnemy.GetComponent<BaseEnemy>());
        spawnedEnemy.GetComponent<BaseEnemy>().currentTile.AddEnemyToTile(spawnedEnemy.GetComponent<BaseEnemy>());
        spawnedEnemy.name = $"Enemy {spawnedEnemies.Count}";
    }

    public IEnumerator SpawnNextWave()
    {
        noOfEnemiesToSpawn = (int)Mathf.Ceil(GridManager.Instance.size * 3f);
        for (int i = 0; i < noOfEnemiesToSpawn; i++)
        {
            Random.InitState(42+i);
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
        if(spawnedEnemies.Count > 0) { StartCoroutine(MoveAllEnemiesAwayFromTargetTile()); }
        GameManager.Instance.ChangeState(GameState.EndPhase);
    }

    public void OnDebug_SpawnEnemy(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) { return; }
        SpawnEnemy(0);

    }

    public void OnDebug_MoveAllEnemies(InputAction.CallbackContext ctx)
    {
        if (!ctx.started || GameManager.Instance.GameState != GameState.PlayerPhase) { return; }
        Debug.Log(ctx.control.path);
        if (ctx.control.path == "/Keyboard/r")
        {
            StartCoroutine(MoveAllEnemiesRandomly());
        }
        else if (ctx.control.path == "/Keyboard/space")
        {
            StartCoroutine(MoveAllEnemiesToTile());
        }
    }

    public IEnumerator MoveAllEnemiesRandomly()
    {
        GameManager.Instance.ChangeState(GameState.MovePhase);
        foreach (BaseEnemy enemy in spawnedEnemies)
        {
            var randomTile = GridManager.Instance.GetRandomTile();
            enemy.SetTargetTile(randomTile);
        }
        var allEnemiesMoved = false;
        while (!allEnemiesMoved)
        {
            foreach (BaseEnemy enemy in spawnedEnemies)
            {
                allEnemiesMoved = true;
                if (enemy.targetTile != null) { allEnemiesMoved = false; }
            }
            yield return null;
        }
        GameManager.Instance.ChangeState(GameState.PlayerPhase);
        yield return null;
    }

    public IEnumerator MoveAllEnemiesToTile()
    {
        GameManager.Instance.ChangeState(GameState.MovePhase);
        foreach(BaseEnemy enemy in spawnedEnemies)
        {
            var targetTile = GunManager.Instance.targetTile;
            if(targetTile != null) { enemy.SetTargetTile(targetTile); }
            else
            {yield break;}
        }

        var allEnemiesMoved = false;
        while (!allEnemiesMoved)
        {
            foreach(BaseEnemy enemy in spawnedEnemies)
            {
                allEnemiesMoved = true;
                if (enemy.targetTile != null) { allEnemiesMoved = false; }
            }
            yield return null;
        }
        GameManager.Instance.ChangeState(GameState.PlayerPhase);
        yield return null;
    }

    public IEnumerator MoveAllEnemiesAwayFromTargetTile()
    {
        GameManager.Instance.ChangeState(GameState.MovePhase);
        var targetTilePos = GunManager.Instance.targetTile.GetPosition();
        foreach(BaseEnemy enemy in spawnedEnemies)
        {
            if(enemy.transform.position.x < targetTilePos.x && enemy.transform.position.y == targetTilePos.y)
            {
                Debug.Log("Moving left");
                var offset = -1;
                var axis = Axis.Horizontal;
                
                var tileAtLeft = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile.GetPosition(), offset);
                
                if (tileAtLeft != null) { enemy.SetTargetTile(tileAtLeft);}
                continue;
            }
            else if(enemy.transform.position.x > targetTilePos.x && enemy.transform.position.y == targetTilePos.y)
            {
                Debug.Log("Moving right");
                var offset = 1;
                var axis = Axis.Horizontal;
                
                var tileAtRight = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile.GetPosition(), offset);
                
                if (tileAtRight != null) { enemy.SetTargetTile(tileAtRight); }
                continue;
            }

            if(enemy.transform.position.x == targetTilePos.x && enemy.transform.position.y > targetTilePos.y)
            {
                Debug.Log("Moving up");
                var offset = 1;
                var axis = Axis.Vertical;

                var tileAtTop = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile.GetPosition(), offset);

                if (tileAtTop != null) { enemy.SetTargetTile(tileAtTop); }
            }
            else if (enemy.transform.position.x == targetTilePos.x && enemy.transform.position.y < targetTilePos.y)
            {
                Debug.Log("Moving down");
                var offset = -1;
                var axis = Axis.Vertical;

                var tileAtBottom = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile.GetPosition(), offset);

                if (tileAtBottom != null) { enemy.SetTargetTile(tileAtBottom); }
            }
            
            if(enemy.transform.position.x > targetTilePos.x && enemy.transform.position.y > targetTilePos.y)
            {
                Debug.Log("Moving diagonally right and up");
                var offset = 1;
                var axis = Axis.DiagonalR;

                var tileAtDiagonalUpRight = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile.GetPosition(), offset);

                if(tileAtDiagonalUpRight != null) { enemy.SetTargetTile(tileAtDiagonalUpRight); }
            }
            else if(enemy.transform.position.x < targetTilePos.x && enemy.transform.position.y < targetTilePos.y)
            {
                Debug.Log("Moving diagonally left and down");
                var offset = -1;
                var axis = Axis.DiagonalR;

                var tileAtDiagonalDownLeft = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile.GetPosition(), offset);

                if (tileAtDiagonalDownLeft != null) { enemy.SetTargetTile(tileAtDiagonalDownLeft); }
            }

            if(enemy.transform.position.x > targetTilePos.x && enemy.transform.position.y < targetTilePos.y)
            {
                Debug.Log("Moving diagonally right and down");
                
                var offset = -1;
                var axis = Axis.DiagonalL;

                var tileAtDiagonalUpLeft = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile.GetPosition(), offset);

                if (tileAtDiagonalUpLeft != null) { enemy.SetTargetTile(tileAtDiagonalUpLeft); }
            }
            else if (enemy.transform.position.x < targetTilePos.x && enemy.transform.position.y > targetTilePos.y)
            {
                Debug.Log("Moving diagonally left and up");
                var offset = 1;
                var axis = Axis.DiagonalL;

                var tileAtDiagonalDownRight = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile.GetPosition(), offset);

                if (tileAtDiagonalDownRight != null) { enemy.SetTargetTile(tileAtDiagonalDownRight); }
            }



        }
        yield return null;
    }

    

}
