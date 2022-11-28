using System.Collections.Generic;
using UnityEngine;

// simple level which introduces player to ice
public class IceLevel1 : LevelManager
{
#pragma warning disable IDE0051
    void Start()
    {
        gridSizeX = gridSizeY = 6;
        turnLimit = 11;

        SetupLevel(5, 4);

        Vector2Int[] waypointsInOrder = new[] {
            new Vector2Int(gridSizeX - 1, 2),
            new Vector2Int(0, 0),
            new Vector2Int(squareOne.x, squareOne.y),
        };


        gsm.SetWaypoints(waypointsInOrder, true);
        gsm.SetTurnLimit(turnLimit);
        gsm.ManageGameState();

        gridController.SpawnIceTilesAroundPosition(waypointsInOrder[0].x, waypointsInOrder[0].y, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, 4, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, 3, OnIceTileSteppedOn);
        gridController.SpawnIceTile(1, 5, OnIceTileSteppedOn);
        gridController.SpawnIceTile(2, 0, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, 0, OnIceTileSteppedOn);
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