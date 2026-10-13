using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class LevelEditor_DataManager : MonoBehaviour
{
    public static LevelEditor_DataManager Instance;
    private List<ScriptableLevel> savedLevels = new List<ScriptableLevel>();

    void Awake()
    {
        Instance = this;
        UpdateSavedLevelsList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSavedLevelsList()
    {

        savedLevels = Resources.LoadAll<ScriptableLevel>("Levels").ToList();
    }

    public List<ScriptableLevel> GetLevels()
    {
        return savedLevels;
    }


#if UNITY_EDITOR
    public void SaveLevelToList(string name, Dictionary<Vector2, LevelEditor_Tile> tiles, int width, int height)
    {
        ScriptableLevel level = ScriptableObject.CreateInstance<ScriptableLevel>();
        foreach(ScriptableLevel levels in savedLevels)
        {
            if(level.name == name) { AssetDatabase.DeleteAsset($"Assets/Resources/Levels/{level.name}.asset"); }
        }
        level.name = name;
        level.width = width;
        level.height = height;

        foreach(var key in tiles.Keys)
        {
            if(tiles.TryGetValue(key, out LevelEditor_Tile tile))
            {
                if(tile.type == TileType.Ground)
                {
                    level.gridLayout.Add(key, 0);
                }
                else if(tile.type == TileType.Wall)
                {
                    level.gridLayout.Add(key, 1);
                }

                if (tile.enemyOnTile)
                {
                    level.enemyLayout.Add(key, 0);
                }
            }
        }


        AssetDatabase.CreateAsset(level, $"Assets/Resources/Levels/{level.name}.asset");
        AssetDatabase.SaveAssets();
        savedLevels = Resources.LoadAll<ScriptableLevel>("Levels").ToList();
    }
#endif
}
