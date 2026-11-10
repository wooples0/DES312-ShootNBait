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
        UpdateSavedLevelsList();
        return savedLevels;
    }


#if UNITY_EDITOR
    public void SaveLevelToList(string name, Dictionary<Vector2, LevelEditor_Tile> tiles, int width, int height)
    {
        ScriptableLevel level = ScriptableObject.CreateInstance<ScriptableLevel>();
        
        level.name = name;
        level.width = width;
        level.height = height;

        foreach(var key in tiles.Keys)
        {
            if(tiles.TryGetValue(key, out LevelEditor_Tile tile))
            {
                Tile_Data t = new Tile_Data();
                t.position = key;

                if (tile.type == TileType.Ground)
                {
                    Debug.Log("tile type is ground");
                    t.type = TileType.Ground;
                }
                else if(tile.type == TileType.Wall)
                {
                    Debug.Log("tile type is wall");
                    t.type = TileType.Wall;
                }

                if (tile.enemyOnTile)
                {
                    Debug.Log("tile has enemy");
                    t.noOfEnemiesOnTile += 1;
                }

                level.tileDataList.Add(t);
            }
        }

        Debug.Log($"Tile data list is {level.tileDataList.Count()} tiles long");

        foreach (ScriptableLevel levels in savedLevels)
        {
            if (level.name == name) { AssetDatabase.DeleteAsset($"Assets/Resources/Levels/{level.name}.asset"); }
        }
        AssetDatabase.CreateAsset(level, $"Assets/Resources/Levels/{level.name}.asset");
        EditorUtility.SetDirty(level);
        AssetDatabase.SaveAssets();
        UpdateSavedLevelsList();
    }
#endif
}
