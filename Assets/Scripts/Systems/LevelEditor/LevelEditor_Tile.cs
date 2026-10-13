using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class LevelEditor_Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TileType type;
    public bool enemyOnTile;
    public static LevelEditor_Tile hoveredTile;

    [SerializeField] private Color baseColour;
    [SerializeField]private Color offsetColour;

    [SerializeField] private SpriteRenderer renderer;

    [SerializeField] private TMP_Text enemyOnTileText;
    [SerializeField] private TMP_Text tileIsWallText;

    public void Init(int x, int y)
    {
        var isOffset = (x + y) % 2 == 1;
        renderer.color = isOffset ? offsetColour : baseColour;
        if (enemyOnTile) { enemyOnTileText.text = "Enemy"; }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoveredTile = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoveredTile = null;
    }

    public void ToggleWall()
    {
        if(type != TileType.Wall)
        {
            enemyOnTile = false;
            enemyOnTileText.text = string.Empty;
            tileIsWallText.text = "Wall";
            type = TileType.Wall;
        }
        else
        {
            tileIsWallText.text = string.Empty;
            type = TileType.Ground;
        }
    }

    public void ToggleEnemy()
    {
        if (enemyOnTile)
        {
            enemyOnTile = false;
            enemyOnTileText.text = string.Empty;
            type = TileType.Ground;
        }
        else
        {
            enemyOnTile = true;
            enemyOnTileText.text = "Enemy";
            tileIsWallText.text = string.Empty;
            type = TileType.Ground;
        }
        
    }

    public void ClearAllText()
    {
        enemyOnTileText.text = string.Empty;
        tileIsWallText.text = string.Empty;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
