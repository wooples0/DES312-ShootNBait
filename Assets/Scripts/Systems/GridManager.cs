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
    private void Update()
    {
        UpdateTileHighlights();
    }
    
    private void UpdateTileHighlights()
    {
        ClearAllTileHighlights();
        var targetTile = GunManager.Instance.targetTile;
        if(targetTile == null)
        { 
            return; 
        }
        switch (GunManager.Instance.equippedGun)
        {
            
            case GunManager.GunTypes.Gun:
                targetTile.SetHighlight(0, true);
                break;
            case GunManager.GunTypes.Bait1:
                targetTile.SetHighlight(0, true);
                foreach (Tile tile in GetTilesInColumn(targetTile.GetXY()))
                {
                    if (tile.TryGetComponent<GroundTile>(out var ground))
                    {
                        tile.SetHighlight(1, true);
                    }
                }
                break;
            case GunManager.GunTypes.Bait2:
                targetTile.SetHighlight(0, true);
                foreach (Tile tile in GetTilesInRow(targetTile.GetXY()))
                {
                    if (tile.TryGetComponent<GroundTile>(out var ground))
                    {
                        tile.SetHighlight(1, true);
                    }
                }
                break;
        }


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

    public Tile[] GetTilesInColumn(Vector2 pos)
    {
        Tile[] columnTiles = new Tile[height];

        if(tiles.TryGetValue(pos, out var tile))//thers prob a more efficient way to do this but this works For now
        {
            for(int i = 0; i < height; i++)
            {
                var indexTile = GetTileAtPosition(new Vector2(pos.x, i));
                columnTiles[i] = indexTile;
            }
            return columnTiles;
        }
        return null;
    }

    public Tile[] GetTilesInRow(Vector2 pos)
    {
        Tile[] rowTiles = new Tile[height];

        if (tiles.TryGetValue(pos, out var tile))
        {
            for (int i = 0; i < height; i++)
            {
                var indexTile = GetTileAtPosition(new Vector2(i, pos.y));
                rowTiles[i] = indexTile;
            }
            return rowTiles;
        }
        return null;
    }
    
    public Tile[] GetTilesBetween(Vector2 pos1, Vector2 pos2)
    {
        return null;
    }

    public void ClearAllTileHighlights()
    {
        foreach(var tile in tiles.Values)
        {
            if(tile.TryGetComponent<GroundTile>(out var tile2)){
                tile2.SetHighlight(0, false);
                tile2.SetHighlight(1, false);
            }
            
        }
    }
}
