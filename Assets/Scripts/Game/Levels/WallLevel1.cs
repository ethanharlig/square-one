using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// introduces player to camera movement, uses ice
public class WallLevel1 : LevelManager
{
    private List<MovingObstacle> obstacles;
    int timesWallMoved = 0;

#pragma warning disable IDE0051
    void Start()
    {
        gridSizeX = gridSizeY = 6;
        turnLimit = 13;

        SetupLevel(2, 3);

        Waypoint[] waypointPositionsInOrder = new[] {
            Waypoint.Of(gridSizeX - 1, gridSizeY - 2)
            .WithOnTriggeredAction(() => {
                int desiredObjectMoveCount = timesWallMoved + 2;
                StartCoroutine(WaitForObjectToMove(desiredObjectMoveCount, () => MoveObstacles(Vector3.right), () => {
                    gsm.SpawnNextWaypoint();
                    gsm.SpawnNextWaypoint();
                }));
            }),
            Waypoint.Of(gridSizeX - 2, gridSizeY - 1)
            .WithOnTriggeredAction(() => {
                gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 2, OnIceTileSteppedOn);
                gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 3, OnIceTileSteppedOn);
                gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 4, OnIceTileSteppedOn);
            })
            // TODO wanna make this smaller so we it's hidden before you have to rotate right
            .WithOptions(Waypoint.WaypointOptions.Of(0.2f, Color.cyan)),
            Waypoint.Of(squareOne.x, squareOne.y)
        };

        gsm.SetWaypoints(waypointPositionsInOrder);
        gsm.SetTurnLimit(turnLimit);

        gsm.ManageGameState();

        // TODO add more ice that's visible before rotate, and another waypoint that makes player go further

        obstacles = new();
        for (int ndx = 0; ndx < gridSizeY; ndx++)
        {
            obstacles.Add(gridController.AddMovingObstacleAtPosition(1, ndx));
        }

        obstacles[^1].SetAfterRollAction((_, _, _) => AfterObjectMoves());
    }

#pragma warning restore IDE0051

    /**
    Allow wall to move until it hits the desired number of times moved
    */
    IEnumerator WaitForObjectToMove(int desiredAfterMoveCount, Action doMove, Action afterObjectMoveAction)
    {
        doMove?.Invoke();
        int currentTimesWallMoved = timesWallMoved;
        // this loop and the break condition are kinda weird but they seem to work
        while (timesWallMoved != desiredAfterMoveCount)
        {
            if (currentTimesWallMoved != timesWallMoved)
            {
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
        timesWallMoved++;
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