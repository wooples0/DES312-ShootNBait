using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        LevelEditor_GridGenerator.Instance.GenerateGrid();
    }

    public void SetTilesInGridToDefault()
    {
        LevelEditor_GridGenerator.Instance.DefaultAllTiles();
    }

    public void ButtonPressed_LoadLevel()
    {
        LevelEditor_GridGenerator.Instance.LoadGrid(levels[levelDropdown.value]);
        gridSizeDropdown.value = levels[levelDropdown.value].width - LevelEditor_GridGenerator.Instance.minGridSize;
        loadLevelText.GetComponent<FadeText>().Fade(0, 1f);
    }
    public void DropdownValueChanged_LoadLevel()
    {
        ButtonPressed_LoadLevel();
    }

    public void ButtonPressed_SaveLevel()
    {
        if(saveLevelName == string.Empty || saveLevelName == null) { return; }
        if (!loadLevelButton.gameObject.activeSelf) { loadLevelButton.gameObject.SetActive(true); levelDropdown.gameObject.SetActive(true); ReloadLevelList(); }
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

        if (levels.Count == 0)
        {
            levelDropdown.gameObject.SetActive(false);
            loadLevelButton.gameObject.SetActive(false);
            return;
        }
        levelDropdown.ClearOptions();
        levelDropdown.gameObject.SetActive(true);
        loadLevelButton.gameObject.SetActive(true);

        for (int i = 0; i < levels.Count; i++)
        {
            levelDropdown.options.Add(
                new TMP_Dropdown.OptionData(
                    $"{levels[i].name}", null, Color.white));
        }

        selectedLevel = levels[levels.Count - 1];

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
