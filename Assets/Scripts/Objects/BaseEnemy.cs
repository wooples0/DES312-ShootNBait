using UnityEngine;
using System.Collections;
using UnityEngine.VFX;
using System.Collections.Generic;
using System.Linq;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField] private Animator a;
    [SerializeField] private Renderer outlineRenderer;

    public List<Tile> tileMoveQueue;
    public Tile currentTile;
    public Tile targetTile;

    public EnemyState enemyState;

    private float walkSpeed;
    [SerializeField] private float speedModifier;
    [SerializeField] private float minimumSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyState = EnemyState.Idle;   
        transform.position = currentTile.GetPosition();
    }

    private void Update()
    {
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
                a.SetTrigger("finished");
                AudioManager.Instance.PlayRandom(new string[] { "pop1", "pop2", "pop3" });
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
        if (tile == currentTile) { /*Debug.Log($"{name} trying to move to {tile.name}, already at {tile.name}")*/; ChangeState(EnemyState.Finished); return; }
        targetTile = tile; ChangeState(EnemyState.Moving);
    }

    public void SetOutline(bool outline)
    {
        
        if (outlineRenderer == null) { Debug.Log("Outline not found"); }
        if (outline)
        {
            outlineRenderer.material.color = Color.white;
        }
        else
        {
            outlineRenderer.material.color = Color.black;
        }
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

    private IEnumerator MoveThroughQueuedTiles()
    {
        var initialQueueCount = tileMoveQueue.Count;
        for (int i = 0; i < initialQueueCount; i++)
        {
            var tile = tileMoveQueue[i];
            if (tile.Walkable)
            {
                //leaving space for ifi want walkable tiles to have special effects
            }
            else
            {
                for(int j = 0; j < initialQueueCount - i; j++)
                {
                    tileMoveQueue.RemoveAt(i);
                }
                break;
            }
        }

        if (tileMoveQueue.Count == 0)
        {
            tileMoveQueue.Clear();
            
            ChangeState(EnemyState.Finished);
            yield break;
        }

        walkSpeed = Mathf.Max(Vector3.Distance(transform.position, tileMoveQueue[tileMoveQueue.Count - 1].GetPosition()) * speedModifier, minimumSpeed);
        a.SetTrigger("moving");

        yield return new WaitForSeconds(0.0125f);
        for (int i = 0; i < tileMoveQueue.Count; i++)
        {
            var currentTargetTile = tileMoveQueue[i];

            while(Vector3.Distance(transform.position, currentTargetTile.GetPosition()) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentTargetTile.GetPosition(), walkSpeed * Time.deltaTime);
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