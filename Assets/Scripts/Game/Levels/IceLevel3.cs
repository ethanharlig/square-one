using System.Collections.Generic;
using UnityEngine;

// full ice level with following obstacle
public class IceLevel3 : LevelManager
{

#pragma warning disable IDE0051
    void Start()
    {
        gridSizeX = gridSizeY = 8;
        turnLimit = 17;

        SetupLevel(1, 1);

        // TODO should rotate camera?
        // TODO should rely on moving the obstacle around so you can hit it as a stopper

        Waypoint[] waypointsInOrder = new[] {
            Waypoint.Of(gridSizeX - 2, 0),
            Waypoint.Of(4, gridSizeY - 5),
            Waypoint.Of(0, gridSizeY - 1),
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

        gridController.AddStationaryObstacleAtPosition(5, 0);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 1, 1);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 2, 6);
        gridController.AddStationaryObstacleAtPosition(2, 5);
        gridController.AddStationaryObstacleAtPosition(4, gridSizeY);
        gridController.AddStationaryObstacleAtPosition(3, 3);
        gridController.AddStationaryObstacleAtPosition(0, 4);
        gridController.AddStationaryObstacleAtPosition(1, gridSizeY);
        gridController.AddStationaryObstacleAtPosition(squareOne.x + 1, gridSizeY - 1);
        gridController.AddStationaryObstacleAtPosition(-1, gridSizeY - 1);

        gridController.AddStationaryObstacleAtPosition(gridSizeX - 2, -1);
        gridController.AddStationaryObstacleAtPosition(-1, gridSizeY - 2);
        gridController.AddStationaryObstacleAtPosition(1, -1);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 1, 2);
    }

#pragma warning restore IDE0051
}