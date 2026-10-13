using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class LevelEditor_GridGenerator : MonoBehaviour
{
    public static LevelEditor_GridGenerator Instance;

    public int minGridSize, maxGridSize;
    public int width, height;
    private Dictionary<Vector2, LevelEditor_Tile> placedTiles;
    private Dictionary<Vector2, EnemyType> placedEnemies;

    private Camera cam;
    [SerializeField] private LevelEditor_Tile tile;
    private LevelEditor_Enemy enemy;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cam = Camera.main;
        GenerateGrid();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateGrid()
    {
        if(placedTiles != null && placedTiles.Count > 0) { DestroyGridObjects(); }
        
        placedTiles = new Dictionary<Vector2, LevelEditor_Tile>();

        Camera.main.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
        Camera.main.GetComponent<Camera>().orthographicSize = ((width + height) / 3.0f);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var i = x * width + y;
                var spawnedTile = Instantiate(tile, new Vector2(x, y), Quaternion.identity);
                spawnedTile.Init(x, y);
                placedTiles.Add(new Vector2(x, y), spawnedTile);
                
            }
        }
    }

    public void LoadGrid(ScriptableLevel level)
    {
        if (placedTiles != null && placedTiles.Count > 0) { DestroyGridObjects(); }

        placedTiles = new Dictionary<Vector2, LevelEditor_Tile>();

        cam.transform.position = new Vector3((float)level.width / 2 - 0.5f, (float)level.height / 2 - 0.5f, -10);
        cam.GetComponent<Camera>().orthographicSize = ((level.width + level.height) / 3.0f);

        for (int x = 0; x < level.width; x++)
        {
            for (int y = 0; y < level.height; y++)
            {
                var spawnedTile = Instantiate(tile, new Vector2(x, y), Quaternion.identity);
                spawnedTile.Init(x, y);
                if(level.enemyLayout.TryGetValue(new Vector2(x, y), out var enemy))
                {
                    spawnedTile.ToggleEnemy();
                }
                placedTiles.Add(new Vector2(x, y), spawnedTile);

            }
        }
    }

    public void DestroyGridObjects()
    {
        foreach(LevelEditor_Tile tile in placedTiles.Values)
        {
            Destroy(tile.gameObject);
        }
        placedTiles.Clear();
    }

    public void DefaultAllTiles()
    {
        foreach(LevelEditor_Tile tile in placedTiles.Values)
        {
            tile.type = TileType.Ground;
            tile.enemyOnTile = false;
            tile.ClearAllText();
        }
    }

    public Dictionary<Vector2, LevelEditor_Tile> GetTiles()
    {
        return placedTiles;
    }
    public void AddEnemyToPlacedEnemies(Vector2 key)
    {

    }

    public void AddTileToPlacedTiles(Vector2 key)
    {

    }

    public void RemoveEnemyFromPlacedEnemies(Vector2 key)
    {

    }

    public void RemoveTileFromPlacedTiles(Vector2 key)
    {

    }

    public void ClearGrid()
    {

    }
    public void OnLevelEditor_ToggleEnemyOnTile(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) { return; }
        if (LevelEditor_Tile.hoveredTile != null) { LevelEditor_Tile.hoveredTile.ToggleEnemy(); }
    }
    public void OnLevelEditor_ToggleWallOnTile(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) { return; }
        if (LevelEditor_Tile.hoveredTile != null) { LevelEditor_Tile.hoveredTile.ToggleWall(); }
    }
}

public enum TileType
{
    None = 0,
    Ground = 1,
    Wall = 2
}

public enum EnemyType
{
    Default = 0,
}
