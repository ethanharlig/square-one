using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelManager : MonoBehaviour
{
    protected GridController gridController;
    protected PlayerController playerController;
    protected CameraController cameraController;
    protected LevelUIElements levelUIElements;

    protected int gridSizeX, gridSizeY, turnLimit, turnsLeft;

    protected Vector2Int squareOne;
    protected bool levelActive;

    protected GameStateManager gsm;

    protected void SetupLevel()
    {
        int playerOffsetX = gridSizeX / 2;
        int playerOffsetY = gridSizeY / 2;

        SetupLevel(playerOffsetX, playerOffsetY);
    }

    protected void SetupLevel(int playerOffsetX, int playerOffsetY)
    {
        playerController = (PlayerController)PlayerController.Instance;
        gridController = (GridController)GridController.Instance;
        cameraController = (CameraController)CameraController.Instance;
        levelUIElements = (LevelUIElements)LevelUIElements.Instance;

        gridController.SetupGrid(gridSizeX, gridSizeY);

        playerController.SpawnPlayer(playerOffsetX, playerOffsetY, (x, y) => gridController.TileWillMovePlayer(x, y));
        playerController.gameObject.SetActive(true);

        cameraController.CenterCameraOnOffset(gridSizeX / 2, gridSizeY / 2);

        squareOne = new(playerOffsetX, playerOffsetY);

        turnsLeft = turnLimit;

        SetMoveCountText();

        levelActive = true;

        gsm = new GameStateManager(playerController, gridController);

        gsm.OnStateChange += OnStageChange;
    }

    /**
    handles setting the game to SUCCESS or FAILED
    ideas included below
    */
    // set back to square one text
    // stop input from this script, now we should spawn a NextGamePortal and head there
    // also spawn a plane below you which can reset you into middle of map if you fall off at this point
    protected void SetTerminalGameState(GameObject textElementToEnable)
    {
        SetTerminalGameState(textElementToEnable, 0.2f);
    }

    /**
    handles setting the game to SUCCESS or FAILED with a variable waitDelaySeconds
    */
    protected void SetTerminalGameState(GameObject textElementToEnable, float waitDelaySeconds)
    {
        levelActive = false;
        playerController.EnterTerminalGameState();

        StartCoroutine(SetElementAfterDelay(textElementToEnable, waitDelaySeconds));

        static IEnumerator SetElementAfterDelay(GameObject element, float waitDelaySeconds)
        {
            yield return new WaitForSeconds(waitDelaySeconds);
            element.SetActive(true);
        }
    }

    protected void SetMoveCountText()
    {
        if (gsm != null && gsm.TurnLimitEnabled)
        {
            levelUIElements.EnableMoveCountText();
            levelUIElements.SetMoveCountText($"Turns remaining: {turnsLeft}");
        }
    }

    // handle player movement. override in child classes if they want to access these events
    // prefer to use OnPlayerMoveStart unless you need specific behavior at the end of the movement
    protected virtual void OnPlayerMoveStart(Vector2Int playerPositionBeforeMove) { }
    protected virtual void OnPlayerMoveFinish(Vector2Int playerPositionAfterMove) { }

    protected virtual void OnPlayerMoveFullyCompleted(Vector2Int playerPositionAfterMove, bool shouldCountMove) { }

    // using this doesn't guarantee that a move has finished, so you get no access to know if the move should count
    private void OnPlayerMoveFinish(Vector2Int playerPositionAfterMove, bool shouldCountMove)
    {
        if (shouldCountMove)
        {
            AudioController.Instance.PlayMoveAudio();

        }
        OnPlayerMoveFinish(playerPositionAfterMove);
        // TODO should this be removed?? is this getting called twice?
        // OnPlayerMoveFullyCompleted(playerPositionAfterMove, shouldCountMove);
    }

#pragma warning disable IDE0051
    // must be done at object enable time
    void OnEnable()
    {
        Debug.Log("Enabling player event");
        PlayerController.OnMoveStart += OnPlayerMoveStart;
        PlayerController.OnSingleMoveFinish += OnPlayerMoveFinish;
        PlayerController.OnMoveFullyCompleted += OnPlayerMoveFullyCompleted;
        // PlayerController.OnMoveFullyCompleted += OnPlayerMoveFullyCompleted;
    }

    // make sure to deregister at disable time
    void OnDisable()
    {
        Debug.Log("Disabling player event");
        PlayerController.OnMoveStart -= OnPlayerMoveStart;
        PlayerController.OnSingleMoveFinish -= OnPlayerMoveFinish;
        PlayerController.OnMoveFullyCompleted -= OnPlayerMoveFullyCompleted;
        // PlayerController.OnMoveFinish -= OnPlayerMoveFullyCompleted;

        gsm.OnStateChange -= OnStageChange;
    }
    void Update()
    {
        SetMoveCountText();
        gsm.CheckPlayerState();
    }
#pragma warning restore IDE0051

    // TODO TODO TODO bug bug bug
    // TODO if player needs to step on multiple ice tiles which result in 0 moves remaining but a victory,
    // they actually only traverse the first tile before the game state is checked. need to rethink
    protected void OnIceTileSteppedOn(Vector3Int direction)
    {
        // TODO ice should work when level inactive?
        if (levelActive)
        {
            playerController.ForceMoveInDirection(direction);
        }
    }

    protected void OnStageChange(GameStateManager.GameState state)
    {
        if (state == GameStateManager.GameState.FAILED)
        {
            SetTerminalGameState(levelUIElements.GetFailedElements());
            AudioController.Instance.PlayLoseAudio();

        }
        else if (state == GameStateManager.GameState.SUCCESS)
        {
            SetTerminalGameState(levelUIElements.GetSuccessElements());
            AudioController.Instance.PlayWinAudio();

        }
    }
}
