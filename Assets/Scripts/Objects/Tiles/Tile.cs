using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public abstract class Tile : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler
{
    public bool mousing;
    [SerializeField] protected SpriteRenderer renderer;
    [SerializeField] private GameObject highlight;
    [SerializeField] private GameObject highlight2;
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

    public virtual void OnPointerMove(PointerEventData eventData)
    {
        GunManager.Instance.targetTile = this;
        SetHighlight(0, true);

    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if(GunManager.Instance.targetTile == this)
        {
            GunManager.Instance.targetTile = null;
        }
        SetHighlight(0, false);
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
            Debug.Log("Enemy already exists in list");
        }
        else
        {
            Debug.Log($"Adding {enemy.name} to list.");
            enemiesOnTile.Add(enemy);
            countLabel.text = enemiesOnTile.Count.ToString();
            countLabel.color = new Color(countLabel.color.r, countLabel.color.g, countLabel.color.b, countLabel.color.a + 0.2f);
            Debug.Log($"Count now {enemiesOnTile.Count.ToString()}");
        }
    }

    public void RemoveEnemyFromTile(BaseEnemy enemy)
    {
        if (enemiesOnTile.Contains(enemy))
        {
            Debug.Log($"Removing {enemy.name} from list.");
            enemiesOnTile.Remove(enemy);
            countLabel.text = enemiesOnTile.Count.ToString();
            countLabel.color = new Color(countLabel.color.r, countLabel.color.g, countLabel.color.b, countLabel.color.a - 0.2f);
            if(enemiesOnTile.Count == 0) { countLabel.color = new Color(countLabel.color.r, countLabel.color.g, countLabel.color.b, 0.2f); }
            Debug.Log($"Count now {enemiesOnTile.Count.ToString()}");
        }
        else
        {
            Debug.Log("Enemy not found in list");
        }
    }
    public void SetHighlight(int value, bool set)
    {
        switch (value)
        {
            case 0:
                highlight.SetActive(set);
                break;
            case 1:
                highlight2.SetActive(set); 
                break;
        }
    }
}
