using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : ObstacleController
{
    private enum MoveDirection
    {
        TOWARDS_START_POSITION,
        TOWARDS_PATROL_POSITION
    }

    private bool _isPatrolling;
    private bool _moveTowardsPlayer = false;
    private Func<HashSet<Vector2Int>> getCurrentStationaryObstacles;
    private bool aggressive = true;

    public void StartPatrolling(Vector2Int patrolPosition, Func<HashSet<Vector2Int>> getCurrentStationaryObstaclesAction)
    {
        spawnPosition = GetPositionAsVector2Int();
        getCurrentStationaryObstacles = getCurrentStationaryObstaclesAction;

        StartCoroutine(MoveObstacleOnPatrolCourse(MoveDirection.TOWARDS_PATROL_POSITION));

        // this is bound to go out of the map but I can't be bothered to deal with that right now
        IEnumerator MoveObstacleOnPatrolCourse(MoveDirection moveDirection)
        {
            Vector2Int endPosition;
            if (moveDirection == MoveDirection.TOWARDS_START_POSITION)
            {
                endPosition = spawnPosition;
            }
            else if (moveDirection == MoveDirection.TOWARDS_PATROL_POSITION)
            {
                endPosition = patrolPosition;
            }
            else
            {
                throw new Exception("Unexpected move direction!");
            }

            _isPatrolling = true;
            StartCoroutine(ChangePatrolDirectionWhenDonePatrolling(moveDirection));

            Vector2Int curPosition = GetPositionAsVector2Int();
            MoveTowardsPosition(out int xDiff, out int yDiff, curPosition, endPosition, null);

            while (xDiff != 0 || yDiff != 0)
            {
                // should maybe wait a second or two? we need to let the obstacle finish rolling too?
                yield return null;

                // when we get into the 1 range, we don't want to snap to a zero deficit when we're at like 0.4
                if (xDiff == 1 || yDiff == 1)
                {
                    Debug.Log("You have entered my ceil rounding path, make yourself at home.");
                    curPosition = Vector2Int.CeilToInt(new Vector2(transform.position.x, transform.position.z));
                }
                else
                {
                    curPosition = GetPositionAsVector2Int();
                }
                MoveTowardsPosition(out xDiff, out yDiff, curPosition, endPosition, null);
            }
            _isPatrolling = false;
        }

        IEnumerator ChangePatrolDirectionWhenDonePatrolling(MoveDirection oldMoveDirection)
        {
            while (_isPatrolling)
            {
                // can modify to determine how long should rest at the final position before switching directions
                yield return new WaitForSeconds(0.5f);
            }
            // I think this is okay because this execution of the method will end after the StartCoroutine call
            // and a new execution begins, but the old garbage is freed. if obstacle movement is slow this could be why though
            if (oldMoveDirection == MoveDirection.TOWARDS_START_POSITION)
            {
                StartCoroutine(MoveObstacleOnPatrolCourse(MoveDirection.TOWARDS_PATROL_POSITION));
            }
            else if (oldMoveDirection == MoveDirection.TOWARDS_PATROL_POSITION)
            {
                StartCoroutine(MoveObstacleOnPatrolCourse(MoveDirection.TOWARDS_START_POSITION));
            }
            else
            {
                throw new Exception("Unexpected move direction!");
            }

        }
    }

    public void MoveInDirectionIfNotMovingAndDontEnqueue(Vector3 direction)
    {
        Cube.MoveInDirectionIfNotMovingAndDontEnqueue(direction, _moveType);
    }

    public void MoveTowardsPlayer(PlayerController playerController, Func<HashSet<Vector2Int>> getCurrentStationaryObstaclesAction)
    {
        MoveTowardsPlayer(playerController, getCurrentStationaryObstaclesAction, true);
    }

    public void MoveTowardsPlayer(PlayerController playerController, Func<HashSet<Vector2Int>> getCurrentStationaryObstaclesAction, bool aggressive)
    {
        Cube.SetRollSpeed(playerController.GetRollSpeed());
        _moveTowardsPlayer = true;
        getCurrentStationaryObstacles = getCurrentStationaryObstaclesAction;
        SetAggressive(aggressive);
    }

    private void SetAggressive(bool aggressive)
    {
        this.aggressive = aggressive;
        if (aggressive)
        {
            // red cubes will mess you up
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    // TODO is this only applicable for _moveTowardsPlayer?
    public void StopMovement()
    {
        _moveTowardsPlayer = false;
    }

    public void SetAfterRollAction(Action<bool, bool, Vector3> afterRoll)
    {
        Cube.SetAfterRollAction(afterRoll);
    }

    // obstacles always roll
    private static readonly Cube.MoveType _moveType = Cube.MoveType.ROLL;

    // TODO shouldn't move outside of map too!!
    private void MoveTowardsPosition(out int xDiff, out int yDiff, Vector2Int curPosition, Vector2Int endPosition, Vector2Int? playerPosition)
    {
        xDiff = Mathf.Abs(curPosition.x - endPosition.x);
        yDiff = Mathf.Abs(curPosition.y - endPosition.y);

        if (xDiff == 0 && yDiff == 0) return;

        HashSet<Vector2Int> currentStationaryObstacles = getCurrentStationaryObstacles?.Invoke();

        List<Vector3> directions = new();

        // if bigger x deficit, move that way first
        if (xDiff > yDiff)
        {
            // TODO if something is in the way, try a different path
            // don't need to worry about two moving obstacles next to each other moving in same direction
            if (curPosition.x > endPosition.x)
            {
                directions.Add(Vector3.left);
                // directions.Add(Vector3.right);
            }
            else
            {
                directions.Add(Vector3.right);
                // directions.Add(Vector3.left);
            }

            if (yDiff != 0)
            {
                bool backFirst = curPosition.y > endPosition.y;
                if (backFirst)
                {
                    directions.InsertRange(1, new List<Vector3>() { Vector3.back, Vector3.forward });
                }
                else
                {
                    directions.InsertRange(1, new List<Vector3>() { Vector3.forward, Vector3.back });
                }
            }

        }
        // TODO, add this back in and think of a more explicit solution for when xDiff and yDiff are equivalent
        // else if (xDiff < yDiff)
        else
        {
            {
                if (curPosition.y > endPosition.y)
                {
                    directions.Add(Vector3.back);
                    // directions.Add(Vector3.forward);
                }
                else
                {
                    directions.Add(Vector3.forward);
                    // directions.Add(Vector3.back);
                }
                if (xDiff != 0)
                {
                    bool leftFirst = curPosition.x > endPosition.x;
                    if (leftFirst)
                    {
                        directions.InsertRange(1, new List<Vector3>() { Vector3.left, Vector3.right });
                    }
                    else
                    {
                        directions.InsertRange(1, new List<Vector3>() { Vector3.right, Vector3.left });
                    }
                }
            }
        }

        Queue<Vector3> directionsQueue = new(directions);

        // TODO need to take into account moving obstacles too
        // if there is obstacle in direction that I would move player, don't move there
        MoveInNextDirectionIfNoBlocker(curPosition, directionsQueue, currentStationaryObstacles, playerPosition);
    }

    bool IsStationaryObstacleInWay(int desiredX, int desiredY, HashSet<Vector2Int> currentStationaryObstacles)
    {
        return currentStationaryObstacles.Contains(new Vector2Int(desiredX, desiredY));
    }

    private void MoveInNextDirectionIfNoBlocker(Vector2Int curPosition, Queue<Vector3> directions, HashSet<Vector2Int> currentStationaryObstacles, Vector2Int? playerPosition)
    {
        // not possible to try other way to move now
        if (0 == directions.Count) return;

        Vector3 thisDir = directions.Dequeue();
        Vector3 desiredPosition3d = new Vector3Int(curPosition.x, 0, curPosition.y) + thisDir;
        Vector2Int desiredPosition = new(Mathf.RoundToInt(desiredPosition3d.x), Mathf.RoundToInt(desiredPosition3d.z));

        if (IsStationaryObstacleInWay(desiredPosition.x, desiredPosition.y, currentStationaryObstacles))
        {
            MoveInNextDirectionIfNoBlocker(curPosition, directions, currentStationaryObstacles, playerPosition);
        }
        else
        {
            if (aggressive || desiredPosition != playerPosition)
            {
                Cube.MoveInDirectionIfNotMoving(thisDir, _moveType, false);
            }
        }
    }

    public void UndoLastMove()
    {
        Cube.UndoLastMove();
    }

    private void OnPlayerMoveFullyCompleted(Vector2Int playerPosition, bool moveShouldCount)
    {
        // TODO this triggers when player moves into wall
        if (_moveTowardsPlayer && moveShouldCount)
        {
            // move one unit torwads player's position
            MoveTowardsPosition(out _, out _, GetPositionAsVector2Int(), playerPosition, playerPosition);
        }
    }

#pragma warning disable IDE0051
    // must be done at object enable time
    private void OnEnable()
    {
        PlayerController.OnMoveFullyCompleted += OnPlayerMoveFullyCompleted;

    }

    // make sure to deregister at disable time
    private void OnDisable()
    {
        PlayerController.OnMoveFullyCompleted -= OnPlayerMoveFullyCompleted;
    }

    
#pragma warning restore IDE0051
}