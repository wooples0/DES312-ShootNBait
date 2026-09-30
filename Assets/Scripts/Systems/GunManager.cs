using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class GunManager : MonoBehaviour
{
    public static GunManager Instance;
    public bool isGunEquipped;
    public BaitType equippedBait;
    public Tile targetTile;

    public List<Tile> targetedTiles = new List<Tile>();

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        isGunEquipped = true;
    }
    public void OnEquipGun(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance.GameState != GameState.PlayerPhase) { return; }
        if (!ctx.started) { return; }
        if (isGunEquipped) { return; }

        isGunEquipped = true;
        //Do toher visual stuff
    }
    public void OnSwapBait(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance.GameState != GameState.PlayerPhase) { return; }
        if (!ctx.started) { return; }
        if (isGunEquipped)
        {
            isGunEquipped = false;
            return;
        }
        GridManager.Instance.ClearAllTileHighlights();
        switch (equippedBait)
        {
            case BaitType.Vertical:
                equippedBait = BaitType.Horizontal;
                break;
            case BaitType.Horizontal:
                equippedBait = BaitType.Vertical;
                break;
        }


    }

    public void OnShoot(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance.GameState != GameState.PlayerPhase) { return; }
        if (!ctx.started) { return; }
        
        if (isGunEquipped)
        {
            //Shoot gun at tile, score based on how many enemies on tile (get diagonal charges if you get all enemies in a wave in one shot?)
        }
        else
        {
            foreach(BaseEnemy enemy in EnemyManager.Instance.spawnedEnemies)
            {
                Debug.Log($"{enemy.name} target tile being set to {targetTile}");
                enemy.SetTargetTile(targetTile);
            }
            //switch (equippedBait)
            //{
            //    case BaitType.Horizontal:
            //        //Pull enemies in horizontal line
            //        break;
            //    case BaitType.Vertical:
            //        //pull enemies in vertical line
            //        break;
            //    case BaitType.BaitDiagonalL:
            //        //Pull enemies in line rotated 45 degrees
            //        break;
            //    case BaitType.BaitDiagonalR:
            //        //pull enemies in line rotated 45 degrees
            //        break;
            //}
        }
    }

    public void OnBait(InputAction.CallbackContext ctx)
    {

    }

    public enum BaitType
    {
        Horizontal = 0,
        Vertical = 1,
        BaitDiagonalR = 2,
        BaitDiagonalL = 3
    }
}
