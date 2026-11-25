using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private Tile groundTile, wallTile;
    [SerializeField] private Transform cam;
    [SerializeField] private GameObject circleMarker;
    public SpriteRenderer markerRenderer;

    private Dictionary<Vector2, Tile> tiles = new Dictionary<Vector2, Tile>();
    private List<Tile> highlightedTiles = new List<Tile>();

    private void Awake()
    {
        Instance = this;
        Tile.hoveredTile = null;
        Tile.goalTile = null;
    }
    private void Start()
    {

    }
    private void Update()
    {
        if (GameManager.Instance.GameState == GameState.InitialiseLevel || GameManager.Instance.GameState == GameState.SpawnEnemies)
        {
            return;
        }
        ClearAllTileHighlights();
        UpdateTileHighlights();

    }
    
    public void UpdateTileHighlights()
    {
        if (GameManager.Instance.GameState != GameState.PlayerPhase) { return; }
        
        var targetTile = Tile.hoveredTile;
        
        if(targetTile == null) { Tile.hoveredTile = null; return; }

        GunManager.Instance.targetedTiles.Clear();
        highlightedTiles.Add(targetTile);

        if (GunManager.Instance.isGunEquipped)
        {
            if(targetTile.TryGetComponent<GroundTile>(out GroundTile groundTile))
            {
                groundTile.SetHighlight(HighlightType.Crosshair, true);
            }
            else if(targetTile.TryGetComponent<WallTile>(out WallTile wallTile))
            {
                wallTile.SetHighlight(HighlightType.Nope, true);
            }
            
            return;
        }

        var enemiesOnTile = targetTile.GetEnemiesOnTile().Count > 0;

        if(enemiesOnTile || !targetTile.Walkable)
        {
            targetTile.SetHighlight(HighlightType.Nope, true);
            return;
        }

        foreach (Tile tile in GetTilesInAxis(GunManager.Instance.equippedBait, targetTile))
        {
            highlightedTiles.Add(tile);

            if(tile.TryGetComponent<GroundTile>(out var groundTile))
            {
                GunManager.Instance.targetedTiles.Add(tile);
                tile.SetHighlight(HighlightType.Selected, true);
                
            }
        }
    }

    public IEnumerator GenerateGridAtSetSize()
    {
        CenterCameraOnGrid();
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {

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

    public IEnumerator GenerateGridFromLevel(ScriptableLevel level)
    {

        ResetGrid();
        width = level.width;
        height = level.height;
        var spawnDelay = level.width / 2 * 0.05f;

        CenterCameraOnGrid();

        List<Tile_Data> data = level.tileDataList;

        for(int x = 0; x < width; x++)
        {
            var even = x % 2 == 0;
            AudioManager.Instance.PlaySound(even ? "click1" : "click2");

            for (int y = 0; y < height; y++)
            {
                var spawnPos = new Vector2(x, y);
                Tile_Data tileDataAtPosition = data.Find(tileData => tileData.position == spawnPos);
                var tileToSpawn = tileDataAtPosition.type == TileType.Ground ? groundTile : wallTile;

                var spawnedTile = Instantiate(tileToSpawn, spawnPos, Quaternion.identity);

                if(tileDataAtPosition.noOfEnemiesOnTile > 0) { EnemyManager.AddEnemyToSpawnQueue(spawnedTile); }

                string tilePrefix = tileDataAtPosition.type == TileType.Ground ? "[G]" : "[W]";
                spawnedTile.name = $"{tilePrefix} Tile ({spawnPos.x}, {spawnPos.y})";
                spawnedTile.Init((int)tileDataAtPosition.position.x, (int)tileDataAtPosition.position.y);

                if (spawnPos == level.shootTilePosition && SettingsManager.Instance.mode == GameMode.Limited)
                {
                    Tile.goalTile = spawnedTile;
                    var marker = Instantiate(circleMarker, spawnPos, Quaternion.identity);
                    marker.transform.position += new Vector3(0, 0, -5);
                    markerRenderer = marker.GetComponentInChildren<SpriteRenderer>();
                    UIManager.Instance.spacebarPrompt = marker.GetComponentInChildren<Canvas>();
                }
                tiles[new Vector2((int)tileDataAtPosition.position.x, (int)tileDataAtPosition.position.y)] = spawnedTile;
            }
            
            yield return new WaitForSeconds(1f / Mathf.Pow(width, 0.9f));
        }

        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
        yield return null;
    }

    public void CenterCameraOnGrid()
    {
        //centers camera in middle of grid nd then changes orthographic size based on a value i tweaked until it looked right
        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.39f + (height/10f), -10);
        cam.GetComponent<Camera>().orthographicSize = ((width + height) / 3.0f);
    }

    public bool AllEnemiesOnTargetTile()
    {
        return Tile.goalTile.GetEnemiesOnTile().Count == EnemyManager.GetTotalEnemies();
    }
    public Tile GetTileAtPosition(Vector2 pos)
    {
        if(tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;
        
    }
    public Tile GetAdjacentTile(Axis axis, Tile tile, int offset)
    {
        if (offset != -1 && offset != 1)
        { Debug.Log($"Offset {offset} not valid. (Must be -1 or 1)"); 
            return null; 
        }

        var startPos = new Vector2((int)tile.GetPosition().x, (int)tile.GetPosition().y);

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

    public List<Tile> GetTilesInAxis(Axis axis, Tile tile)
    {
        if(tile == null) { return null; }
        List<Tile> tilesToReturn = new List<Tile>();
        var startPos = tile.GetPosition();
        if(!tiles.TryGetValue(startPos, out var tileAtStartPos))
        {
            return null;
        }
        
        if(axis == Axis.Vertical)
        {
            for(int i = 0; i < height; i++)
            {
                var indexTile = GetTileAtPosition(new Vector2((int)startPos.x, i));
                tilesToReturn.Add(indexTile);
            }
            return tilesToReturn;
        }
        else if(axis == Axis.Horizontal)
        {
            for (int i = 0; i < width; i++)
            {
                var indexTile = GetTileAtPosition(new Vector2(i, (int)startPos.y));
                tilesToReturn.Add(indexTile);
            }
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

    public List<Tile> GetTilesBetween(Axis axis, Tile startTile, Tile endTile)
    {
        var tilesBetween = new List<Tile>();
        var startTilePosition = startTile.GetPosition();
        var endTilePosition = endTile.GetPosition();

        if(axis == Axis.Horizontal)
        {
            var offset = startTilePosition.x > endTilePosition.x ? -1 : 1;
            var currentStepTile = GetTileAtPosition(new Vector2(startTilePosition.x + offset, startTilePosition.y));
            while (currentStepTile != endTile)
            {
                tilesBetween.Add(currentStepTile);
                
                currentStepTile = GetTileAtPosition(new Vector2(currentStepTile.GetPosition().x + offset, currentStepTile.GetPosition().y));   
            }

        }
        else if(axis == Axis.Vertical)
        {
            var offset = startTilePosition.y > endTilePosition.y ? -1 : 1;

            var currentStepTile = GetTileAtPosition(new Vector2(startTilePosition.x, startTilePosition.y + offset));

            while (currentStepTile != endTile)
            {
                tilesBetween.Add(currentStepTile);
                
                currentStepTile = GetTileAtPosition(new Vector2(currentStepTile.GetPosition().x, currentStepTile.GetPosition().y + offset));
            }
        }
        else if(axis == Axis.DiagonalL)
        {
            var offsetX = startTilePosition.x > endTilePosition.x ? -1 : 1;
            var offsetY = startTilePosition.y > endTilePosition.y ? -1 : 1;

            var currentStepTile = GetTileAtPosition(new Vector2(startTilePosition.x + offsetX, startTilePosition.y + offsetY));

            while(currentStepTile != endTile)
            {
                tilesBetween.Add(currentStepTile);
                currentStepTile = GetTileAtPosition(new Vector2(currentStepTile.GetPosition().x + offsetX, currentStepTile.GetPosition().y + offsetY));
            }
        }
        else if(axis == Axis.DiagonalR)
        {
            var offsetX = startTilePosition.x > endTilePosition.x ? 1 : -1;
            var offsetY = startTilePosition.y > endTilePosition.y ? 1 : -1;

            var currentStepTile = GetTileAtPosition(new Vector2(startTilePosition.x - offsetX, startTilePosition.y - offsetY));

            while (currentStepTile != endTile)
            {
                tilesBetween.Add(currentStepTile);
                currentStepTile = GetTileAtPosition(new Vector2(currentStepTile.GetPosition().x - offsetX, currentStepTile.GetPosition().y - offsetY));
            }
        }
        if (endTile != null) { tilesBetween.Add(endTile); }
        return tilesBetween;
    }
    public void ClearAllTileHighlights()
    {
        foreach(var tile in highlightedTiles)
        {
            if(tile == null) continue;
            if(tile.TryGetComponent<GroundTile>(out var ground))
            {
                ground.ClearAllHighlightsOnTile();
            }
            if(tile.TryGetComponent<WallTile>(out var wall))
            {
                wall.ClearAllHighlightsOnTile();
            }
        }
        highlightedTiles.Clear();
    }

    public Tile GetRandomTile()
    {
        var randomPos = new Vector2(Random.Range(0, width - 1), Random.Range(0, height - 1));
        return GetTileAtPosition(randomPos);
    }

    public void InitialiseValues()
    {
        
        width = PlayerPrefs.GetInt("width");
        height = PlayerPrefs.GetInt("height");
    }

    public void ResetGrid()
    {
        if (GameObject.Find("Highlight_Cross(Clone)")){ Destroy(GameObject.Find("Highlight_Cross(Clone)")); }
        Tile.hoveredTile = null;
        Tile.lastHoveredTile = null;
        Tile.goalTile = null;
        highlightedTiles.Clear();   

        foreach (var tile in tiles)
        {
            if (tile.Value != null)
            {
                Destroy(tile.Value.gameObject);
            }
        }

        tiles.Clear();
        
    }

    public int GetMaxGridSize()
    {
        return width > height ? width : height;
    }

}
