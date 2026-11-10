using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public abstract class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static Tile hoveredTile;
    public static Tile goalTile;

    [SerializeField] protected SpriteRenderer renderer;
    [SerializeField] private GameObject h_Crosshair, h_Selected, h_Nope, h_Arrows;
    [SerializeField] protected TMP_Text positionLabel;
    [SerializeField] protected TMP_Text countLabel;

    [SerializeField] private List<BaseEnemy> enemiesOnTile = new List<BaseEnemy>();
    [SerializeField] protected bool IsWalkable;
    public bool Walkable => IsWalkable;

    void Awake()
    {
    }

    public virtual void Init(int x, int y)
    {
        positionLabel.text = ($"{x.ToString()},{y.ToString()}");

    }

    private void Update()
    {
        if (Tile.goalTile == this) 
        {
            SetHighlight(HighlightType.Crosshair, true);
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        Tile.hoveredTile = this;



    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        //if(Tile.hoveredTile == this)
        //{
        //    Tile.hoveredTile = null;
        //}
    }

    public Vector2 GetPosition()
    {
        return new Vector2((int)transform.position.x, (int)transform.position.y);
    }

    public List<BaseEnemy> GetEnemiesOnTile()
    {
        return enemiesOnTile;
    }

    public void AddEnemyToTile(BaseEnemy enemy)
    {
        if(enemiesOnTile.Contains(enemy))
        {
            
        }
        else
        {
            
            enemiesOnTile.Add(enemy);
            UpdateEnemyCountLabel();
        }
    }

    public void RemoveEnemyFromTile(BaseEnemy enemy)
    {
        if (enemiesOnTile.Contains(enemy))
        {
            //Debug.Log($"Removing {enemy.name} from list.");
            enemiesOnTile.Remove(enemy);
            UpdateEnemyCountLabel();
            //Debug.Log($"Count now {enemiesOnTile.Count.ToString()}");
        }
        else
        {
            //Debug.Log("Enemy not found in list");
        }
    }

    public void UpdateEnemyCountLabel()
    {
        if (!SettingsManager.Instance.showEnemyCountOnTiles)
        {
            countLabel.text = string.Empty;
            return;
        }
        if(enemiesOnTile.Count == 0)
        {
            countLabel.text = "0";
            countLabel.gameObject.SetActive(false);
            return;
        }

        if (!countLabel.gameObject.activeSelf) { countLabel.gameObject.SetActive(true); }

        countLabel.text = enemiesOnTile.Count.ToString();

    }
    public void SetHighlight(HighlightType h_Type, bool value)
    {
        switch (h_Type)
        {
            case HighlightType.Crosshair://If gun is hovering this tile
                h_Crosshair.SetActive(value);
                break;

            case HighlightType.Selected://If this tile is in range of bait but not target tile
                h_Selected.SetActive(value);
                if (GunManager.Instance.lastBaitUsedIsEquipped)
                //Bait NOT able to be used
                { 
                    h_Selected.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.2f);
                    if (Tile.hoveredTile == this) { SetHighlight(HighlightType.Nope, true); }
                }
                else
                //Bait able to be used
                { 
                    h_Selected.GetComponent<SpriteRenderer>().color = new Color(1, 1f, 0, 0.2f);
                    if (Tile.hoveredTile == this) { SetHighlight(HighlightType.Arrows, true); }
                    switch (GunManager.Instance.equippedBait)
                    {
                        case Axis.Vertical:
                            h_Arrows.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                            break;
                        case Axis.Horizontal:
                            h_Arrows.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                            break;
                        case Axis.DiagonalL:
                            h_Arrows.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45));
                            break;
                        case Axis.DiagonalR:
                            h_Arrows.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45));
                            break;
                    }
                }    
                break;

            case HighlightType.Nope://If this tile is hovered but can't bait on this tile
                h_Nope.SetActive(value);
                break;

            case HighlightType.Arrows://If this tile is hovered and can bait on this tile
                h_Arrows.SetActive(value);
                break;
        }
    }

    public void ClearAllHighlightsOnTile()
    {
        if (h_Crosshair != null) h_Crosshair.SetActive(false);
        if (h_Selected != null) h_Selected.SetActive(false);
        if (h_Arrows != null) h_Arrows.SetActive(false);
        if (h_Nope != null) h_Nope.SetActive(false);
    }
}

[System.Serializable]
public enum HighlightType
{
    Crosshair = 0,
    Selected = 1,
    Nope = 2,
    Arrows = 3
}
