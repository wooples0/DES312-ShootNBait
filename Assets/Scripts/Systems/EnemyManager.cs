using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private List<ScriptableEnemy> enemies;

    private void Awake()
    {
        Instance = this;

        enemies = Resources.LoadAll<ScriptableEnemy>("Enemies").ToList();
    }

    public void SpawnEnemy(int i)
    {
        Debug.Log($"Spawning enemy: ");
    }

}
