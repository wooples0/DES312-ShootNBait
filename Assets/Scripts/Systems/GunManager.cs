using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class GunManager : MonoBehaviour
{
    public static GunManager Instance;
    public bool isGunEquipped;
    public BaitType equippedBait;
    public BaitType lastBaitUsed;
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
        GridManager.Instance.UpdateTileHighlights();
        if (GameManager.Instance.GameState != GameState.PlayerPhase) { return; }
        if (!ctx.started) { return; }
        if (isGunEquipped) { isGunEquipped = false; return; }

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
        }

            switch (equippedBait)
            {
                case BaitType.Vertical:
                    equippedBait = ctx.ReadValue<float>() > 0 ? BaitType.DiagonalR : BaitType.DiagonalL;
                    GridManager.Instance.UpdateTileHighlights();
                    break;
                case BaitType.Horizontal:
                    equippedBait = ctx.ReadValue<float>() > 0 ? BaitType.DiagonalL : BaitType.DiagonalR;
                    GridManager.Instance.UpdateTileHighlights();
                    break;
                case BaitType.DiagonalR:
                    equippedBait = ctx.ReadValue<float>() > 0 ? BaitType.Horizontal : BaitType.Vertical;
                    GridManager.Instance.UpdateTileHighlights();
                    break;
                case BaitType.DiagonalL:
                    equippedBait = ctx.ReadValue<float>() > 0 ? BaitType.Vertical : BaitType.Horizontal;
                    GridManager.Instance.UpdateTileHighlights();
                    break;
            }


    }

    public void OnShoot(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance.GameState != GameState.PlayerPhase) { return; }
        if (!ctx.started) { return; }
        if(targetTile == null) {  return; }

        if (isGunEquipped )
        {
            ScoreManager.Instance.IncrementBulletsUsed(1);
            GameManager.Instance.ChangeState(GameState.ShootPhase);
        }
        else
        {
            if(targetTile.GetEnemiesOnTile().Count > 0 || equippedBait == lastBaitUsed)
            {
                return;
            }
            OnBait();
        }
    }

    public void OnBait()
    {
        ScoreManager.Instance.IncrementBaitUsed(1);
        GameManager.Instance.ChangeState(GameState.MovePhase);
        lastBaitUsed = equippedBait;
        switch (equippedBait)
        {
            case BaitType.Horizontal:
                StartCoroutine(EnemyManager.Instance.SetEnemiesOnTiles(targetTile, targetedTiles));
                break;
            case BaitType.Vertical:
                StartCoroutine(EnemyManager.Instance.SetEnemiesOnTiles(targetTile, targetedTiles));
                break;
            case BaitType.DiagonalR:
                StartCoroutine(EnemyManager.Instance.SetEnemiesOnTiles(targetTile, targetedTiles));
                break;
            case BaitType.DiagonalL:
                StartCoroutine(EnemyManager.Instance.SetEnemiesOnTiles(targetTile, targetedTiles));
                break;
        }
    }

    public enum BaitType
    {
        Horizontal = 0,
        Vertical = 1,
        DiagonalR = 2,
        DiagonalL = 3
    }
}
