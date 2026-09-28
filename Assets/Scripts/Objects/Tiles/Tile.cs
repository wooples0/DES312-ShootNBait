using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public abstract class Tile : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler
{
    public bool mousing;
    [SerializeField] protected SpriteRenderer renderer;
    [SerializeField] private GameObject highlight;
    [SerializeField] private GameObject highlight2;

    public virtual void Init(int x, int y)
    {
        
    }

    private void Update()
    {
    }
    private void LateUpdate()
    {
        //SetHighlight(1, false);
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

    public Vector2 GetXY()
    {
        return new Vector2(transform.position.x, transform.position.y);
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
