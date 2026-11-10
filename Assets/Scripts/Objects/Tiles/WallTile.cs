using UnityEngine;
using UnityEngine.EventSystems;

public class WallTile : Tile
{

    public override void Init(int x, int y)
    {
        positionLabel.gameObject.SetActive(false);
        IsWalkable = false;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        Tile.hoveredTile = this;
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (Tile.hoveredTile == this)
        {
            SetHighlight(HighlightType.Nope, false);
            Tile.hoveredTile = null;
        }
    }


}
