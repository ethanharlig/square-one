using System.Collections.Generic;
using UnityEngine;

// full ice level with two following obstacles
public class IceChaseLevel3 : LevelManager
{

#pragma warning disable IDE0051
    void Start()
    {
        gridSizeX = gridSizeY = 10;
        turnLimit = 20;

        SetupLevel(5, 5);

        // TODO should rotate camera?
        // TODO should rely on moving the obstacle around so you can hit it as a stopper

        Vector2Int[] waypointsInOrder = new[] {
            new Vector2Int(gridSizeX - 2, 1),
            new Vector2Int(1, 4),
            new Vector2Int(squareOne.x, squareOne.y),
        };


        gsm.SetWaypoints(waypointsInOrder, true);
        gsm.SetTurnLimit(turnLimit);
        gsm.ManageGameState();

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                gridController.SpawnIceTile(x, y, OnIceTileSteppedOn);
            }
        }

        // TODO this is copied from IceLevel2, think of new puzzle

        // player is at 5, 5 so this is hittable
        gridController.AddStationaryObstacleAtPosition(5, 0);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 1, 1);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 2, 6);
        gridController.AddStationaryObstacleAtPosition(2, 5);
        // allows player to hit red
        gridController.AddStationaryObstacleAtPosition(3, 3);
        gridController.AddStationaryObstacleAtPosition(0, 4);
        gridController.AddStationaryObstacleAtPosition(1, gridSizeY);
        // allows player to hit blue
        gridController.AddStationaryObstacleAtPosition(squareOne.x + 1, gridSizeY - 1);

        gridController.AddStationaryObstacleAtPosition(gridSizeX - 2, -1);
        gridController.AddStationaryObstacleAtPosition(-1, gridSizeY - 2);
        gridController.AddStationaryObstacleAtPosition(1, -1);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 1, 2);

        MovingObstacle lowerQuadrantFollower = gridController.AddMovingObstacleAtPosition(0, 0);
        lowerQuadrantFollower.MoveTowardsPlayer(playerController, gridController.GetCurrentStationaryObstaclesAction(), false);

        MovingObstacle upperQuadrantFollower = gridController.AddMovingObstacleAtPosition(gridSizeX - 1, gridSizeY - 3);
        upperQuadrantFollower.MoveTowardsPlayer(playerController, gridController.GetCurrentStationaryObstaclesAction(), false);
    }

#pragma warning restore IDE0051

    override protected void OnPlayerMoveFinishWithShouldCountMove(Vector2Int playerPosition, bool shouldCountMove)
    {
        if (shouldCountMove)
        {
            turnsLeft = turnLimit - playerController.GetMoveCount();
        }
    }
}