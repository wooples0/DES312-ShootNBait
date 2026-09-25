using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public abstract class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected SpriteRenderer renderer;
    [SerializeField] private GameObject highlight;

    public virtual void Init(int x, int y)
    {
        
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        highlight.SetActive(true);
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        highlight.SetActive(false);
        
    }
}
