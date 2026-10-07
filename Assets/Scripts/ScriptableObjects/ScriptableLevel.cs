using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Level")]

public class ScriptableLevel : ScriptableObject
{
    public string levelName;
    public int size;
    public Dictionary<Vector2, Tile> levelTiles;
    public Dictionary<Vector2, BaseEnemy> enemiesInLevel;

    public Color tileColour;
    public Color tileOffsetColour;

}