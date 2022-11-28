using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    // currentPosition here is before player moves
    public delegate void MoveStartAction(Vector2Int positionBeforeMove);
    public static event MoveStartAction OnMoveStart;

    // currentPosition here is after player moves one single movement. You probably want to use OnMoveFullyCompleted
    public delegate void SingleMoveFinishAction(Vector2Int positionAfterMove, bool moveShouldCount);
    public static event SingleMoveFinishAction OnSingleMoveFinish;

    // currentPosition here is after player moves
    public delegate void MoveFullyCompletedAction(Vector2Int positionAfterMove, bool oneMoveInThereShouldCount);
    public static event MoveFullyCompletedAction OnMoveFullyCompleted;

    private Vector3 originalPosition;

    private bool inTerminalGameState = false;
    private bool shouldCountMoves = true;
    private int moveCount;
    private GameObject playerInstanceGameObject;
    private Cube Cube { get; set; }

    private Vector2Int currentPosition;
    private bool _isMoving;

    private Func<int, int, bool> tileAtLocationWillMovePlayer;
    private bool forcedStopMoving = false;

    public void SpawnPlayer(int row, int col, Func<int, int, bool> tileAtLocationWillMovePlayer)
    {
        originalPosition = new Vector3(row, 1.5f, col);
        transform.localPosition = originalPosition;
        moveCount = 0;

        playerInstanceGameObject = gameObject;

        this.tileAtLocationWillMovePlayer = tileAtLocationWillMovePlayer;

        // defines roll speed and allows to roll. can pass through beforeMovePosition if we want it
        Cube = new(this, 4.4f, () => BeforeRollActions(), (moveCompleted, moveShouldCount, beforeMovePosition) => AfterRollActions(moveCompleted, moveShouldCount));
        currentPosition = GetRawCurrentPosition();
        CameraController.OnCameraRotate += TrackCameraLocation;
    }

#pragma warning disable IDE0051
    private void OnDisable()
    {
        CameraController.OnCameraRotate -= TrackCameraLocation;
    }

    // OnMove comes from the InputActions action defined Move
    void OnMove(InputValue movementValue)
    {
        if (_isMoving) return;

        Vector2 movementVector = movementValue.Get<Vector2>();

        if (Mathf.Abs(movementVector.x) != 1.0f && Mathf.Abs(movementVector.y) != 1.0f) return;

        int movementX = Mathf.RoundToInt(movementVector.x);
        int movementY = Mathf.RoundToInt(movementVector.y);

        Vector3Int relativeMoveDirection = GetRelativeMoveDirectionWithCameraOffset(movementX, movementY);
        Cube.MoveInDirectionIfNotMoving(relativeMoveDirection, Cube.MoveType.ROLL, shouldCountMoves && !inTerminalGameState);

        // TODO player can float by constant input, how to disallow? prev solution below

        // downwards force disallows wall climbing, constant was chosen because it plays well
        // this solution isn't great but seems good enough, feel free to update it to be cleaner
        // rb.AddForce(Vector3.down * 25, ForceMode.Force);
    }
#pragma warning restore IDE0051

    private void TrackCameraLocation(Vector2Int direction)
    {
        if (direction == Vector2Int.right)
        {
            RotateRight();
        }
        else if (direction == Vector2Int.left)
        {
            RotateLeft();
        }
        else
        {
            Debug.LogErrorFormat("Camera moved in unexpected direction: {0}", direction);
        }
    }

    public int GetMoveCount()
    {
        return moveCount;
    }

    public void ResetPosition()
    {
        playerInstanceGameObject.transform.localPosition = originalPosition;
        Cube.ResetPhysics();
    }

    void BeforeRollActions()
    {
        OnMoveStart?.Invoke(currentPosition);
        _isMoving = true;
    }

    bool anyMoveCompleted = false;
    bool shouldAnyMoveBeCounted = false;

    void AfterRollActions(bool moveCompleted, bool shouldMoveBeCounted)
    {
        _isMoving = false;


        currentPosition = GetRawCurrentPosition();
        OnSingleMoveFinish?.Invoke(currentPosition, moveCompleted && shouldMoveBeCounted);

        anyMoveCompleted |= moveCompleted;
        shouldAnyMoveBeCounted |= shouldMoveBeCounted;

        // ((GridController)GridController.Instance).TileWillMovePlayer(.)

        bool willTileMovePlayer = tileAtLocationWillMovePlayer.Invoke(currentPosition.x, currentPosition.y);
        Debug.LogFormat("forcedStopMoving {0}, will tile move player {1}", forcedStopMoving, willTileMovePlayer);


        if (!forcedStopMoving && willTileMovePlayer)
        {
            return;
        }

        Debug.LogFormat("anyMoveCompleted {0}, shouldAnyMoveBeCounted {1}", anyMoveCompleted, shouldAnyMoveBeCounted);

        if (anyMoveCompleted && shouldAnyMoveBeCounted)
        {
            moveCount++;
        }

        OnMoveFullyCompleted?.Invoke(currentPosition, shouldAnyMoveBeCounted);

        forcedStopMoving = false;
        anyMoveCompleted = false;
        shouldAnyMoveBeCounted = false;
    }

    /**
        This internal method should only be used to get the raw current position of the player. Its public counterpart
        should be used to determine the player's position since it handles the roll animation in its calculation.
    */
    private Vector2Int GetRawCurrentPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(playerInstanceGameObject.transform.position.x), Mathf.RoundToInt(playerInstanceGameObject.transform.position.z));
    }

    private static readonly List<Vector3Int> playerInputDirections = new(){
        Vector3Int.forward, //up 
        Vector3Int.left, // left
        Vector3Int.back, // down
        Vector3Int.right //right
    };

    private int playerInputDirectionCameraOffset = 0;

    void RotateLeft()
    {
        if (playerInputDirectionCameraOffset == 0) { playerInputDirectionCameraOffset = playerInputDirections.Count - 1; }
        else { playerInputDirectionCameraOffset--; }
    }

    void RotateRight()
    {
        if (playerInputDirectionCameraOffset == playerInputDirections.Count - 1) { playerInputDirectionCameraOffset = 0; }
        else { playerInputDirectionCameraOffset++; }
    }

    Vector3Int GetRelativeMoveDirectionWithCameraOffset(int movementX, int movementY)
    {
        if (movementX == -1) return PlayerInputDirectionsFromOffset(1);
        else if (movementX == 1) return PlayerInputDirectionsFromOffset(3);
        else if (movementY == 1) return PlayerInputDirectionsFromOffset(0);
        else if (movementY == -1) return PlayerInputDirectionsFromOffset(2);
        else
        {
            Debug.LogErrorFormat("Tried to move player without one unit movements - movementX: {0}; movementY: {1}", movementX, movementY);
            throw new Exception("Probably shouldn't be an exception but wtf you do?!");
        }

        Vector3Int PlayerInputDirectionsFromOffset(int directionOffset)
        {
            return playerInputDirections[(directionOffset + playerInputDirectionCameraOffset) % playerInputDirections.Count];
        }
    }

    public bool ShouldCountMoves()
    {
        return shouldCountMoves;
    }

    public void StopCountingMoves()
    {
        shouldCountMoves = false;
    }

    public void EnterTerminalGameState()
    {
        inTerminalGameState = true;
    }

    /**
        Get player's location taking into account roll animation. This position only updates once a roll animation completes.
    */
    public Vector2Int GetCurrentPosition()
    {
        return currentPosition;
    }

    /**
        Update the player's known location after it has moved via something other than player;
        e.g. something else (obstacle) interacting with player and causing player to move.

    */
    public void StartUpdatingLocation()
    {
        // this is a bad solution, but it kinda works
        StartCoroutine(StartUpdatingLocation(1.0f));
    }

    /**
        For $seconds seconds, update the player's externally-exposed currentPosition to its current position.
    */
    IEnumerator StartUpdatingLocation(float seconds)
    {
        float interval = 0.1f;
        float elapsed = 0.0f;
        while (elapsed < seconds)
        {
            // if you notice the player's position is screwed up after colliding with an obstacle, validate this is working
            currentPosition = GetRawCurrentPosition();
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        };
    }

    public void ForceMoveInDirection(Vector3 direction)
    {
        StopCountingMoves();
        OnSingleMoveFinish += StartCountingMovesAndDeregisterOnMoveFinish;
        Cube.MoveInDirectionIfNotMoving(direction, Cube.MoveType.SLIDE, false);
    }

    private void StartCountingMovesAndDeregisterOnMoveFinish(Vector2Int pos, bool _)
    {
        StartCountingMoves();
        OnSingleMoveFinish -= StartCountingMovesAndDeregisterOnMoveFinish;
    }

    private void StartCountingMoves()
    {
        shouldCountMoves = true;
    }

    public void FinishMovingThenStopMovement()
    {
        Cube.FinishMovingThenStopMovement();
        forcedStopMoving = true;
    }

    public void StopMoving()
    {
        Cube.StopMoving();
        forcedStopMoving = true;
    }

    public void StartMoving()
    {
        _isMoving = false;
        Cube.StartMoving();
        forcedStopMoving = false;
    }

    public float GetRollSpeed()
    {
        return Cube.GetRollSpeed();
    }

    public static bool IsColliderPlayer(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            // Debug.LogFormat("Obstacle collided with non-player entity: {0}", other);
            return false;
        }

        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogAssertion("Something tagged `Player` with no PlayerController collided with this obstacle!");
            return false;
        }
        return true;
    }
}
