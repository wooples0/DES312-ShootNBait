using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [SerializeField] private int width, height;
    [SerializeField] private Tile groundTile, wallTile;
    [SerializeField] private Transform cam;
    [SerializeField] private int[] map;

    private Dictionary<Vector2, Tile> tiles = new Dictionary<Vector2, Tile>();

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {

    }
    private void Update()
    {
        UpdateTileHighlights();
    }
    
    private void UpdateTileHighlights()
    {
        ClearAllTileHighlights();
        if (GameManager.Instance.GameState != GameState.PlayerPhase) { return; }
        var targetTile = GunManager.Instance.targetTile;
        GunManager.Instance.targetedTiles.Clear();
        if(targetTile == null)
        { 
            return; 
        }
        if (GunManager.Instance.isGunEquipped)
        {
            targetTile.SetHighlight(0, true);
            GunManager.Instance.targetedTiles.Add(targetTile);
            return;
        }
        
        switch (GunManager.Instance.equippedBait)
        {
            case GunManager.BaitType.Vertical:
                targetTile.SetHighlight(0, true);
                foreach (Tile tile in GetTilesInColumn(new Vector2((int)targetTile.transform.position.x, (int)targetTile.transform.position.y)))
                {
                    if (tile.TryGetComponent<GroundTile>(out var ground))
                    {
                        GunManager.Instance.targetedTiles.Add(ground);
                        tile.SetHighlight(1, true);
                    }
                }
                break;
            case GunManager.BaitType.Horizontal:
                targetTile.SetHighlight(0, true);
                foreach (Tile tile in GetTilesInRow(new Vector2((int)targetTile.transform.position.x, (int)targetTile.transform.position.y)))
                {
                    if (tile.TryGetComponent<GroundTile>(out var ground))
                    {
                        GunManager.Instance.targetedTiles.Add(ground);
                        tile.SetHighlight(1, true);
                    }
                }
                break;
            default:
                Debug.Log("Not written yet");
                break;
        }


    }

    public void GenerateGrid()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var i = x * width + y;
                //var tileToSpawn = groundTile;
                //switch (i)
                //{
                    
                //    case 0:
                //        tileToSpawn = groundTile;
                //        break;
                //}

                var spawnedTile = Instantiate(groundTile, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                spawnedTile.Init(x,y);

                tiles[new Vector2((int)x, (int)y)] = spawnedTile;
            }
        }

        cam.transform.position = new Vector3((float)width/2-0.5f, (float)height/2-0.5f, (width+height)/2*-1);
        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if(tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;
        
    }

    public Tile[] GetTilesInColumn(Vector2 pos)
    {
        Tile[] columnTiles = new Tile[height];

        if(tiles.TryGetValue(pos, out var tile))//thers prob a more efficient way to do this but this works For now
        {
            for(int i = 0; i < height; i++)
            {
                var indexTile = GetTileAtPosition(new Vector2((int)pos.x, i));
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
                var indexTile = GetTileAtPosition(new Vector2(i, (int)pos.y));
                rowTiles[i] = indexTile;
            }
            return rowTiles;
        }
        return null;
    }

    public List<Tile> GetTilesInDiagonalRight(Vector2 pos)
    {
        List<Tile> diagonalTiles = new List<Tile>();
        return null;
    }

    public List<Tile> GetTilesInDiagonalLeft(Vector2 pos)
    {
        List<Tile> diagonalTiles = new List<Tile>();

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

    public Tile GetRandomTile()
    {
        var randomPos = new Vector2(Random.Range(0, width - 1), Random.Range(0, height - 1));
        return GetTileAtPosition(randomPos);
    }

}
