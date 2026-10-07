using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class GunManager : MonoBehaviour
{
    public static GunManager Instance;
    public bool isGunEquipped;
    public Axis equippedBait;
    public Axis lastBaitUsed;
    public bool lastBaitUsedIsEquipped
    {
        get
        {
            return lastBaitUsed == equippedBait;
        }
    }

    public Tile targetTile;

    public List<Tile> targetedTiles = new List<Tile>();

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        isGunEquipped = true;
        equippedBait = Axis.Horizontal;
        lastBaitUsed = Axis.None;
    }
    public void OnEquipGun(InputAction.CallbackContext ctx)
    {
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
            case Axis.Vertical:
                equippedBait = ctx.ReadValue<float>() > 0 ? Axis.DiagonalR : Axis.DiagonalL;

                break;
            case Axis.Horizontal:
                equippedBait = ctx.ReadValue<float>() > 0 ? Axis.DiagonalL : Axis.DiagonalR;

                break;
            case Axis.DiagonalR:
                equippedBait = ctx.ReadValue<float>() > 0 ? Axis.Horizontal : Axis.Vertical;

                break;
            case Axis.DiagonalL:
                equippedBait = ctx.ReadValue<float>() > 0 ? Axis.Vertical : Axis.Horizontal;

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
        StartCoroutine(EnemyManager.Instance.SetEnemiesOnTiles(targetTile, targetedTiles));
    }

    
}

public enum Axis
{
    None = 0,
    Horizontal = 1,
    Vertical = 2,
    DiagonalR = 3,
    DiagonalL = 4
}
