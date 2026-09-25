using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    [SerializeField] private Enemy[] enemies;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnEnemy(int i)
    {
        Debug.Log($"Spawning enemy: ");
    }
}
