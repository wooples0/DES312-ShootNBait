using UnityEngine;
using UnityEngine.InputSystem;

public class GunManager : MonoBehaviour
{
    [SerializeField] private int activeItem = 0;
    [SerializeField] private int ammo = 5;
    public void OnSwap(InputAction.CallbackContext ctx)
    {
        Debug.Log("Swapping");
    }

    public void OnShoot(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {

        }
        if (ctx.canceled)
        {
            Debug.Log("Button lifted");
        }
    }

    public void OnBait(InputAction.CallbackContext ctx)
    {

    }
}
