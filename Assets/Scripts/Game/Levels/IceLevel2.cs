using System.Collections.Generic;
using UnityEngine;

// full ice level with distractions
public class IceLevel2 : LevelManager
{

#pragma warning disable IDE0051
    void Start()
    {
        gridSizeX = gridSizeY = 10;
        turnLimit = 10;

        SetupLevel(5, 5);

        Waypoint[] waypointsInOrder = new[] {
            Waypoint.Of(gridSizeX - 2, 1),
            Waypoint.Of(1, 4),
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

        // useless ones added for extra challenge
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 2, -1);
        gridController.AddStationaryObstacleAtPosition(2, 1);
        gridController.AddStationaryObstacleAtPosition(gridSizeX, 4);
        gridController.AddStationaryObstacleAtPosition(gridSizeX, 5);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 1, gridSizeY);
        gridController.AddStationaryObstacleAtPosition(-1, gridSizeY - 1);
    }

#pragma warning restore IDE0051

    override protected void OnPlayerMoveFullyCompleted(Vector2Int playerPosition, bool shouldCountMove)
    {
        if (shouldCountMove)
        {
            turnsLeft = turnLimit - playerController.GetMoveCount();
        }
    }
}