using System.Collections.Generic;
using UnityEngine;

// ice tiles allow player to glide
public class LevelSevenManager : LevelManager
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

#pragma warning disable IDE0051
    void Start()
    {
        gridSizeX = gridSizeY = 10;
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

        for (int ndx = 0; ndx < 5; ndx++)
        {
            int iceTileRow = 4 % gridSizeX;
            int iceTileCol = (3 + ndx) % gridSizeY;
            gridController.SpawnIceTile(iceTileRow, iceTileCol, OnIceTileSteppedOn);
        }

        for (int ndx = 0; ndx < 3; ndx++)
        {
            int iceTileRow = (gridSizeX - 3 - ndx) % gridSizeX;
            int iceTileCol = (gridSizeY - 3) % gridSizeY;
            gridController.SpawnIceTile(iceTileRow, iceTileCol, OnIceTileSteppedOn);
        }

        for (int ndx = 0; ndx < 2; ndx++)
        {
            int iceTileRow = (gridSizeX - 3 - ndx) % gridSizeX;
            int iceTileCol = (gridSizeY - 2) % gridSizeY;
            gridController.SpawnIceTile(iceTileRow, iceTileCol, OnIceTileSteppedOn);
        }

        gridController.AddStationaryObstacleAtPosition(4, 4);
        gridController.AddStationaryObstacleAtPosition(6, 7);

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
#pragma warning restore IDE0051

    override protected void OnPlayerMoveFinish(Vector2Int playerPosition)
    {
        if (levelActive)
        {
            turnsLeft = turnLimit - playerController.GetMoveCount();
        }
    }

    void ManageGameState()
    {
        Vector2Int playerPos = playerController.GetCurrentPosition();

        // allow devMode to not fall out of map
        if (!gridController.IsWithinGrid(playerPos))
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
                    // you must manage game state here before falling through, otherwise you could be transitioning
                    // into a success game state when you're at zero turns!
                    ManageGameState();
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

        if (currentGameState != GameState.SUCCESS && turnsLeft <= 0)
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
