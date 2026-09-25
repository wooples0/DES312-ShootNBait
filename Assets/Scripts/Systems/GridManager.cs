using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private Tile groundTile, wallTile;
    [SerializeField] private Transform cam;
    [SerializeField] private int[] map;

    private Dictionary<Vector2, Tile> tiles;
    private void Start()
    {
        GenerateGrid();
    }
    private void GenerateGrid()
    {
        tiles = new Dictionary<Vector2, Tile>();
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var i = x * width + y;
                var iTile = map[i] == 0 ? groundTile: wallTile; 

                var spawnedTile = Instantiate(iTile, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                var isOffset = (x + y) % 2 == 1;
                spawnedTile.Init(isOffset);

                tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        cam.transform.position = new Vector3((float)width/2-0.5f, (float)height/2-0.5f, (width+height)/2*-1);

    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if(tiles.TryGetValue(pos, out var tile)) { return tile; } else { return null; }
    }
}
