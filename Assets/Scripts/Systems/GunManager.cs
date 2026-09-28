using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunManager : MonoBehaviour
{
    public static GunManager Instance;
    public GunTypes equippedGun;
    public Tile targetTile;
    [SerializeField] private int activeItem = 0;
    [SerializeField] private int ammo = 5;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        equippedGun = GunTypes.Gun;
    }
    
    public void OnSwap(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) { return; }
        GridManager.Instance.ClearAllTileHighlights();
        switch (equippedGun)
        {
            case GunTypes.Gun:
                equippedGun = GunTypes.Bait1;
                break;
            case GunTypes.Bait1:
                equippedGun = GunTypes.Bait2;
                break;
            case GunTypes.Bait2:
                equippedGun = GunTypes.Gun;
                break;
        }
    }

    public void OnShoot(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {

        }
        if (ctx.canceled)
        {
        }
    }

    public void OnBait(InputAction.CallbackContext ctx)
    {

    }

    public enum GunTypes
    {
        Gun = 0,
        Bait1 = 1,
        Bait2 = 2,
    }
}
