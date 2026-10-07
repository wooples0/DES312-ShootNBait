using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [SerializeField] public int size;
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
        ClearAllTileHighlights();
        UpdateTileHighlights();
    }
    
    private void UpdateTileHighlights()
    {
        if (GameManager.Instance.GameState != GameState.PlayerPhase) { return; }
        
        var targetTile = GunManager.Instance.targetTile;
        GunManager.Instance.targetedTiles.Clear();
        
        if(targetTile == null) { return; }

        if (GunManager.Instance.isGunEquipped)
        {
            targetTile.SetHighlight(0, true);
            return;
        }

        var enemiesOnTile = targetTile.GetEnemiesOnTile().Count > 0;

        if(enemiesOnTile)
        {
            targetTile.SetHighlight(HighlightType.Nope, true);
            return;
        }

        Vector2 targetTilePos = new Vector2((int)targetTile.transform.position.x, (int)targetTile.transform.position.y);

        foreach (Tile tile in GetTilesInAxis(GunManager.Instance.equippedBait, targetTilePos))
        {
            if(tile.TryGetComponent<GroundTile>(out var groundTile))
            {
                GunManager.Instance.targetedTiles.Add(tile);
                tile.SetHighlight(HighlightType.Selected, true);
            }
        }
    }

    public IEnumerator GenerateGrid(int width, int height)
    {
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
                yield return new WaitForSeconds(0.00025f);
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
    public Tile GetAdjacentTile(Axis axis, Vector2 startPos, int offset)
    {
        if (offset != -1 && offset != 1)
        { Debug.Log($"Offset {offset} not valid. (Must be -1 or 1)"); 
            return null; 
        }

        Tile adjacentTile = null;

        switch (axis)
        {
            case Axis.Horizontal:
                adjacentTile = GetTileAtPosition(new Vector2(startPos.x + offset, startPos.y));
                break;
            case Axis.Vertical:
                adjacentTile = GetTileAtPosition(new Vector2(startPos.x, startPos.y + offset));
                break;
            case Axis.DiagonalR:
                adjacentTile = GetTileAtPosition(new Vector2(startPos.x + offset, startPos.y + offset));
                break;
            case Axis.DiagonalL:
                adjacentTile = GetTileAtPosition(new Vector2(startPos.x - offset, startPos.y + offset));
                break;
        }

        if (adjacentTile == null) { Debug.Log("Tile not found or out of bounds."); }
        return adjacentTile != null ? adjacentTile : null;
    }
    public List<Tile> GetTilesInAxis(Axis axis, Vector2 startPos)
    {
        List<Tile> tilesToReturn = new List<Tile>();
        if(!tiles.TryGetValue(startPos, out var tileAtStartPos))
        {
            //Debug.Log($"Specified target tile {startPos} not found in grid.");
            return null;
        }
        
        if(axis == Axis.Vertical)
        {
            for(int i = 0; i < size; i++)
            {
                var indexTile = GetTileAtPosition(new Vector2((int)startPos.x, i));
                tilesToReturn.Add(indexTile);
            }
            //Debug.Log($"{tilesToReturn.Count} tiles in horizontal");
            return tilesToReturn;
        }
        else if(axis == Axis.Horizontal)
        {
            for (int i = 0; i < size; i++)
            {
                var indexTile = GetTileAtPosition(new Vector2(i, (int)startPos.y));
                tilesToReturn.Add(indexTile);
            }
            //Debug.Log($"{tilesToReturn.Count} tiles in vertical");
            return tilesToReturn;
        }
        else if(axis == Axis.DiagonalL)
        {
            var currentPos = startPos;

            while (tiles.TryGetValue(currentPos, out var tileFound))
            {
                tilesToReturn.Add(tileFound);
                currentPos += new Vector2(-1, 1);
            }

            currentPos = startPos;
            while (tiles.TryGetValue(currentPos, out var tileFound))
            {
                tilesToReturn.Add(tileFound);
                currentPos += new Vector2(1, -1);
            }
            return tilesToReturn;
        }
        else if(axis == Axis.DiagonalR)
        {
            var currentPos = startPos;

            while (tiles.TryGetValue(currentPos, out var tileFound))
            {
                tilesToReturn.Add(tileFound);
                currentPos += new Vector2(1, 1);
            }

            currentPos = startPos;
            while (tiles.TryGetValue(currentPos, out var tileFound))
            {
                tilesToReturn.Add(tileFound);
                currentPos += new Vector2(-1, -1);
            }
            return tilesToReturn;
        }
        else
        {
            return null;
        }
        
    }

    public void ClearAllTileHighlights()
    {
        highlightedTiles.Clear();
        foreach(var tile in tiles.Values)
        {
            if(tile.TryGetComponent<GroundTile>(out var groundTile)){
                groundTile.SetHighlight(HighlightType.Crosshair, false);
                groundTile.SetHighlight(HighlightType.Selected, false);
                groundTile.SetHighlight(HighlightType.Nope, false);
                groundTile.SetHighlight(HighlightType.Arrows, false);
            }
            
        }
    }

    public Tile GetRandomTile()
    {
        var randomPos = new Vector2(Random.Range(0, size - 1), Random.Range(0, size - 1));
        return GetTileAtPosition(randomPos);
    }

}
