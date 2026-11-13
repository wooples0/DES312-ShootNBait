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

    public static List<Tile> enemySpawnQueue = new List<Tile>();

    private int noOfEnemiesToSpawn = 0;
    private int totalEnemiesInWave = 0;

    

    private void Awake()
    {
        Instance = this;

        enemies = Resources.LoadAll<ScriptableEnemy>("Enemies").ToList();
        enemySpawnQueue.Clear();
    }
    private void Start()
    {
        
    }

    public void SpawnEnemy(Tile spawnTile)
    {
        var spawnPos = spawnTile.GetPosition();
        if (spawnPos == null) { Debug.Log($"{spawnTile.name} position is null"); }
        var prefabToSpawn = enemies[0].enemyPrefab;
        if (prefabToSpawn == null) { Debug.Log($"enemy prefab null"); }

        var spawnedEnemy = Instantiate(prefabToSpawn, spawnPos, prefabToSpawn.transform.rotation);
        spawnedEnemy.GetComponent<BaseEnemy>().currentTile = GridManager.Instance.GetTileAtPosition(spawnTile.GetPosition());
        spawnedEnemies.Add(spawnedEnemy.GetComponent<BaseEnemy>());
        spawnedEnemy.GetComponent<BaseEnemy>().currentTile.AddEnemyToTile(spawnedEnemy.GetComponent<BaseEnemy>());
        spawnedEnemy.name = $"Enemy {spawnedEnemies.Count}";
    }

    public IEnumerator SpawnNextWave()
    {
        if(enemySpawnQueue.Count == 0)
        {
            noOfEnemiesToSpawn = (int)Mathf.Ceil(GridManager.Instance.GetMaxGridSize() * 3f);
            for (int i = 0; i < noOfEnemiesToSpawn; i++)
            {
                Random.InitState(42 + i);
                SpawnEnemy(GridManager.Instance.GetRandomTile());
                yield return new WaitForSeconds(0.005f);
            }
            totalEnemiesInWave = spawnedEnemies.Count;
            GameManager.Instance.ChangeState(GameState.PlayerPhase);
            yield return null;
        }
        else
        {
            for(int i = 0; i < enemySpawnQueue.Count; i++)
            {
                SpawnEnemy(enemySpawnQueue[i]);
                yield return new WaitForSeconds(0.005f);
            }
            totalEnemiesInWave = spawnedEnemies.Count;
            GameManager.Instance.ChangeState(GameState.PlayerPhase);
            yield return null;
        }
        
    }

    public static void AddEnemyToSpawnQueue(Tile spawnTile)
    {
        enemySpawnQueue.Add(spawnTile);
    }

    public IEnumerator SetEnemiesOnTiles(Tile targetTile, List<Tile> highlightedTiles)
    {
        List<BaseEnemy> enemiesToMove = new List<BaseEnemy>();
        
        //Debug.Log($"No. of highlighted tiles: {highlightedTiles.Count}");
        foreach(BaseEnemy enemy in spawnedEnemies)
        {

            foreach(Tile tile in highlightedTiles)

            {
                if(enemy.currentTile == tile)
                {
                    //Debug.Log($"{enemy.name} on tile {tile.name}");
                    enemiesToMove.Add(enemy);
                }
            }
        }
        //Debug.Log($"Enemies to move = {enemiesToMove.Count}");

        if (enemiesToMove.Count == 0)
        {
            GameManager.Instance.ChangeState(GameState.PlayerPhase);
            yield break;
        }

        for (int i = 0; i < enemiesToMove.Count; i++)
        {
            var tileList = GridManager.Instance.GetTilesBetween(GunManager.Instance.lastBaitUsed, enemiesToMove[i].GetTile(), targetTile);
            //Debug.Log($"{enemiesToMove[i].name} has {tileList.Count} tiles to move");
            enemiesToMove[i].AddTileListToMoveQueue(tileList);
            enemiesToMove[i].SetTargetTile(targetTile);
        }

        var allEnemiesMoved = false;

        while (!allEnemiesMoved)
        {
            foreach (BaseEnemy enemy in enemiesToMove)
            {
                allEnemiesMoved = true;
                if(!enemy.IsFinishedMoving()) { allEnemiesMoved = false; }
                yield return null;
            }
            yield return null;
        }
        //Debug.Log("All enemies finished moving");
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
        //if(spawnedEnemies.Count > 0) { StartCoroutine(MoveAllEnemiesAwayFromTargetTile()); }
        GameManager.Instance.ChangeState(GameState.EndPhase);
    }

    public void OnDebug_SpawnEnemy(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) { return; }
        SpawnEnemy(GridManager.Instance.GetRandomTile());

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
            var targetTile = Tile.hoveredTile;
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
        var targetTilePos = Tile.hoveredTile.GetPosition();
        foreach(BaseEnemy enemy in spawnedEnemies)
        {
            if(enemy.transform.position.x < targetTilePos.x && enemy.transform.position.y == targetTilePos.y)
            {
                Debug.Log("Moving left");
                var offset = -1;
                var axis = Axis.Horizontal;
                
                var tileAtLeft = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile, offset);
                
                if (tileAtLeft != null) { enemy.SetTargetTile(tileAtLeft);}
                continue;
            }
            else if(enemy.transform.position.x > targetTilePos.x && enemy.transform.position.y == targetTilePos.y)
            {
                Debug.Log("Moving right");
                var offset = 1;
                var axis = Axis.Horizontal;
                
                var tileAtRight = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile, offset);
                
                if (tileAtRight != null) { enemy.SetTargetTile(tileAtRight); }
                continue;
            }

            if(enemy.transform.position.x == targetTilePos.x && enemy.transform.position.y > targetTilePos.y)
            {
                Debug.Log("Moving up");
                var offset = 1;
                var axis = Axis.Vertical;

                var tileAtTop = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile, offset);

                if (tileAtTop != null) { enemy.SetTargetTile(tileAtTop); }
            }
            else if (enemy.transform.position.x == targetTilePos.x && enemy.transform.position.y < targetTilePos.y)
            {
                Debug.Log("Moving down");
                var offset = -1;
                var axis = Axis.Vertical;

                var tileAtBottom = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile, offset);

                if (tileAtBottom != null) { enemy.SetTargetTile(tileAtBottom); }
            }
            
            if(enemy.transform.position.x > targetTilePos.x && enemy.transform.position.y > targetTilePos.y)
            {
                Debug.Log("Moving diagonally right and up");
                var offset = 1;
                var axis = Axis.DiagonalR;

                var tileAtDiagonalUpRight = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile, offset);

                if(tileAtDiagonalUpRight != null) { enemy.SetTargetTile(tileAtDiagonalUpRight); }
            }
            else if(enemy.transform.position.x < targetTilePos.x && enemy.transform.position.y < targetTilePos.y)
            {
                Debug.Log("Moving diagonally left and down");
                var offset = -1;
                var axis = Axis.DiagonalR;

                var tileAtDiagonalDownLeft = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile, offset);

                if (tileAtDiagonalDownLeft != null) { enemy.SetTargetTile(tileAtDiagonalDownLeft); }
            }

            if(enemy.transform.position.x > targetTilePos.x && enemy.transform.position.y < targetTilePos.y)
            {
                Debug.Log("Moving diagonally right and down");
                
                var offset = -1;
                var axis = Axis.DiagonalL;

                var tileAtDiagonalUpLeft = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile, offset);

                if (tileAtDiagonalUpLeft != null) { enemy.SetTargetTile(tileAtDiagonalUpLeft); }
            }
            else if (enemy.transform.position.x < targetTilePos.x && enemy.transform.position.y > targetTilePos.y)
            {
                Debug.Log("Moving diagonally left and up");
                var offset = 1;
                var axis = Axis.DiagonalL;

                var tileAtDiagonalDownRight = GridManager.Instance.GetAdjacentTile(axis, enemy.currentTile, offset);

                if (tileAtDiagonalDownRight != null) { enemy.SetTargetTile(tileAtDiagonalDownRight); }
            }



        }
        yield return null;
    }

    public static int GetTotalEnemiesAlive()
    {
        return EnemyManager.Instance.spawnedEnemies.Count;
    }

    public static int GetTotalEnemies()
    {
        return EnemyManager.Instance.totalEnemiesInWave;
    }

    

}
