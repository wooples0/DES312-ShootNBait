using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [SerializeField] private int width, height;
    [SerializeField] private Tile groundTile;
    [SerializeField] private Transform cam;

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
        
        if(targetTile.GetEnemiesOnTile().Count > 0)
        {
            targetTile.SetHighlight(2, true);
            return;
        }
        else if(GunManager.Instance.equippedBait == GunManager.Instance.lastBaitUsed)
        {
            targetTile.SetHighlight(2, true);
        }
        else
        {
            targetTile.highlight_directionArrow.SetActive(true);
            targetTile.SetHighlight(1, true);
        }

        switch (GunManager.Instance.equippedBait)
        {
            case GunManager.BaitType.Vertical:
                
                foreach (Tile tile in GetTilesInColumn(new Vector2((int)targetTile.transform.position.x, (int)targetTile.transform.position.y)))
                {
                    if (tile.TryGetComponent<GroundTile>(out var ground))
                    {
                        GunManager.Instance.targetedTiles.Add(ground);
                        if (tile == targetTile)
                        {
                            tile.highlight_directionArrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                            tile.SetHighlight(1, true);
                        }
                        else { tile.SetHighlight(1, true); }
                    }
                }
                break;
            case GunManager.BaitType.Horizontal:
                foreach (Tile tile in GetTilesInRow(new Vector2((int)targetTile.transform.position.x, (int)targetTile.transform.position.y)))
                {
                    if (tile.TryGetComponent<GroundTile>(out var ground))
                    {
                        GunManager.Instance.targetedTiles.Add(ground);

                        if (tile == targetTile)
                        {
                            tile.highlight_directionArrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                            tile.SetHighlight(1, true);
                        }
                        else { tile.SetHighlight(1, true); }

                    }
                }
                break;
            case GunManager.BaitType.DiagonalR:
                foreach (Tile tile in GetTilesInDiagonalRight(new Vector2((int)targetTile.transform.position.x, (int)targetTile.transform.position.y)))
                {
                    if (tile.TryGetComponent<GroundTile>(out var ground))
                    {
                        GunManager.Instance.targetedTiles.Add(ground);
                        if (tile == targetTile)
                        {
                            tile.highlight_directionArrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45));
                            tile.SetHighlight(1, true);
                        }
                        else { tile.SetHighlight(1, true); }

                    }
                }
                break;
            case GunManager.BaitType.DiagonalL:
                foreach (Tile tile in GetTilesInDiagonalLeft(new Vector2((int)targetTile.transform.position.x, (int)targetTile.transform.position.y)))
                {
                    if (tile.TryGetComponent<GroundTile>(out var ground))
                    {
                        GunManager.Instance.targetedTiles.Add(ground);
                        if (tile == targetTile)
                        {
                            tile.highlight_directionArrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45));
                            tile.SetHighlight(1, true);
                        }
                        else { tile.SetHighlight(1, true); }
                    }
                }
                break;
        }


    }

    public IEnumerator GenerateGrid()
    {
        //Random.InitState(42);
        if (PlayerPrefs.HasKey("width")) { width = PlayerPrefs.GetInt("width"); }
        else
        {
            PlayerPrefs.SetInt("width", width);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.HasKey("height")) { height = PlayerPrefs.GetInt("height"); }
        else
        {
            PlayerPrefs.SetInt("height", height);
            PlayerPrefs.Save();
        }
        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
        cam.GetComponent<Camera>().orthographicSize = ((width + height) / 4.0f);
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var i = x * width + y;

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

    //public IEnumerator GeneratePremadeGrid(Dictionary<>)
    //{

    //}

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

        if (tiles.TryGetValue(pos, out var tile))
        {
            var currentPos = pos;

            while(tiles.TryGetValue(currentPos, out var tileFound))
            {
                diagonalTiles.Add(tileFound);
                currentPos += new Vector2(1, 1);
            }

            currentPos = pos;
            while (tiles.TryGetValue(currentPos, out var tileFound))
            {
                diagonalTiles.Add(tileFound);
                currentPos += new Vector2(-1, -1);
            }
        }
        return diagonalTiles;
    }

    public List<Tile> GetTilesInDiagonalLeft(Vector2 pos)
    {
        List<Tile> diagonalTiles = new List<Tile>();

        if (tiles.TryGetValue(pos, out var tile))
        {
            var currentPos = pos;

            while (tiles.TryGetValue(currentPos, out var tileFound))
            {
                diagonalTiles.Add(tileFound);
                currentPos += new Vector2(-1, 1);
            }

            currentPos = pos;
            while (tiles.TryGetValue(currentPos, out var tileFound))
            {
                diagonalTiles.Add(tileFound);
                currentPos += new Vector2(1, -1);
            }
        }
        return diagonalTiles;
    }
   

    public void ClearAllTileHighlights()
    {
        highlightedTiles.Clear();
        foreach(var tile in tiles.Values)
        {
            if(tile.TryGetComponent<GroundTile>(out var groundTile)){
                groundTile.SetHighlight(0, false);
                groundTile.SetHighlight(1, false);
                groundTile.SetHighlight(2, false);
                groundTile.highlight_directionArrow.SetActive(false);
            }
            
        }
    }

    public Tile GetRandomTile()
    {
        var randomPos = new Vector2(Random.Range(0, width - 1), Random.Range(0, height - 1));
        return GetTileAtPosition(randomPos);
    }

}
