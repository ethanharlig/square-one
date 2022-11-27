using System.Collections.Generic;
using UnityEngine;

// simple chase level that introduces player to the concept 
public class ChaseLevel1 : LevelManager
{

    private List<MovingObstacle> obstacles;

#pragma warning disable IDE0051
    void Start()
    {
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

        obstacles.Add(obstacle);
    }

#pragma warning restore IDE0051

    override protected void OnPlayerMoveFinishWithShouldCountMove(Vector2Int playerPosition, bool shouldCountMove)
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
