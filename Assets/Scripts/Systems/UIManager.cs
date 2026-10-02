using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [Header("Debug")]
    [SerializeField] private bool debugEnabled;
    public bool Debug => debugEnabled;
    public TMP_Text equippedGunText;
    public TMP_Text equippedBaitText;
    public TMP_Text targetTileText;
    public TMP_Text gameStateText;
    public TMP_Text bulletsUsedText;
    public TMP_Text baitUsedText;
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
                gameStateText.text = "Initialising Level...";
                break;
            case GameState.SpawnEnemies:
                gameStateText.text = "Spawn Phase";
                break;
            case GameState.PlayerPhase:
                gameStateText.text = "Player Phase";
                break;
            case GameState.ShootPhase:
                gameStateText.text = "Shoot Phase";
                break;
            case GameState.MovePhase:
                gameStateText.text = "Moving Phase";
                break;
            default:
                gameStateText.text = "NULL";
                break;
        }
        if (GunManager.Instance.isGunEquipped)
        {
            equippedGunText.text = "> YES";
            switch (GunManager.Instance.equippedBait)
            {
                case GunManager.BaitType.Horizontal:
                    equippedBaitText.text = "Horizontal";
                    break;
                case GunManager.BaitType.Vertical:
                    equippedBaitText.text = "Vertical";
                    break;
                case GunManager.BaitType.DiagonalR:
                    equippedBaitText.text = "DiagonalRight";
                    break;
                case GunManager.BaitType.DiagonalL:
                    equippedBaitText.text = "DiagonalLeft";
                    break;
            }
            equippedGunText.color = equippedGunTextColour1;
            equippedBaitText.color = equippedBaitTextColour2;
        }
        else
        {
            equippedGunText.text = "NO";
            equippedGunText.color = equippedGunTextColour2;
            equippedBaitText.color = equippedBaitTextColour1;
            switch (GunManager.Instance.equippedBait)
            {
                case GunManager.BaitType.Horizontal:
                    equippedBaitText.text = "> Horizontal";
                    break;
                case GunManager.BaitType.Vertical:
                    equippedBaitText.text = "> Vertical";
                    break;
                case GunManager.BaitType.DiagonalR:
                    equippedBaitText.text = "> DiagonalRight";
                    break;
                case GunManager.BaitType.DiagonalL:
                    equippedBaitText.text = "> DiagonalLeft";
                    break;
            }
        }

        if (GunManager.Instance.targetTile != null)
        {
            var targetTilePos = GunManager.Instance.targetTile.GetPosition();
            targetTileText.text = $"X({targetTilePos.x}) Y({targetTilePos.y})";
        }
        else
        {
            targetTileText.text = "NO TILE TARGETED";
        }

        bulletsUsedText.text = ScoreManager.Instance.bulletsUsed.ToString();
        baitUsedText.text = ScoreManager.Instance.baitUsed.ToString();
    }

}
