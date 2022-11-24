using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameStateManager
{

    List<(TileController, Color)> tilePath;

    int activeTile;

    private GridController gridController;
    private PlayerController playerController;

    private bool DEV_MODE = false;

    private int turnLimit;

    private bool turnLimitEnabled = false;

    enum GameState
    {
        START,
        PLAYING,
        SUCCESS,
        FAILED
    };
    GameState currentState;

    List<Action> actions;

    /*
        Constructor that takes no default parameters
    */
    public GameStateManager(PlayerController player, GridController grid)
    {
        playerController = player;
        gridController = grid;
        this.tilePath = new List<(TileController, Color)>();

        actions = new() {
            CheckWithinGrid,
            CheckPlayerPosition
        };
    }

    /*
        Constructor that initializes a tile path. Calls the default constructor
    */
    public GameStateManager(PlayerController player, GridController grid, (TileController, Color)[] tilePath) : this(player, grid)
    {
        if (tilePath.Length > 0)
        {
            activeTile = 0;
        }

        this.tilePath = new(tilePath);
    }

    // Adds a tile to the current active path
    public void AddTileToPath(TileController tile, Color color)
    {
        if (tilePath.Count == 0)
        {
            activeTile = 0;
        }

        tilePath.Add((tile, color));
    }

    // Sets if dev mode is enabled. False by default
    public void SetDevMode(bool devMode)
    {
        DEV_MODE = devMode;
    }

    public void SetTurnLimit(int turnLimit)
    {
        this.turnLimit = turnLimit;
        this.turnLimitEnabled = true;

        actions.Add(CheckTurnLimit);
    }

    private void CheckTurnLimit()
    {
        if (!turnLimitEnabled)
        {
            return;
        }
        if (playerController.GetMoveCount() >= turnLimit)
        {
            currentState = GameState.FAILED;
        }
    }

    private void CheckWithinGrid()
    {
        Vector2Int playerPos = playerController.GetCurrentPosition();
        if (!DEV_MODE && !gridController.IsWithinGrid(playerPos))
        {
            currentState = GameState.FAILED;
        }
    }

    private void CheckPlayerPosition()
    {
        Vector2Int playerPos = playerController.GetCurrentPosition();
        TileController playerTile = gridController.TileAtLocation(playerPos.x, playerPos.y);

        // check if player has hit the next active tile
        if (playerTile == tilePath[activeTile].Item1)
        {
            ActivateNextTile(playerPos);
        }

    }

    private void ActivateNextTile(Vector2Int playerPos)
    {
        activeTile += 1;
        // we've reached the end of the path and the game is over
        if (activeTile == tilePath.Count)
        {
            currentState = GameState.SUCCESS;
        }
        else
        {
            gridController.PaintTileAtLocation(playerPos, tilePath[activeTile].Item2);
        }
    }


    void ManageGameState()
    {
        switch (currentState)
        {
            case GameState.START:
                // welcome text or interaction?
                // make an LevelUI or something if theres a new mechanic to introduce
                break;
            case GameState.PLAYING:
                // player is currently playing the level
                // main checks and mechanics
                foreach (Action func in actions) {
                    func();
                }
                break;
            case GameState.SUCCESS:
                // player has met criteria for success on level
                break;
            case GameState.FAILED:
                // player has failed the level
                break;
            default:
                Debug.LogErrorFormat("Encountered unexpected game state: {0}", currentState);
                break;
        }
    }
}
