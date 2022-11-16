using System.Collections;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] protected GridController gridController;
    [SerializeField] protected PlayerController playerController;
    [SerializeField] protected CameraController cameraController;

    [SerializeField] protected TextMeshProUGUI moveCountText;

    [SerializeField] protected GameObject successElements;
    [SerializeField] protected GameObject failedElements;
    [SerializeField] protected string nextLevelSceneString;

    [SerializeField] protected int gridSizeX, gridSizeY, turnLimit;
    [SerializeField] protected bool devMode = false;

    protected Vector2Int squareOne;
    protected bool levelActive;
    protected int turnsLeft;

    protected void SetupLevel()
    {
        playerController = (PlayerController)PlayerController.Instance;
        gridController = (GridController)GridController.Instance;
        cameraController = (CameraController)CameraController.Instance;

        gridController.SetupGrid(gridSizeX, gridSizeY);

        int playerOffsetX = gridSizeX / 2;
        int playerOffsetY = gridSizeY / 2;

        playerController.SpawnPlayer(playerOffsetX, playerOffsetY);
        playerController.gameObject.SetActive(true);

        cameraController.CenterCameraOnOffset(playerOffsetX, playerOffsetY);

        squareOne = new(playerOffsetX, playerOffsetY);

        turnsLeft = turnLimit;

        SetMoveCountText();

        levelActive = true;
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
        playerController.StopCountingMoves();
        StartCoroutine(SetElementAfterDelay(textElementToEnable, waitDelaySeconds));

        static IEnumerator SetElementAfterDelay(GameObject element, float waitDelaySeconds)
        {
            yield return new WaitForSeconds(waitDelaySeconds);
            element.SetActive(true);
        }
    }

    protected void SetMoveCountText()
    {
        moveCountText.text = $"Turns remaining: {turnsLeft}";
    }

}
