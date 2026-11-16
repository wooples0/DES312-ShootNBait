using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Debug UI")]
    [SerializeField] private Canvas debugCanvas;
    private bool debugInitialised;
    public TMP_Text equippedGunText;
    public TMP_Text equippedBaitText;
    public TMP_Text targetTileText;
    public TMP_Text gameStateText;
    public TMP_Text bulletsUsedText;
    public TMP_Text baitUsedText;
    public TMP_Dropdown gridSizeDropdown;
    public Button reloadLevelButton;
    public Color equippedBaitTextColour1, equippedBaitTextColour2, equippedGunTextColour1, equippedGunTextColour2;

    [Header("Bait Axis UI")]
    [SerializeField] private Image BaitIconHZ;
    [SerializeField] private Image CooldownIconHZ;
    [SerializeField] private Image BaitIconVT;
    [SerializeField] private Image CooldownIconVT;
    [SerializeField] private Image BaitIconDR;
    [SerializeField] private Image CooldownIconDR;
    [SerializeField]private Image BaitIconDL;
    [SerializeField] private Image CooldownIconDL;

    [SerializeField] private Color cooldownColour1, cooldownColour2, highlightedColour1, highlightedColour2, baseColour;

    [Header("Level Information UI")]
    [SerializeField] private TMP_Text levelTitle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EnableAllDebugUI(SettingsManager.Instance.debugEnabled);
    }
    // Update is called once per frame
    void Update()
    {
        if (SettingsManager.Instance.debugEnabled) { UpdateDebugUI(); }
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
                case Axis.Horizontal:
                    equippedBaitText.text = "Horizontal";
                    break;
                case Axis.Vertical:
                    equippedBaitText.text = "Vertical";
                    break;
                case Axis.DiagonalR:
                    equippedBaitText.text = "DiagonalRight";
                    break;
                case Axis.DiagonalL:
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
                case Axis.Horizontal:
                    equippedBaitText.text = "> Horizontal";
                    break;
                case Axis.Vertical:
                    equippedBaitText.text = "> Vertical";
                    break;
                case Axis.DiagonalR:
                    equippedBaitText.text = "> DiagonalRight";
                    break;
                case Axis.DiagonalL:
                    equippedBaitText.text = "> DiagonalLeft";
                    break;
            }
        }

        if (Tile.hoveredTile != null)
        {
            var targetTilePos = Tile.hoveredTile.GetPosition();
            targetTileText.text = $"X({targetTilePos.x}) Y({targetTilePos.y})";
        }
        else
        {
            targetTileText.text = "NO TILE TARGETED";
        }

        bulletsUsedText.text = ScoreManager.Instance.bulletsUsed.ToString();
        baitUsedText.text = ScoreManager.Instance.baitUsed.ToString();
    }

    public void EnableAllDebugUI(bool val)
    {
        var objList = debugCanvas.GetComponentsInChildren<Transform>(true);

        foreach (var obj in objList)
        {
            obj.gameObject.SetActive(val);
        }
    }

    public void UpdateBaitUI(Axis selectedAxis, Axis lastUsedAxis)
    {
        ResetBaitUI();
        bool lastUsedAxisIsSelected = selectedAxis == lastUsedAxis;

        if (lastUsedAxis != Axis.None)
        {
            GetCorrespondingBaitUIObject(lastUsedAxis).color = cooldownColour1;
            GetCorrespondingBaitUIObject(lastUsedAxis).transform.Find("cooldownIcon").gameObject.SetActive(true);
            if (selectedAxis == lastUsedAxis)
            {
                GetCorrespondingBaitUIObject(lastUsedAxis).transform.Find("activeIcon").GetComponent<Image>().color = cooldownColour2;
                GetCorrespondingBaitUIObject(lastUsedAxis).transform.Find("activeIcon").gameObject.SetActive(true);
                return;
            }

        }

        if (selectedAxis != Axis.None) 
        { 
            GetCorrespondingBaitUIObject(selectedAxis).color = highlightedColour1;
            GetCorrespondingBaitUIObject(selectedAxis).transform.Find("activeIcon").gameObject.SetActive(true);
        }
        return;

    }

    public Image GetCorrespondingBaitUIObject(Axis axis)
    {
        switch (axis)
        {
            case Axis.Horizontal:
                return BaitIconHZ;
            case Axis.Vertical:
                return BaitIconVT;
            case Axis.DiagonalL:
                return BaitIconDL;
            case Axis.DiagonalR:
                return BaitIconDR;
            default:
                Debug.Log("Axis is Axis.None");
                return null;
        }
        
    }

    public void UpdateLevelTitle(string newTitle)
    {
        levelTitle.text = newTitle;
    }
    private void ResetBaitUI()
    {
        //GetCorrespondingBaitUIObject(Axis.Horizontal).color = baseColour;
        //GetCorrespondingBaitUIObject(Axis.Horizontal).transform.Find("cooldownIcon").gameObject.SetActive(false);
        //GetCorrespondingBaitUIObject(Axis.Vertical).color = baseColour;
        //GetCorrespondingBaitUIObject(Axis.Vertical).transform.Find("cooldownIcon").gameObject.SetActive(false);
        //GetCorrespondingBaitUIObject(Axis.DiagonalR).color = baseColour;
        //GetCorrespondingBaitUIObject(Axis.DiagonalR).transform.Find("cooldownIcon").gameObject.SetActive(false);
        //GetCorrespondingBaitUIObject(Axis.DiagonalL).color = baseColour;
        //GetCorrespondingBaitUIObject(Axis.DiagonalL).transform.Find("cooldownIcon").gameObject.SetActive(false);
        foreach (Axis axis in System.Enum.GetValues(typeof(Axis)))
        {
            if (axis == Axis.None) { continue; }
            GetCorrespondingBaitUIObject(axis).color = baseColour;
            GetCorrespondingBaitUIObject(axis).transform.Find("cooldownIcon").gameObject.SetActive(false);
            GetCorrespondingBaitUIObject(axis).transform.Find("activeIcon").gameObject.SetActive(false);
            GetCorrespondingBaitUIObject(axis).transform.Find("activeIcon").GetComponent<Image>().color = highlightedColour2;
        }

    }
    public void OnGridSizeDropdownChanged()
    {
        PlayerPrefs.SetInt("width", (gridSizeDropdown.value + 2));
        PlayerPrefs.SetInt("height", (gridSizeDropdown.value + 2));
        PlayerPrefs.Save();
    }

}
