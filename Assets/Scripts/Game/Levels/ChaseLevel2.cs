using UnityEngine;

// player has to complete tasks while keeping moving obstacle away
// TODO this level might be too borked for final game
public class ChaseLevel2 : LevelManager
{
#pragma warning disable IDE0051
    void Start()
    {
#pragma warning restore IDE0051
        gridSizeX = gridSizeY = 13;
        turnLimit = 90;

        SetupLevel(1, 1);

        for (int x = 0; x < gridSizeX; x++)
        {
            if (x > 3 && x < 7) continue;
            gridController.AddStationaryObstacleAtPosition(x, gridSizeY - 2);
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            if (x > 2 && x < 5 || x == gridSizeX - 1) continue;
            gridController.AddStationaryObstacleAtPosition(x, gridSizeY - 4);
        }

        for (int y = 2; y < 5; y++)
        {
            gridController.AddStationaryObstacleAtPosition(gridSizeX - 3, y);
        }

        gridController.AddStationaryObstacleAtPosition(2, 3);
        gridController.AddStationaryObstacleAtPosition(3, 3);
        gridController.AddStationaryObstacleAtPosition(3, 4);

        Waypoint[] waypointsInOrder = new[] {
            Waypoint.Of(gridSizeX - 1, gridSizeY - 5),
            Waypoint.Of(gridSizeX - 1, 2),
            Waypoint.Of(squareOne.x, squareOne.y),
        };

        gsm.SetWaypoints(waypointsInOrder);
        gsm.SetTurnLimit(turnLimit);

        gsm.ManageGameState();


        // TODO we need to figure out how to handle these obstacles colliding with one another. The undo is working but looks janky
        MovingObstacle followerUpperLeft = gridController.AddMovingObstacleAtPosition(2, gridSizeY - 1);
        followerUpperLeft.MoveTowardsPlayer(playerController, gridController.GetCurrentStationaryObstaclesAction());

        MovingObstacle followerLowerRight = gridController.AddMovingObstacleAtPosition(gridSizeX - 2, 3);
        followerLowerRight.MoveTowardsPlayer(playerController, gridController.GetCurrentStationaryObstaclesAction());

        // can maybe use disable button as bait?
        // gridController.PaintTileAtLocation(new Vector2Int(1, gridSizeY - 2), Color.white);
    }
}
