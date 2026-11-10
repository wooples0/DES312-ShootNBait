using UnityEngine;
using System.Collections;
using UnityEngine.VFX;
using System.Collections.Generic;
using System.Linq;

public class BaseEnemy : MonoBehaviour
{
    public List<Tile> tileMoveQueue;
    public Tile currentTile;
    public Tile targetTile;

    public EnemyState enemyState;

    [SerializeField] private float walkSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyState = EnemyState.Idle;   
        transform.position = currentTile.GetPosition();
    }

    public void ChangeState(EnemyState state)
    {
        enemyState = state;
        switch (enemyState)
        {
            case EnemyState.Idle:
                if (targetTile != null) { targetTile = null; }
                break;
            case EnemyState.Moving:
                //Debug.Log("Starting coroutine");
                StartCoroutine(MoveThroughQueuedTiles());
                break;
            case EnemyState.Finished:
                ChangeState(EnemyState.Idle);
                break;
        }
    }

    public Tile GetTile()
    {
        if(currentTile == null)
        { 
            //Debug.Log($"Current tile ({currentTile.name}) is null"); 
            return null; 
        }
        return currentTile;
    }

    public void SetCurrentTile(Tile tile)
    {
        //Debug.Log($"{this.name} is setting current tile to {tile.name}");
        if (currentTile != null) { currentTile.RemoveEnemyFromTile(this); }
        currentTile = tile;
        currentTile.AddEnemyToTile(this);
        transform.position = currentTile.GetPosition();
    }
    public void SetTargetTile(Tile tile)
    {
        if(enemyState != EnemyState.Idle) { return; }
        
        targetTile = tile;
        ChangeState(EnemyState.Moving);
    }

    public void OnDeath()
    {
        currentTile.RemoveEnemyFromTile(this);
        EnemyManager.Instance.spawnedEnemies.Remove(this);
        enemyState = EnemyState.Dead;
        //Debug.Log($"Killed enemy {this.name}");
        Destroy(this.gameObject);
        //StartCoroutine(OnDeathCoroutine());
    }

    //public IEnumerator OnDeathCoroutine()
    //{
    //    Debug.Log($"Killed enemy {this.name}");
    //    Destroy(this.gameObject);
    //    yield return null;
    //}

    private IEnumerator MoveThroughQueuedTiles()
    {
        //Debug.Log($"===Tiles in queue before check: {tileMoveQueue.Count}");
        if (currentTile != null) { currentTile.RemoveEnemyFromTile(this); }

        if(tileMoveQueue.Count == 0 && targetTile != null && targetTile.Walkable)
        {
            while (Vector3.Distance(transform.position, targetTile.GetPosition()) > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, targetTile.GetPosition(), walkSpeed * Time.deltaTime);
                yield return null;
            }
            SetCurrentTile(targetTile);
            ChangeState(EnemyState.Finished);
            yield break;
        }
        var initialQueueCount = tileMoveQueue.Count;
        for (int i = 0; i < initialQueueCount; i++)
        {
            var tile = tileMoveQueue[i];
            if (tile.Walkable)
            {
                
            }
            else
            {
                //Debug.Log($"Found wall tile at index [{i}], removing index [{i}] and tiles after.");
                for(int j = 0; j < initialQueueCount - i; j++)
                {
                    Debug.Log($"~Removing {tileMoveQueue[i].name} from queue");
                    tileMoveQueue.RemoveAt(i);
                }
                break;
            }
        }

        //Debug.Log($"{this.name} is moving [{tileMoveQueue.Count}] tiles.\n");

        if (tileMoveQueue.Count == 0)
        {
            tileMoveQueue.Clear();
            ChangeState(EnemyState.Finished);
            yield break;
        }

        
        for(int i = 0; i < tileMoveQueue.Count; i++)
        {
            var currentTargetTile = tileMoveQueue[i];

            while(Vector3.Distance(transform.position, currentTargetTile.GetPosition()) > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, currentTargetTile.GetPosition(), walkSpeed * Time.deltaTime);
                yield return null;
            }
            SetCurrentTile(currentTargetTile);
            yield return new WaitForSeconds(0.0025f);
            
        }

        tileMoveQueue.Clear();
        ChangeState(EnemyState.Finished);

    }
    public IEnumerator MoveToTile()
    {
        yield return null;
    }
    public void AddTileListToMoveQueue(List<Tile> tileList)
    {
        foreach(var tile in tileList)
        {
            tileMoveQueue.Add(tile);
        }
    }

    public void AddTileToMoveQueue(Tile tile)
    {
        tileMoveQueue.Add(tile);
    }

    public bool IsFinishedMoving()
    {
        return tileMoveQueue.Count == 0 && targetTile == null;
    }
    

    private Vector2 GetPosition()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }
}
public enum EnemyState
{
    Idle,
    Moving,
    TileQueueCheck,
    Finished,
    Dead
}