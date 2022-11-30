using System.Collections.Generic;
using UnityEngine;

// snake
public class LevelTwoManager : LevelManager
{
    private Dictionary<TileController, bool> paintedTiles;

    // TODO add fun message if you paint whole map :)

#pragma warning disable IDE0051
    void Start()
    {
        gridSizeX = gridSizeY = 10;
        turnLimit = 5;

        SetupLevel();

        paintedTiles = new Dictionary<TileController, bool>();
    }
#pragma warning restore IDE0051

    override protected void OnPlayerMoveFinish(Vector2Int playerPosition)
    {
        Vector2Int playerPos = playerController.GetCurrentPosition();

        // once level is done, can still use snake for fun
        if (!levelActive)
        {
            gridController.PaintTileAtLocation(playerPos, Color.red);
            return;
        }

        if (levelActive)
        {
            turnsLeft = turnLimit - playerController.GetMoveCount();
        }
        SetMoveCountText();

        // win by making it to no turns I guess
        if (turnsLeft == 0)
        {
            SetTerminalGameState(levelUIElements.GetSuccessElements());
        }

        CheckFailureAndPaintTile(playerPos);
    }

    // TODO probably want to use this in other levels, should be a mechanic
    void CheckFailureAndPaintTile(Vector2Int playerPos)
    {
        TileController tile = gridController.TileAtLocation(playerPos);

        if (tile == null)
        {
            Debug.Log("Out of map bounds");
            SetTerminalGameState(levelUIElements.GetFailedElements(), 0f);
        }
        else if (HasTile(tile))
        {
            Debug.Log("Hit already painted tile");
            SetTerminalGameState(levelUIElements.GetFailedElements(), 0f);
        }
        else
        {
            Debug.LogFormat("Time to paint at this pos: {0}", playerPos);
            gridController.PaintTileAtLocation(playerPos, Color.red);
            paintedTiles.Add(tile, true);
        }

        bool HasTile(TileController tile)
        {
            try
            {
                return paintedTiles[tile];
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
    }

}
