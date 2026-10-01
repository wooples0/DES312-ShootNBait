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
    private List<Tile> highlightedTiles = new List<Tile>();

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {

    }
    private void Update()
    {
        //UpdateTileHighlights();
    }
    
    public void UpdateTileHighlights()
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
            case GunManager.BaitType.DiagonalR:
                targetTile.SetHighlight(0, true);
                foreach (Tile tile in GetTilesInDiagonalRight(new Vector2((int)targetTile.transform.position.x, (int)targetTile.transform.position.y)))
                {
                    if (tile.TryGetComponent<GroundTile>(out var ground))
                    {
                        GunManager.Instance.targetedTiles.Add(ground);
                        tile.SetHighlight(1, true);
                    }
                }
                break;
        }


    }

    public IEnumerator GenerateGrid()
    {
        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, (width + height) / 2 * -1);
        for (int x = 0; x < width; x++)
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
                yield return new WaitForSeconds(0.0005f);
            }
        }

        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
        yield return null;
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
        

        if(tiles.TryGetValue(pos, out var tile))
        {
            List<Tile> diagonalTiles = new List<Tile>();

            for(int y = (int)pos.y ; y < height-1; y++)
            {
                for(int x = (int)pos.x; x < width-1; x++)
                {
                    if(tiles.TryGetValue(new Vector2(x, y), out var tileToAdd))
                    {
                        Debug.Log($"Adding tile {tileToAdd.name} to the list");
                        diagonalTiles.Add(tileToAdd);
                    }
                    else
                    {
                        return diagonalTiles;
                    }
                }
            }
        }
        return null;
    }

    public List<Tile> GetTilesInDiagonalLeft(Vector2 pos)
    {
        List<Tile> diagonalTiles = new List<Tile>();

        return null;
    }
   

    public void ClearAllTileHighlights()
    {
        highlightedTiles.Clear();
        foreach(var tile in tiles.Values)
        {
            if(tile.TryGetComponent<GroundTile>(out var groundTile)){
                groundTile.SetHighlight(0, false);
                groundTile.SetHighlight(1, false);
            }
            
        }
    }

    public Tile GetRandomTile()
    {
        var randomPos = new Vector2(Random.Range(0, width - 1), Random.Range(0, height - 1));
        return GetTileAtPosition(randomPos);
    }

}
