using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController
{

    PlayerController playerController;
    GridController gridController;

    private HashSet<Vector2Int> paintedTiles;

    public delegate void HitPaintedTileEvent(Vector2Int position);
    public event HitPaintedTileEvent OnPaintedTileHit;

    public SnakeController(PlayerController player, GridController gridController)
    {
        this.playerController = player;
        this.gridController = gridController;

        this.paintedTiles = new();

        PlayerController.OnMoveFinish += OnPlayerMoveFinish;
    }

    private void OnPlayerMoveFinish(Vector2Int playerPosition, bool shouldCountMove)
    {
        // once level is done, can still use snake for fun
        if (paintedTiles.Contains(playerPosition))
        {
            if (OnPaintedTileHit != null)
            {
                OnPaintedTileHit(playerPosition);
            }
        }

        if (gridController.TileAtLocation(playerPosition) != null)
        {
            gridController.PaintTileAtLocation(playerPosition, Color.red);
            paintedTiles.Add(playerPosition);
        }

    }
}
