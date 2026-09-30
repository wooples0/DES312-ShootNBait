using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour
{
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
                break;
            case EnemyState.Moving:
                //Debug.Log("Starting coroutine");
                StartCoroutine(MoveToTile());
                break;
            case EnemyState.Finished:
                SetCurrentTile(targetTile);
                targetTile = null;
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


    private IEnumerator MoveToTile()
    {
        if (currentTile != null) { currentTile.RemoveEnemyFromTile(this); }
        while (new Vector2(transform.position.x, transform.position.y) != targetTile.GetPosition())
        {
            transform.position = Vector3.MoveTowards(transform.position, targetTile.GetPosition(), walkSpeed * Time.deltaTime);
            yield return null;
        }
        //yield return new WaitUntil(() => new Vector2() == targetTile.GetPosition());
        
        ChangeState(EnemyState.Finished);
    }
    public enum EnemyState
    {
        Idle,
        Moving,
        Finished,
    }

    private Vector2 GetPosition()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }
}
