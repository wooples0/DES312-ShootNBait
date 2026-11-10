using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Debug")]
    public bool debugEnabled;
    public bool showEnemyCountOnTiles;

    [Header("GameMode")]
    public GameMode mode = GameMode.Default;

    void Awake()
    {
        Instance = this;
    }


}

public enum GameMode
{
    Default,
    Limited

}