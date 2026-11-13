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

    public List<Tile> targetedTiles = new List<Tile>();

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (SettingsManager.Instance.mode == GameMode.Default) { isGunEquipped = true; }
        else { isGunEquipped = false; }
        equippedBait = Axis.Horizontal;
        lastBaitUsed = Axis.None;
    }
    public void OnEquipGun(InputAction.CallbackContext ctx)
    {
        if (SettingsManager.Instance.mode == GameMode.Limited) { return; }
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
        if(Tile.hoveredTile == null) { Debug.Log("Hovered tile is null"); return; }
        if(!HoveringShootableTile()) { Debug.Log($"{Tile.hoveredTile.name} is not a shootable tile"); AudioManager.Instance.PlaySound("wrong"); return; }
       
        if (isGunEquipped)
        {
            if(Tile.hoveredTile.GetEnemiesOnTile().Count == 0)
            {
                AudioManager.Instance.PlaySound("wrong");
                return;
            }

            
            ScoreManager.Instance.IncrementBulletsUsed(1);
            GameManager.Instance.ChangeState(GameState.ShootPhase);
            
        }
        else
        {
            if(Tile.hoveredTile.GetEnemiesOnTile().Count > 0 || equippedBait == lastBaitUsed)
            {
                Debug.Log("Enemies on tile or you are trying to use your last used bait");
                AudioManager.Instance.PlaySound("wrong");
                return;
            }

            var enemyCount = 0;
            foreach(var tile in targetedTiles)
            {
                if(tile.GetEnemiesOnTile().Count > 0)
                {
                    enemyCount += tile.GetEnemiesOnTile().Count;
                }
            }
            if (enemyCount == 0) { AudioManager.Instance.PlaySound("wrong"); return; }
            OnBait();
        }
    }

    public void OnShootAlternate(InputAction.CallbackContext ctx)
    {
        if(SettingsManager.Instance.mode == GameMode.Default) { return; }
        if (!ctx.started) { return; }
        if (GameManager.Instance.GameState == GameState.ShootPhase) { return; }
        ScoreManager.Instance.IncrementBulletsUsed(1);
        GameManager.Instance.ChangeState(GameState.ShootPhase);
    }

    public void OnBait()
    {
        AudioManager.Instance.PlaySound("swoosh");
        ScoreManager.Instance.IncrementBaitUsed(1);
        GameManager.Instance.ChangeState(GameState.MovePhase);
        lastBaitUsed = equippedBait;
        StartCoroutine(EnemyManager.Instance.SetEnemiesOnTiles(Tile.hoveredTile, targetedTiles));
    }

    private bool HoveringShootableTile()
    {
        return Tile.hoveredTile.GetComponent<GroundTile>() != null;
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
