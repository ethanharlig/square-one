using System.Collections.Generic;
using UnityEngine;

public class LevelThreeManager : LevelManager
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

    /**
    Level one is a tile painting level. 
    When the player moves once, a square one unit away from them turns green.
    When the player touches the green square, we trigger an orange and red square towards the nearest corner.
    Touching either results in a blue square back at square one. Touching blue square wins game.
    */
    void Start()
    {
        // comment these out to use editor values
        gridSizeX = gridSizeY = 13;
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

        currentGameState = GameState.START;

        playerController.gameObject.SetActive(true);
    }

    void Update()
    {
        SetMoveCountText();
        if (levelActive)
        {
            ManageGameState();
        }
    }

    // required naming for events
    void OnEnable()
    {
        PlayerController.OnMoveStart += HandlePlayerMove;
    }

    // required naming for events
    void OnDisable()
    {
        PlayerController.OnMoveStart -= HandlePlayerMove;
    }

    void HandlePlayerMove()
    {
        if (levelActive)
        {
            turnsLeft--;
        }
    }

    void ManageGameState()
    {
        Vector2Int playerPos = playerController.GetRoundedPosition();

        // allow devMode to not fall out of map
        if (!devMode && !gridController.IsWithinGrid(playerPos))
        {
            Debug.Log("Player has exited map.");
            currentGameState = GameState.FAILED;
        }
        if (playerController.GetMoveCount() >= turnLimit)
        {
            Debug.Log("Player exceeded move count");
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

        void TransitionState()
        {
            // could probably use a better data structure as the state machine that allows a failure state as defined by the state machine
            currentGameState = gameStateOrder[gameStateOrder.IndexOf(currentGameState) + 1];
        }
    }
}
