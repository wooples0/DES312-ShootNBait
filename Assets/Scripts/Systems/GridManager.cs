using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [SerializeField] private int width, height;
    [SerializeField] private Tile groundTile, wallTile;
    [SerializeField] private Transform cam;
    [SerializeField] private int[] map;

    private Dictionary<Vector2, Tile> tiles;

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateGrid()
    {
        tiles = new Dictionary<Vector2, Tile>();
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var i = x * width + y;
                var tileToSpawn = groundTile;
                switch (i)
                {
                    
                    case 0:
                        tileToSpawn = groundTile;
                        break;
                    case 1:
                        tileToSpawn = groundTile;
                        break;
                    case 2:
                        tileToSpawn = wallTile;
                        break;
                }

                var spawnedTile = Instantiate(tileToSpawn, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                spawnedTile.Init(x,y);

                tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        cam.transform.position = new Vector3((float)width/2-0.5f, (float)height/2-0.5f, (width+height)/2*-1);

        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if(tiles.TryGetValue(pos, out var tile)) { return tile; } else { return null; }
    }
}
