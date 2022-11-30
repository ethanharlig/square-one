using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController
{

    PlayerController playerController;
    GridController gridController;

    private HashSet<Vector2Int> paintedTiles;

    Vector2Int lastPosition;

    public delegate void HitPaintedTileEvent(Vector2Int position);
    public event HitPaintedTileEvent OnPaintedTileHit;

    public SnakeController(PlayerController player, GridController gridController)
    {
        this.playerController = player;
        this.gridController = gridController;

        this.paintedTiles = new();
        Debug.Log("Instantiating tiles again");

        PlayerController.OnMoveFullyCompleted += OnPlayerMoveFinish;
    }

    public void Reset()
    {
        PlayerController.OnMoveFullyCompleted -= OnPlayerMoveFinish;
    }

    private void OnPlayerMoveFinish(Vector2Int playerPosition, bool shouldCountMove)
    {
        if (lastPosition.x == playerPosition.x && lastPosition.y == playerPosition.y)
        {
            // player hasn't moved
            // TODO: find better solution for this, maybe in trigger?
            return;
        }

        // check to see if we have already hit this tile
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
            lastPosition = playerPosition;
        }

    }
}
