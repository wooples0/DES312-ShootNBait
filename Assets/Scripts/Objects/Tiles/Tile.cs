using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private Color baseColour, offsetColour;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private GameObject highlight;
    private bool offset;
    public void Init(bool isOffset)
    {
        offset = isOffset;
        renderer.color = isOffset ? offsetColour : baseColour;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlight.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        highlight.SetActive(false);
        
    }
}
