using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Level")]

public class ScriptableLevel : ScriptableObject
{
    public int id;
    public int width;
    public int height;
    public Dictionary<Vector2, int> gridLayout = new Dictionary<Vector2, int>();
    public Dictionary<Vector2, int> enemyLayout = new Dictionary<Vector2, int>();

}