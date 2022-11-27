using System.Collections.Generic;
using UnityEngine;
using System;


public class GameStateManager
{

    List<(Vector2Int, Color)> waypoints;

    int activeWaypoint;

    public bool AutoSpawnEnabled
    {
        get; set;
    }

    private GridController gridController;
    private PlayerController playerController;

    private bool DEV_MODE = false;

    private int turnLimit;

    private bool turnLimitEnabled = false;

    public delegate void WaypointHitAction(int idx, Vector2Int pos, Color color);
    public event WaypointHitAction OnWaypointHit;


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

        activeWaypoint = -1;

        this.waypoints = new List<(Vector2Int, Color)>();

        actions = new() {
            CheckWithinGrid,
            CheckPlayerPosition
        };
        TransitionState(GameState.START);
    }

    public GameStateManager(PlayerController player, GridController grid, (Vector2Int, Color)[] waypoints) : this(player, grid)
    {
        this.waypoints = new(waypoints);
    }

    public void SetWaypoints((Vector2Int, Color)[] waypoints, bool autoTrack)
    {
        this.waypoints = new(waypoints);

        this.AutoSpawnEnabled = autoTrack;
    }

    // Adds a tile to the current active path
    public void AddWaypoint((Vector2Int, Color) waypoint)
    {
        if (waypoints.Count == 0)
        {
            activeWaypoint = 0;
        }

        waypoints.Add(waypoint);
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
            TransitionState(GameState.FAILED);
        }
    }

    private void CheckWithinGrid()
    {
        Vector2Int playerPos = playerController.GetCurrentPosition();
        if (!DEV_MODE && !gridController.IsWithinGrid(playerPos))
        {
            TransitionState(GameState.FAILED);
        }
    }

    private void CheckPlayerPosition()
    {
        Vector2Int playerPos = playerController.GetCurrentPosition();
        Vector2Int nextWaypoint = waypoints[activeWaypoint].Item1;
        // check if player has hit the next active tile
        if (playerPos.x == nextWaypoint.x && playerPos.y == nextWaypoint.y)
        {
            if (OnWaypointHit != null)
            {
                OnWaypointHit(activeWaypoint, waypoints[activeWaypoint].Item1, waypoints[activeWaypoint].Item2);
            }

            if (AutoSpawnEnabled)
            {
                SpawnNextWaypoint();
            }
        }

    }

    /**
       * Spawns next waypoint in the waypoints arrray and paints correct color 
    */
    public void SpawnNextWaypoint()
    {
        Debug.Log("Spawning next Waypoint");
        activeWaypoint += 1;

        if (activeWaypoint == waypoints.Count)
        {
            TransitionState(GameState.SUCCESS);
        }
        else
        {
            gridController.SpawnWaypoint(waypoints[activeWaypoint].Item1);
            gridController.PaintTileAtLocation(waypoints[activeWaypoint].Item1, waypoints[activeWaypoint].Item2);
        }

    }

    void TransitionState(GameState nextState) {
        currentState = nextState;
    }


    public void ManageGameState()
    {
        switch (currentState)
        {
            case GameState.START:
                // welcome text or interaction?
                // make an LevelUI or something if theres a new mechanic to introduce
                if (AutoSpawnEnabled) {
                    SpawnNextWaypoint();
                }
                TransitionState(GameState.PLAYING);
                break;
            case GameState.PLAYING:
                // player is currently playing the level
                // main checks and mechanics
                foreach (Action func in actions)
                {
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
