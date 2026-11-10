using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Level")]

public class ScriptableLevel : ScriptableObject
{
    public int id;
    public int width = 0;
    public int height = 0;

    public List<Tile_Data> tileDataList = new List<Tile_Data>();
    public List<Vector2> shootPositions = new List<Vector2>();

    public Vector2 shootTilePosition;
}

[System.Serializable]
public struct Tile_Data
{
    public Vector2 position;
    public TileType type;
    public int noOfEnemiesOnTile;

}