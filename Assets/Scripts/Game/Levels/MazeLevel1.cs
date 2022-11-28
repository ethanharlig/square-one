using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// basic maze, maze level 2 can start moving walls on waypoint hit
public class MazeLevel1 : LevelManager
{
    private List<ObstacleController> obstacles;

#pragma warning disable IDE0051
    void Start()
#pragma warning restore IDE0051
    {
        gridSizeX = gridSizeY = 12;
        turnLimit = 60;

        SetupLevel(2, 4);

        Vector2Int[] waypointsInOrder = new[] {
            new Vector2Int(10, 7),
            new Vector2Int(9, 3),
            new Vector2Int(3, 10),
            new Vector2Int(squareOne.x, squareOne.y)
        };

        gsm.SetWaypoints(waypointsInOrder, true);
        gsm.SetTurnLimit(turnLimit);

        gsm.ManageGameState();

        HashSet<Vector2Int> path = new() {
            new Vector2Int(1, 1),
            new Vector2Int(1, 2),
            new Vector2Int(1, 3),
            new Vector2Int(1, 4),
            new Vector2Int(1, 5),
            new Vector2Int(1, 6),
            new Vector2Int(1, 7),
            new Vector2Int(1, 8),
            new Vector2Int(1, 9),
            new Vector2Int(1, 10),

            new Vector2Int(2, 1),
            new Vector2Int(2, 4),
            new Vector2Int(2, 7),
            new Vector2Int(2, 10),

            new Vector2Int(3, 1),
            new Vector2Int(3, 2),
            new Vector2Int(3, 4),
            new Vector2Int(3, 5),
            new Vector2Int(3, 6),
            new Vector2Int(3, 7),
            new Vector2Int(3, 10),

            new Vector2Int(4, 2),
            new Vector2Int(4, 3),
            new Vector2Int(4, 4),
            new Vector2Int(4, 6),
            new Vector2Int(4, 10),

            new Vector2Int(5, 1),
            new Vector2Int(5, 2),
            new Vector2Int(5, 4),
            new Vector2Int(5, 5),
            new Vector2Int(5, 6),
            new Vector2Int(5, 7),
            new Vector2Int(5, 8),
            new Vector2Int(5, 10),

            new Vector2Int(6, 1),
            new Vector2Int(6, 4),
            new Vector2Int(6, 6),
            new Vector2Int(6, 8),
            new Vector2Int(6, 9),
            new Vector2Int(6, 10),

            new Vector2Int(7, 1),
            new Vector2Int(7, 3),
            new Vector2Int(7, 4),
            new Vector2Int(7, 6),
            new Vector2Int(7, 9),

            new Vector2Int(8, 1),
            new Vector2Int(8, 2),
            new Vector2Int(8, 3),
            new Vector2Int(8, 6),
            new Vector2Int(8, 8),
            new Vector2Int(8, 9),
            new Vector2Int(8, 10),

            new Vector2Int(9, 1),
            new Vector2Int(9, 3),
            new Vector2Int(9, 5),
            new Vector2Int(9, 6),
            new Vector2Int(9, 7),
            new Vector2Int(9, 8),
            new Vector2Int(9, 10),

            new Vector2Int(10, 1),
            new Vector2Int(10, 2),
            new Vector2Int(10, 3),
            new Vector2Int(10, 4),
            new Vector2Int(10, 5),
            new Vector2Int(10, 7),
            new Vector2Int(10, 8),
            new Vector2Int(10, 9),
            new Vector2Int(10, 10),
        };

        obstacles = new();
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2Int pos = new(x, y);
                if (!path.Contains(pos))
                {
                    if (Array.IndexOf(waypointsInOrder, pos) >= 0)
                    {
                        Debug.LogErrorFormat("Tried to set a waypoint but cover it up with obstacle {0}", pos);
                    }
                    obstacles.Add(gridController.AddStationaryObstacleAtPosition(x, y));
                }
            }
        }

        gridController.SpawnIceTile(5, 6, OnIceTileSteppedOn);

        MovingObstacle bottomRightQuadrantFollower = gridController.AddMovingObstacleAtPosition(10, 1);
        bottomRightQuadrantFollower.MoveTowardsPlayer(playerController, gridController.GetCurrentStationaryObstaclesAction(), false);

        // TODO not sure this is winnable
        // MovingObstacle upperLeftQuadrantFollower = gridController.AddMovingObstacleAtPosition(1, 9);
        // upperLeftQuadrantFollower.MoveTowardsPlayer(playerController, gridController.GetCurrentStationaryObstaclesAction(), false);
    }


    override protected void OnPlayerMoveFinishWithShouldCountMove(Vector2Int playerPosition, bool shouldCountMove)
    {
        if (shouldCountMove)
        {
            turnsLeft = turnLimit - playerController.GetMoveCount();
        }
    }

}