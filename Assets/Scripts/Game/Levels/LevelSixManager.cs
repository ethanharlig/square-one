using System.Collections.Generic;
using UnityEngine;

// moving obstacles in two dimensions
public class LevelSixManager : LevelManager
{
    private List<GameState> gameStateOrder;
    private GameState currentGameState;

    enum GameState
    {
        START,
        GREEN_SETUP,
        GREEN_HIT,
        RED_SETUP,
        RED_HIT,
        BLUE_SETUP,
        BLUE_HIT,
        SUCCESS,
        FAILED,
    };

    void Start()
    {
        gridSizeX = gridSizeY = 11;
        turnLimit = 70;

        gameStateOrder = new List<GameState>
        {
            GameState.START,
            GameState.GREEN_SETUP,
            GameState.GREEN_HIT,
            GameState.RED_SETUP,
            GameState.RED_HIT,
            GameState.BLUE_SETUP,
            GameState.BLUE_HIT,
            GameState.SUCCESS
        };

        SetupLevel();

        ObstacleController obstacle = gridController.AddObstacleAtPosition(2, 2);
        obstacle.StartPatrolling(new Vector2Int(8, 0));

        ObstacleController obstacle2 = gridController.AddObstacleAtPosition(5, 7);
        obstacle2.StartPatrolling(new Vector2Int(8, 9));

        ObstacleController obstacle3 = gridController.AddObstacleAtPosition(1, 10);
        obstacle3.StartPatrolling(new Vector2Int(0, 1));

        currentGameState = GameState.START;
    }

    void Update()
    {
        SetMoveCountText();
        if (levelActive)
        {
            ManageGameState();
        }
    }

    override protected void OnPlayerMoveFinish(Vector2Int playerPosition)
    {
        if (levelActive)
        {
            turnsLeft--;
        }
    }

    void ManageGameState()
    {
        Vector2Int playerPos = playerController.GetCurrentPosition();

        // allow devMode to not fall out of map
        if (!DEV_MODE && !gridController.IsWithinGrid(playerPos))
        {
            Debug.Log("Player has exited map.");
            currentGameState = GameState.FAILED;
        }

        // game state handler
        switch (currentGameState)
        {
            case GameState.START:
                // secret location? kinda shitty to make player guess and check
                if (playerPos.x == 9 && playerPos.y == 9)
                {
                    TransitionState();
                }
                break;
            case GameState.GREEN_SETUP:
                gridController.PaintTileAtLocation(0, 0, Color.green);
                TransitionState();
                break;
            case GameState.GREEN_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.green)
                {
                    TransitionState();
                }
                break;
            case GameState.RED_SETUP:
                gridController.PaintTileAtLocation(4, gridSizeY - 1, Color.red);
                TransitionState();
                break;
            case GameState.RED_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.red)
                {
                    TransitionState();
                }
                break;
            case GameState.BLUE_SETUP:
                // last step is back to square one
                gridController.PaintTileAtLocation(squareOne.x, squareOne.y, Color.blue);
                TransitionState();
                break;
            case GameState.BLUE_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.blue)
                {
                    TransitionState();
                }
                break;
            case GameState.SUCCESS:
                Debug.Log("Player has won!");
                SetTerminalGameState(successElements);
                break;
            case GameState.FAILED:
                Debug.Log("Player has failed.");
                SetTerminalGameState(failedElements);
                break;
            default:
                Debug.LogErrorFormat("Encountered unexpected game state: {0}", currentGameState);
                break;
        }

        if (turnLimit <= 0)
        {
            Debug.Log("Player exceeded move count");
            currentGameState = GameState.FAILED;
        }

        void TransitionState()
        {
            // could probably use a better data structure as the state machine that allows a failure state as defined by the state machine
            currentGameState = gameStateOrder[gameStateOrder.IndexOf(currentGameState) + 1];
        }
    }
}
