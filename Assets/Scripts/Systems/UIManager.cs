using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [Header("Debug")]
    [SerializeField] private bool debugEnabled;
    public TMP_Text equippedGunText;
    public TMP_Text equippedBaitText;
    public TMP_Text targetTileText;
    public TMP_Text gameStateText;
    public Color equippedBaitTextColour1, equippedBaitTextColour2, equippedGunTextColour1, equippedGunTextColour2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (debugEnabled) { UpdateDebugUI(); }
    }

    void UpdateDebugUI()
    {
        switch (GameManager.Instance.GameState)
        {
            case GameState.InitialiseLevel:
                gameStateText.text = "[Initialising Level]";
                break;
            case GameState.SpawnEnemies:
                gameStateText.text = "[Spawning Enemies]";
                break;
            case GameState.PlayerPhase:
                gameStateText.text = "[Player Phase]";
                break;
            case GameState.ShootPhase:
                gameStateText.text = "[Shoot Phase]";
                break;
            case GameState.MovePhase:
                gameStateText.text = "[Moving Enemies]";
                break;
            default:
                break;
        }
        if (GunManager.Instance.isGunEquipped)
        {
            equippedGunText.text = "[1] Equipped Gun: YES";
            equippedBaitText.text = GunManager.Instance.equippedBait == GunManager.BaitType.Horizontal ? "[2] EquippedBait: HZ" : "[2] EquippedBait: VT";
            equippedGunText.color = equippedGunTextColour1;
            equippedBaitText.color = equippedBaitTextColour2;
        }
        else
        {
            equippedGunText.text = "[1] Equipped Gun: NO!";
            equippedGunText.color = equippedGunTextColour2;
            equippedBaitText.color = equippedBaitTextColour1;
            if(GunManager.Instance.equippedBait == GunManager.BaitType.Horizontal)
            {
                equippedBaitText.text = "[2] EquippedBait: HZ";
            }
            else if (GunManager.Instance.equippedBait == GunManager.BaitType.Vertical)
            {
                equippedBaitText.text = "[2] EquippedBait: VT";
            }
            else
            {
                equippedBaitText.text = "[2] EquippedBait: NULL";
            }
        }

        if (GunManager.Instance.targetTile != null)
        {
            var targetTilePos = GunManager.Instance.targetTile.GetPosition();
            targetTileText.text = $"Target Tile : X({targetTilePos.x}) Y({targetTilePos.y})";
        }
        else
        {
            targetTileText.text = "Target Tile : NONE";
        }
    }

}
