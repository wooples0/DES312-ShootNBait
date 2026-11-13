using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
public class LevelEditor_UIManager : MonoBehaviour
{
    public static LevelEditor_UIManager Instance;
    [SerializeField] Button saveLevelButton, loadLevelButton, generateGridButton, clearGridButton;
    [SerializeField] TMP_InputField saveLevelInputField;
    [SerializeField] TMP_Dropdown levelDropdown, gridSizeDropdown;
    [SerializeField] TMP_Text saveLevelText, loadLevelText;

    private string saveLevelName;

    private List<ScriptableLevel> levels;
    private ScriptableLevel selectedLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Instance = this;
        
    }
    void Start()
    {
        InitialiseUIOptions();
        
    }

    void InitialiseUIOptions()
    {
        gridSizeDropdown.ClearOptions();
        //set dropdowns
        for(int i = 0; i < LevelEditor_GridGenerator.Instance.maxGridSize; i++)
        {
            gridSizeDropdown.options.Add(
                new TMP_Dropdown.OptionData(
                    $"{i+LevelEditor_GridGenerator.Instance.minGridSize}", null, Color.white));

        }
        gridSizeDropdown.value = LevelEditor_GridGenerator.Instance.width - 2;

        ReloadLevelList();

    }


    public void ButtonPressed_GenGrid()
    {
        if (!clearGridButton.gameObject.activeSelf) { clearGridButton.gameObject.SetActive(true); }
        LevelEditor_GridGenerator.Instance.GenerateGrid();
    }

    public void DropdownValueChanged_GenGrid()
    {

        LevelEditor_GridGenerator.Instance.width = gridSizeDropdown.value + LevelEditor_GridGenerator.Instance.minGridSize;
        LevelEditor_GridGenerator.Instance.height = gridSizeDropdown.value + LevelEditor_GridGenerator.Instance.minGridSize;
    }

    public void SetTilesInGridToDefault()
    {
        //LevelEditor_GridGenerator.Instance.DefaultAllTiles();
    }

    public void ButtonPressed_LoadLevel()
    {
        selectedLevel = levels[levelDropdown.value];
        LevelEditor_GridGenerator.Instance.LoadGrid(selectedLevel);
        Debug.Log($"Loading {selectedLevel.name}");
        loadLevelText.GetComponent<FadeText>().Fade(0, 1f);
    }

    public void ButtonPressed_SaveLevel()
    {
        if (saveLevelName == string.Empty) { Debug.Log("Saved name cannot be null"); return; }
        LevelEditor_DataManager.Instance.SaveLevelToList(saveLevelName, LevelEditor_GridGenerator.Instance.GetTiles(), LevelEditor_GridGenerator.Instance.width, LevelEditor_GridGenerator.Instance.height);
        saveLevelText.GetComponent<FadeText>().Fade(0, 1f);
        ReloadLevelList();
    }
    public void InputValueChanged_SaveLevel()
    {
        saveLevelName = saveLevelInputField.text;
    }

    //public void DropdownMenuOpened_LoadLevel()
    //{
    //    ReloadLevelList();
    //}

    private void ReloadLevelList()
    {
        
        levels = LevelEditor_DataManager.Instance.GetLevels();
        Debug.Log(levels.Count);

        levelDropdown.ClearOptions();
        levelDropdown.gameObject.SetActive(true);
        loadLevelButton.gameObject.SetActive(true);

        for (int i = 0; i < levels.Count; i++)
        {
            levelDropdown.options.Add(
                new TMP_Dropdown.OptionData(
                    $"{levels[i].name}", null, Color.white));
        }

        levelDropdown.value = levelDropdown.options.Count - 1;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
#endif