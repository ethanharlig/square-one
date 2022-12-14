using System;
using System.Collections;
using TMPro;
using UnityEngine;

// introduces player to game controls
public class IntroLevel1 : LevelManager
{
    [SerializeField] GameObject helpElement;
    private TextMeshProUGUI helpText;

    void Start()
    {
        gridSizeX = gridSizeY = 8;

        SetupLevel();

        helpText = helpElement.GetComponentInChildren<TextMeshProUGUI>();

        Waypoint[] waypointPositionsInOrder = new[] {
            Waypoint.Of(playerController.GetCurrentPosition().x, playerController.GetCurrentPosition().y)
            .WithOptions(Waypoint.WaypointOptions.Of().WithEnableAudio(false))
            .WithOnTriggeredAction(() => {
                helpText.text = "Move with WASD or arrow keys";
                helpElement.SetActive(true);
                StartCoroutine(WaitForNumberOfMoves(3, () => {
                    helpText.text = "Your objective is to get\nto all of the waypoints";
                    gsm.SpawnNextWaypoint();
                }));
            }),
            Waypoint.Of(1, 1)
            .WithOnTriggeredAction(() => {
                gsm.SpawnNextWaypoint();
            }),
            Waypoint.Of(gridSizeX - 2, 0)
            .WithOnTriggeredAction(() => {
                helpText.text = "Rotate the camera with Q/E\n\nThe compass in the bottom left shows your orientation";
                gsm.SpawnNextWaypoint();
            }),
            Waypoint.Of(2, gridSizeY - 1),
            Waypoint.Of(gridSizeX - 4, 3)
            .WithOnTriggeredAction(() => {
                helpText.text = "You can turn the compass off\nin the pause menu (esc)";
                gsm.SpawnNextWaypoint();
            }),
            Waypoint.Of(1, 2)
            .WithOnTriggeredAction(() => {
                helpText.text = "Some waypoints can be hard to find!";
                gsm.SpawnNextWaypoint();
            }),
            Waypoint.Of(gridSizeX - 1, gridSizeY - 2).WithOptions(Waypoint.WaypointOptions.Of(0.15f))
            .WithOnTriggeredAction(() => {
                helpText.text = "Levels have a turn limit\ndisplayed in the top left";
                SetTurnLimit(30);
                gsm.SpawnNextWaypoint();
            }),
            Waypoint.Of(3, gridSizeY - 4)
            .WithOnTriggeredAction(() => {
                helpText.text = "Watch out for slippery ice\nand obstacles!";

                gridController.AddMovingObstacleAtPosition(gridSizeX - 2, gridSizeY - 1).MoveTowardsPlayer(playerController, gridController.GetCurrentStationaryObstaclesAction(), false);

                gridController.SpawnIceTile(gridSizeX - 2, 4, OnIceTileSteppedOn);
                gridController.SpawnIceTile(gridSizeX - 3, 4, OnIceTileSteppedOn);
                gridController.SpawnIceTile(gridSizeX - 3, 3, OnIceTileSteppedOn);

                gridController.SpawnIceTile(gridSizeX - 4, gridSizeY - 2, OnIceTileSteppedOn);
                gridController.SpawnIceTile(gridSizeX - 4, gridSizeY - 3, OnIceTileSteppedOn);

                gsm.SpawnNextWaypoint();
            }),
            Waypoint.Of(gridSizeX - 2, 2)
            .WithOnTriggeredAction(() => {
                helpElement.SetActive(true);
                gsm.SpawnNextWaypoint();
            }),
            Waypoint.Of(squareOne.x, squareOne.y)
        };

        gsm.SetWaypoints(waypointPositionsInOrder);

        gsm.ManageGameState();

        gridController.AddStationaryObstacleAtPosition(gridSizeX, gridSizeY - 1);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 1, gridSizeY);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 2, gridSizeY - 2);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 3, gridSizeY - 2);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 2, gridSizeY - 3);
    }

    IEnumerator WaitForNumberOfMoves(int movesToWait, Action afterWaitAction)
    {
        int desiredMoveCount = playerController.GetMoveCount() + movesToWait;
        yield return new WaitWhile(() => playerController.GetMoveCount() < desiredMoveCount);
        afterWaitAction?.Invoke();
    }
}