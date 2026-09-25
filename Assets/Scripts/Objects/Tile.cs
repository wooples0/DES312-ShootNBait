using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color baseColour, offsetColour;
    [SerializeField] private SpriteRenderer renderer;
    
    public void Init(bool isOffset)
    {
        renderer.color = isOffset ? offsetColour : baseColour;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(name);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
