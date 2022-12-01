using TMPro;
using UnityEngine;

// introduces player to game controls
public class IntroLevel1 : LevelManager
{
    [SerializeField] GameObject helpElement;
    private TextMeshProUGUI helpText;

    void Start()
    {
        gridSizeX = gridSizeY = 7;
        // turnLimit = 30;

        SetupLevel();

        helpText = helpElement.GetComponentInChildren<TextMeshProUGUI>();

        Waypoint[] waypointPositionsInOrder = new[] {
            Waypoint.Of(playerController.GetCurrentPosition().x, playerController.GetCurrentPosition().y)
            .WithOnTriggeredAction(() => {
                helpText.text = "Move with WASD or arrow keys";
                helpElement.SetActive(true);
                gsm.SpawnNextWaypoint();
            }),
            Waypoint.Of(gridSizeX - 2, 0)
            .WithOnTriggeredAction(() => {
                helpElement.SetActive(false);
                gsm.SpawnNextWaypoint();
            }),
            Waypoint.Of(2, gridSizeY - 1),
            Waypoint.Of(1, 2)
            .WithOnTriggeredAction(() => {
                helpText.text = "Rotate the camera with Q/E\nSome waypoints can be hard to find";
                helpElement.SetActive(true);
                gsm.SpawnNextWaypoint();
            }),
            Waypoint.Of(gridSizeX - 1, gridSizeY - 2).WithOptions(Waypoint.WaypointOptions.Of(0.15f))
            .WithOnTriggeredAction(() => {
                helpElement.SetActive(false);
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
}