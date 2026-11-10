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
        ClearAllTileHighlights();
        UpdateTileHighlights();
    }
    
    private void UpdateTileHighlights()
    {
        if (GameManager.Instance.GameState != GameState.PlayerPhase) { return; }
        
        var targetTile = Tile.hoveredTile;
        GunManager.Instance.targetedTiles.Clear();
        
        if(targetTile == null) { return; }

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
        width = level.width;
        height = level.height;

        //Debug.Log($"Level {level.name}");
        //Debug.Log($"Width {level.width}");
        //Debug.Log($"Height {level.height}");
        //Debug.Log($"List has {level.tileDataList.Count} tiles");

        CenterCameraOnGrid();

        foreach(Tile_Data data in level.tileDataList)
        {

            var spawnPos = data.position;


            var spawnedTile = Instantiate(data.type == TileType.Ground ? groundTile : wallTile, spawnPos, Quaternion.identity);
            
            if(data.noOfEnemiesOnTile > 0) { EnemyManager.AddEnemyToSpawnQueue(spawnedTile); }

            string tilePrefix = data.type == TileType.Ground ? "[G]" : "[W]";
            spawnedTile.name = $"{tilePrefix} Tile ({spawnPos.x}, {spawnPos.y})";
            spawnedTile.Init((int)spawnPos.x, (int)spawnPos.y);
            if (spawnPos == level.shootTilePosition && SettingsManager.Instance.mode == GameMode.Limited) 
            {
                Tile.goalTile = spawnedTile;
                var marker = Instantiate(circleMarker, spawnPos, Quaternion.identity);
                marker.SetActive(true);
                marker.transform.position += new Vector3(0, 0, -5);
            }
            tiles[new Vector2((int)spawnPos.x, (int)spawnPos.y)] = spawnedTile;
            yield return new WaitForSeconds(0.00025f);
        }

        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
        yield return null;
    }

    //public IEnumerator GeneratePremadeGrid(Dictionary<>)
    //{

    //}
    public void CenterCameraOnGrid()
    {
        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
        cam.GetComponent<Camera>().orthographicSize = ((width + height) / 4.0f);
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
        List<Tile> tilesToReturn = new List<Tile>();
        var startPos = tile.GetPosition();
        if(!tiles.TryGetValue(startPos, out var tileAtStartPos))
        {
            //Debug.Log($"Specified target tile {startPos} not found in grid.");
            return null;
        }
        
        if(axis == Axis.Vertical)
        {
            for(int i = 0; i < height; i++)
            {
                var indexTile = GetTileAtPosition(new Vector2((int)startPos.x, i));
                tilesToReturn.Add(indexTile);
            }
            //Debug.Log($"{tilesToReturn.Count} tiles in horizontal");
            return tilesToReturn;
        }
        else if(axis == Axis.Horizontal)
        {
            for (int i = 0; i < width; i++)
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
                Debug.Log(GetTileAtPosition(new Vector2(currentStepTile.GetPosition().x + offsetX, currentStepTile.GetPosition().y + offsetY)));
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
                Debug.Log(new Vector2(currentStepTile.GetPosition().x + offsetX, currentStepTile.GetPosition().y + offsetY));
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
        //highlightedTiles.Clear();
        //foreach(var tile in tiles.Values)
        //{
        //    if(tile.TryGetComponent<GroundTile>(out var groundTile)){
        //        groundTile.SetHighlight(HighlightType.Crosshair, false);
        //        groundTile.SetHighlight(HighlightType.Selected, false);
        //        groundTile.SetHighlight(HighlightType.Nope, false);
        //        groundTile.SetHighlight(HighlightType.Arrows, false);
        //    }

        //}
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

    public int GetMaxGridSize()
    {
        return width > height ? width : height;
    }

}
