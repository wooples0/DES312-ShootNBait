using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Debug")]
    public bool debugEnabled;
    public bool showEnemyCountOnTiles;

    [Header("Game")]
    public GameMode mode = GameMode.Default;
    public bool recordPlayerData;

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