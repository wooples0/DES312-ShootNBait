using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Level")]

public class ScriptableLevel : ScriptableObject
{
    [SerializeField] private string levelName;
    private Dictionary<Vector2, Tile> levelTiles;
    private Dictionary<Vector2, BaseEnemy> enemiesInLevel;

    [SerializeField] private Color tileColour;
    [SerializeField] private Color tileOffsetColour;

}