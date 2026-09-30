using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    }

    public void SetEnemiesOnTiles(Tile[] tiles)
    {
        
    }

    public void OnDebug_SpawnEnemy(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            SpawnEnemy(0);
            foreach(BaseEnemy enemy in spawnedEnemies)
            {
                var randomTile = GridManager.Instance.GetRandomTile();
                enemy.SetTargetTile(randomTile);
            }
        }

    }

}
