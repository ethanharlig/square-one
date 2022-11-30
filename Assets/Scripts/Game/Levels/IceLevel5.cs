using System;
using System.Collections;
using UnityEngine;

// full ice level with certain waypoints moving obstacles
public class IceLevel5 : LevelManager
{

#pragma warning disable IDE0051
    void Start()
    {
        gridSizeX = gridSizeY = 9;
        turnLimit = 20;

        SetupLevel();

        MovingObstacle moving = gridController.AddMovingObstacleAtPosition(1, 4);

        // TODO should add mechanic where hitting a waypoint moves an obstacle 
        Waypoint[] waypointsInOrder = new[] {
            Waypoint.Of(gridSizeX - 1, gridSizeY - 1),
            Waypoint.Of(4, gridSizeY - 5).WithOnTriggeredAction(() => {
                StartCoroutine(WaitForObjectToMove(moving, Vector3.left, () => gsm.SpawnNextWaypoint()));
            }),
            Waypoint.Of(0, gridSizeY - 1),
            Waypoint.Of(gridSizeX - 1, gridSizeY - 1),
            Waypoint.Of(squareOne.x + 1, squareOne.y + 2),
            Waypoint.Of(squareOne.x, squareOne.y),
        };


        gsm.SetWaypoints(waypointsInOrder);
        gsm.SetTurnLimit(turnLimit);
        gsm.ManageGameState();

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                gridController.SpawnIceTile(x, y, OnIceTileSteppedOn);
            }
        }

        gridController.AddStationaryObstacleAtPosition(playerController.GetCurrentPosition().x, gridSizeY);

        gridController.AddStationaryObstacleAtPosition(gridSizeX, gridSizeY - 1);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 1, gridSizeY);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 2, gridSizeY - 2);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 3, gridSizeY - 2);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 2, gridSizeY - 3);

        gridController.AddStationaryObstacleAtPosition(-1, 0);
        gridController.AddStationaryObstacleAtPosition(0, -1);

        gridController.AddStationaryObstacleAtPosition(1, 2);
        gridController.AddStationaryObstacleAtPosition(1, 1);
        gridController.AddStationaryObstacleAtPosition(2, 1);
        gridController.AddStationaryObstacleAtPosition(1, 2);

        gridController.AddStationaryObstacleAtPosition(gridSizeX - 1, 4);
        gridController.AddStationaryObstacleAtPosition(3, 5);
        gridController.AddStationaryObstacleAtPosition(4, -1);
        gridController.AddStationaryObstacleAtPosition(-1, 0);
        gridController.AddStationaryObstacleAtPosition(0, gridSizeY);
        gridController.AddStationaryObstacleAtPosition(-1, gridSizeY - 1);

        gridController.AddStationaryObstacleAtPosition(7, 0);
        gridController.AddStationaryObstacleAtPosition(1, 6);
        gridController.AddStationaryObstacleAtPosition(5, 2);
        gridController.AddStationaryObstacleAtPosition(2, 7);
        gridController.AddStationaryObstacleAtPosition(1, 7);

        // TODO this is moving through stuff
        // MovingObstacle follower = gridController.AddMovingObstacleAtPosition(2, 6);
        // follower.StartPatrolling(new Vector2Int(6, 8), gridController.GetCurrentStationaryObstaclesAction());
    }

#pragma warning restore IDE0051

    override protected void OnPlayerMoveFullyCompleted(Vector2Int playerPosition, bool shouldCountMove)
    {
        if (shouldCountMove)
        {
            turnsLeft = turnLimit - playerController.GetMoveCount();
        }
    }

    IEnumerator WaitForObjectToMove(MovingObstacle obstacle, Vector3 dir, Action afterObjectMoveAction)
    {
        bool isFinished = false;
        obstacle.SetAfterRollAction((_, _, _) => isFinished = true);
        obstacle.MoveInDirectionIfNotMovingAndDontEnqueue(dir);

        while (!isFinished)
        {
            yield return null;
        }
        afterObjectMoveAction?.Invoke();
    }
}