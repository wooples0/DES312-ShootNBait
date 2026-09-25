using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private Color baseColour, offsetColour, hoverColour, highlightColour;
    [SerializeField] private SpriteRenderer renderer;
    private bool offset;
    public void Init(bool isOffset)
    {
        offset = isOffset;
        renderer.color = isOffset ? offsetColour : baseColour;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        renderer.color = hoverColour;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Init(offset);
    }
}
