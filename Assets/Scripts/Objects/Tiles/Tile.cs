using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public abstract class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected SpriteRenderer renderer;
    [SerializeField] private GameObject h_Crosshair, h_Selected, h_Nope, h_Arrows;
    [SerializeField] protected TMP_Text positionLabel;
    [SerializeField] protected TMP_Text countLabel;

    [SerializeField] private List<BaseEnemy> enemiesOnTile = new List<BaseEnemy>();
    [SerializeField] private bool IsWalkable;
    public bool Walkable => IsWalkable;

    public virtual void Init(int x, int y)
    {
        positionLabel.text = ($"{x.ToString()},{y.ToString()}");
    }

    private void Update()
    {
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        GunManager.Instance.targetTile = this;



    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if(GunManager.Instance.targetTile == this)
        {
            GunManager.Instance.targetTile = null;
        }
    }

    public Vector2 GetPosition()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public List<BaseEnemy> GetEnemiesOnTile()
    {
        return enemiesOnTile;
    }

    public void AddEnemyToTile(BaseEnemy enemy)
    {
        if(enemiesOnTile.Contains(enemy))
        {
            //Debug.Log("Enemy already exists in list");
        }
        else
        {
            //Debug.Log($"Adding {enemy.name} to list.");
            enemiesOnTile.Add(enemy);
            countLabel.text = enemiesOnTile.Count.ToString();
            countLabel.color = new Color(countLabel.color.r, countLabel.color.g, countLabel.color.b, countLabel.color.a + 0.2f);
            //Debug.Log($"Count now {enemiesOnTile.Count.ToString()}");
        }
    }

    public void RemoveEnemyFromTile(BaseEnemy enemy)
    {
        if (enemiesOnTile.Contains(enemy))
        {
            //Debug.Log($"Removing {enemy.name} from list.");
            enemiesOnTile.Remove(enemy);
            countLabel.text = enemiesOnTile.Count.ToString();
            countLabel.color = new Color(countLabel.color.r, countLabel.color.g, countLabel.color.b, countLabel.color.a - 0.2f);
            if(enemiesOnTile.Count == 0) { countLabel.color = new Color(countLabel.color.r, countLabel.color.g, countLabel.color.b, 0.2f); }
            //Debug.Log($"Count now {enemiesOnTile.Count.ToString()}");
        }
        else
        {
            //Debug.Log("Enemy not found in list");
        }
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
                    if (GunManager.Instance.targetTile == this) { SetHighlight(HighlightType.Nope, true); }
                }
                else
                //Bait able to be used
                { 
                    h_Selected.GetComponent<SpriteRenderer>().color = new Color(1, 1f, 0, 0.2f);
                    if (GunManager.Instance.targetTile == this) { SetHighlight(HighlightType.Arrows, true); }
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

}

public enum HighlightType
{
    Crosshair = 0,
    Selected = 1,
    Nope = 2,
    Arrows = 3
}
