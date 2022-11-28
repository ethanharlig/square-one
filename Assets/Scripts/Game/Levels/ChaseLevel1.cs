using System.Collections.Generic;
using UnityEngine;

// simple chase level that introduces player to the concept 
public class ChaseLevel1 : LevelManager
{

    private List<MovingObstacle> obstacles;

#pragma warning disable IDE0051
    void Start()
    {
#pragma warning restore IDE0051
        gridSizeX = gridSizeY = 11;
        turnLimit = 70;

        SetupLevel(gridSizeX / 2 + 4, gridSizeY / 2);

        Vector2Int[] waypointsInOrder = new[] {
            new Vector2Int(9, 9),
            new Vector2Int(0, 0),
            new Vector2Int(4, gridSizeY - 1),
            new Vector2Int(squareOne.x, squareOne.y)
        };

        gsm.SetWaypoints(waypointsInOrder, true);
        gsm.SetTurnLimit(turnLimit);

        gsm.ManageGameState();

        obstacles = new List<MovingObstacle>();

        ObstacleController stationaryObstacle = gridController.AddStationaryObstacleAtPosition(4, 4);

        MovingObstacle obstacle = gridController.AddMovingObstacleAtPosition(2, 1);
        obstacle.MoveTowardsPlayer(playerController, gridController.GetCurrentStationaryObstaclesAction(), false);
        gridController.PaintTileAtLocation(new Vector2Int(1, gridSizeY - 2), Color.white);

        // TODO I want to add ice but not sure how to not make it too easy
        // for (int x = 0; x < gridSizeX; x++)
        // {
        //     if (x == 0 || x == gridSizeX - 1)
        //     {
        //         for (int y = 2; y < gridSizeY - 2; y++)
        //         {
        //             gridController.SpawnIceTile(x, y, OnIceTileSteppedOn);
        //         }
        //     }
        // }

        // for (int y = 0; y < gridSizeY; y++)
        // {
        //     if (y == 0 || y == gridSizeY - 1)
        //     {
        //         for (int x = 2; x < gridSizeY - 2; x++)
        //         {
        //             gridController.SpawnIceTile(x, y, OnIceTileSteppedOn);
        //         }
        //     }
        // }


        obstacles.Add(obstacle);
    }

    override protected void OnPlayerMoveFullyCompleted(Vector2Int playerPosition, bool shouldCountMove)
    {
        // white tile color disables all moving obstacles
        if (gridController.TileColorAtLocation(playerPosition) == Color.white)
        {
            Debug.Log("Stopping obstacle movement!");
            foreach (MovingObstacle obstacle in obstacles)
            {
                obstacle.StopMovement();
            }
        }

        if (shouldCountMove)
        {
            turnsLeft = turnLimit - playerController.GetMoveCount();
        }
    }
}
