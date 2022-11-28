using UnityEngine;

// full ice level with hidden waypoints
public class IceLevel4 : LevelManager
{

#pragma warning disable IDE0051
    void Start()
    {
        gridSizeX = gridSizeY = 9;
        turnLimit = 20;

        SetupLevel();

        // TODO how to make waypoints smaller, that's the point of this level
        Vector2Int[] waypointsInOrder = new[] {
            new Vector2Int(gridSizeX - 1, gridSizeY - 1),
            new Vector2Int(4, gridSizeY - 5),
            new Vector2Int(0, gridSizeY - 1),
            new Vector2Int(gridSizeX - 1, gridSizeY - 1),
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

        MovingObstacle follower = gridController.AddMovingObstacleAtPosition(2, 6);
        follower.MoveTowardsPlayer(playerController, gridController.GetCurrentStationaryObstaclesAction(), false);
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