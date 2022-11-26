using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// introduces player to camera movement, uses ice
public class WallLevel1 : LevelManager
{
    private List<GameState> gameStateOrder;
    private GameState currentGameState;

    private List<MovingObstacle> obstacles;
    int timesWallMoved = 0;


    enum GameState
    {
        START,
        GREEN_SETUP,
        GREEN_HIT,
        RED_SETUP,
        RED_HIT,
        BLUE_SETUP,
        BLUE_HIT,
        SUCCESS,
        FAILED,
    };

#pragma warning disable IDE0051
    void Start()
    {
        gridSizeX = gridSizeY = 6;
        turnLimit = 13;

        gameStateOrder = new List<GameState>
        {
            GameState.START,
            GameState.GREEN_SETUP,
            GameState.GREEN_HIT,
            GameState.RED_SETUP,
            GameState.RED_HIT,
            GameState.BLUE_SETUP,
            GameState.BLUE_HIT,
            GameState.SUCCESS
        };

        SetupLevel(2, 3);

        waypoints = new() {
            new Vector2Int(gridSizeX - 1, gridSizeY - 2),
            // TODO this one should be tiny so it can't be seen unless you rotate
            new Vector2Int(gridSizeX - 2, gridSizeY - 1),
            new Vector2Int(squareOne.x, squareOne.y),
        };

        SpawnFirstWaypoint();

        obstacles = new();
        for (int ndx = 0; ndx < gridSizeY; ndx++)
        {
            obstacles.Add(gridController.AddMovingObstacleAtPosition(1, ndx));
        }

        obstacles[^1].SetAfterRollAction((_, _) => AfterObjectMoves());

        currentGameState = GameState.START;
    }

    void Update()
    {
        SetMoveCountText();
        if (levelActive)
        {
            ManageGameState();
        }
    }
#pragma warning restore IDE0051

    void SpawnFirstWaypoint()
    {
        gridController.SpawnWaypoint(waypoints[0], () => MoveObstaclesThenSpawnSecondWaypoint());

        void MoveObstaclesThenSpawnSecondWaypoint()
        {
            int desiredObjectMoveCount = timesWallMoved + 2;
            StartCoroutine(WaitForObjectToMove(desiredObjectMoveCount, () => MoveObstacles(Vector3.right), () => SpawnSecondWaypoint()));
        }
    }


    /**
    Allow wall to move until it hits the desired number of times moved
    */
    IEnumerator WaitForObjectToMove(int desiredAfterMoveCount, Action doMove, Action afterObjectMoveAction)
    {
        Debug.LogFormat("I want to get to {0} but I'm only at {1}", desiredAfterMoveCount, timesWallMoved);
        doMove?.Invoke();
        int currentTimesWallMoved = timesWallMoved;
        // this loop and the break condition are kinda weird but they seem to work
        while (timesWallMoved != desiredAfterMoveCount)
        {
            if (currentTimesWallMoved != timesWallMoved)
            {
                Debug.Log("The wall has moved once according to the waiter");
                // let it progress
                currentTimesWallMoved = timesWallMoved;

                if (currentTimesWallMoved == desiredAfterMoveCount)
                {
                    break;
                }

                doMove?.Invoke();
            }
            yield return null;
        }
        TransitionState();
        afterObjectMoveAction?.Invoke();
    }

    void SpawnSecondWaypoint()
    {
        gridController.SpawnWaypoint(waypoints[1], () => SpawnLastWaypoint());
    }

    void SpawnLastWaypoint()
    {
        gridController.SpawnWaypoint(waypoints[2], () => Debug.Log("player has won according to custom waypoint manager"));
    }


    void AfterObjectMoves()
    {
        Debug.Log("Object has moved");
        timesWallMoved++;
    }

    override protected void OnPlayerMoveFinish(Vector2Int playerPosition)
    {
        if (levelActive)
        {
            turnsLeft = turnLimit - playerController.GetMoveCount();
        }
    }

    void ManageGameState()
    {
        Vector2Int playerPos = playerController.GetCurrentPosition();

        // allow devMode to not fall out of map
        if (!gridController.IsWithinGrid(playerPos))
        {
            Debug.Log("Player has exited map.");
            currentGameState = GameState.FAILED;
        }
        switch (currentGameState)
        {
            case GameState.START:
                TransitionState();
                break;
            case GameState.GREEN_SETUP:
                gridController.PaintTileAtLocation(waypoints[0].x, waypoints[0].y, Color.green);
                TransitionState();
                break;
            case GameState.GREEN_HIT:
                break;
            case GameState.RED_SETUP:
                // if (!redSetupStarted)
                // {
                //     MoveObstacles(Vector3.right);
                //     redSetupStarted = true;
                // }
                // if (redSetupStarted && wallHasMoved)
                // {
                //     wallHasMoved = false;
                gridController.PaintTileAtLocation(waypoints[1].x, waypoints[1].y, Color.red);
                gridController.PaintTileAtLocation(waypoints[2].x, waypoints[2].y, Color.blue);
                TransitionState();
                // }
                break;
            case GameState.RED_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.red)
                {
                    gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 2, OnIceTileSteppedOn);
                    gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 3, OnIceTileSteppedOn);
                    gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 4, OnIceTileSteppedOn);
                    TransitionState();
                }
                break;
            case GameState.BLUE_SETUP:
                // last step is back to square one
                TransitionState();
                break;
            case GameState.BLUE_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.blue)
                {
                    TransitionState();
                    // you must manage game state here before falling through, otherwise you could be transitioning
                    // into a success game state when you're at zero turns!
                    ManageGameState();
                }
                break;
            case GameState.SUCCESS:
                Debug.Log("Player has won!");
                SetTerminalGameState(successElements);
                break;
            case GameState.FAILED:
                Debug.Log("Player has failed.");
                SetTerminalGameState(failedElements);
                break;
            default:
                Debug.LogErrorFormat("Encountered unexpected game state: {0}", currentGameState);
                break;
        }

        if (turnsLeft <= 0)
        {
            Debug.Log("Player exceeded move count");
            currentGameState = GameState.FAILED;
        }

    }

    void TransitionState()
    {
        // could probably use a better data structure as the state machine that allows a failure state as defined by the state machine
        currentGameState = gameStateOrder[gameStateOrder.IndexOf(currentGameState) + 1];
    }

    void MoveObstacles(Vector3 direction)
    {
        foreach (MovingObstacle obstacle in obstacles)
        {
            // TODO this one really isn't a moving obstacle?
            if (Mathf.RoundToInt(obstacle.transform.position.z) == 0) continue;

            obstacle.MoveInDirectionIfNotMovingAndDontEnqueue(direction);
        }
    }
}