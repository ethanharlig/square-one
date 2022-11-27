using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// introduces player to camera movement, uses ice
public class WallLevelRefactor : LevelManager
{

    private List<MovingObstacle> obstacles;
    int timesWallMoved = 0;

#pragma warning disable IDE0051
    void Start()
    {
        gridSizeX = gridSizeY = 6;
        turnLimit = 13;

        SetupLevel(2, 3);

        (Vector2Int, Color)[] waypointsInOrder = new[] {
            (new Vector2Int(gridSizeX - 1, gridSizeY - 2), Color.green),
            (new Vector2Int(gridSizeX - 2, gridSizeY - 1), Color.red),
            (new Vector2Int(squareOne.x, squareOne.y), Color.blue)
        };

        // setup GSM and make sure to turn off autospawn so we can control
        gsm.SetWaypoints(waypointsInOrder, false);
        gsm.SetTurnLimit(turnLimit);
        gsm.OnWaypointHit += OnWaypointHit;

        gsm.ManageGameState();

        gsm.SpawnNextWaypoint();

        obstacles = new();
        for (int ndx = 0; ndx < gridSizeY; ndx++)
        {
            obstacles.Add(gridController.AddMovingObstacleAtPosition(1, ndx));
        }

        obstacles[^1].SetAfterRollAction((_, _) => AfterObjectMoves());
    }

#pragma warning restore IDE0051

    // must implement since we don't want the GSM to auto manage the waypoints
    void OnWaypointHit(int waypoint, Vector2Int pos, Color color)
    {
        if (waypoint == 0)
        {
            // if we hit the first waypoint, move the wall tiles
            int desiredObjectMoveCount = timesWallMoved + 2;
            StartCoroutine(WaitForObjectToMove(desiredObjectMoveCount, () => MoveObstacles(Vector3.right), () => gsm.SpawnNextWaypoint()));
        }
        else if (waypoint == 1)
        {
            gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 2, OnIceTileSteppedOn);
            gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 3, OnIceTileSteppedOn);
            gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 4, OnIceTileSteppedOn);
            gsm.SpawnNextWaypoint();
        }
        else
        {
            gsm.SpawnNextWaypoint();
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
        afterObjectMoveAction?.Invoke();
    }


    void AfterObjectMoves()
    {
        Debug.Log("Object has moved");
        timesWallMoved++;
    }

    override protected void OnPlayerMoveFinishWithShouldCountMove(Vector2Int playerPosition, bool shouldCountMove)
    {
        if (shouldCountMove)
        {
            turnsLeft = turnLimit - playerController.GetMoveCount();

        }
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