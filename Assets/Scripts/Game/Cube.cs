using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube
{
    // TODO ethan this seems super dangerous but I can't think of better solution for now
    private readonly MonoBehaviour _mb;
    private float _rollSpeed = 1.0f;

    public float GetRollSpeed()
    {
        return _rollSpeed;
    }

    public void SetRollSpeed(float rollSpeed)
    {
        _rollSpeed = rollSpeed;
    }

    private readonly Action _beforeRoll;
    private Action<bool, bool> _afterRoll;
    private readonly Queue<Tuple<Vector3, MoveType, bool>> queuedMovements;

    private bool circuitBreakMovement = false;

    public Cube(MonoBehaviour mb, float rollSpeed, Action beforeRoll, Action<bool, bool> afterRoll)
    {
        _mb = mb;
        _rollSpeed = rollSpeed;
        _beforeRoll = beforeRoll;
        _afterRoll = afterRoll;
        queuedMovements = new();
    }

    protected Rigidbody rb;

    private bool _isMoving;

    public enum MoveType
    {
        ROLL,
        SLIDE
    }

    public void SetAfterRollAction(Action<bool, bool> afterRoll)
    {
        _afterRoll = afterRoll;
    }

    public void MoveInDirectionIfNotMovingAndDontEnqueue(Vector3 dir, MoveType moveType)
    {
        if (_isMoving)
        {
            return;
        }
        else if (queuedMovements.Count == 0)
        {
            // never count these moves
            queuedMovements.Enqueue(new(dir, moveType, false));
            DoQueuedRotation();
        }

    }

    public void MoveInDirectionIfNotMoving(Vector3 dir, MoveType moveType, bool moveShouldCount)
    {
        if (queuedMovements.Count == 0)
        {
            queuedMovements.Enqueue(new(dir, moveType, moveShouldCount));
        }
        if (_isMoving)
        {
            return;
        }

        DoQueuedRotation();
    }

    private void DoQueuedRotation()
    {
        if (_isMoving) return;

        if (queuedMovements.Count == 0) return;

        Tuple<Vector3, MoveType, bool> movement = queuedMovements.Dequeue();

        Vector3 dir = movement.Item1;
        MoveType moveType = movement.Item2;
        bool moveShouldCount = movement.Item3;

        // lock
        _isMoving = true;
        circuitBreakMovement = false;
        _beforeRoll.Invoke();

        if (rb == null)
        {
            rb = _mb.gameObject.GetComponent<Rigidbody>();
        }

        if (moveType == MoveType.ROLL)
        {
            var anchor = _mb.gameObject.transform.localPosition + (Vector3.down + dir) * 0.5f;
            var axis = Vector3.Cross(Vector3.up, dir);
            float rotationRemaining = 90;
            _mb.StartCoroutine(Roll(anchor, axis, rotationRemaining, moveShouldCount));
        }
        else if (moveType == MoveType.SLIDE)
        {
            _mb.StartCoroutine(Slide(dir, moveShouldCount));
        }
        else
        {
            throw new Exception($"Unknown moveType: {moveType}");
        }
    }

    IEnumerator Slide(Vector3 dir, bool moveShouldCount)
    {
        Vector3 currentPos = _mb.gameObject.transform.position;
        Vector3 targetPos = _mb.gameObject.transform.position + dir;

        bool finishedMove = true;
        int numSteps = 20;
        for (var i = 0; i < numSteps; i++)
        {
            if (circuitBreakMovement) { finishedMove = false; break; };

            _mb.gameObject.transform.position = Vector3.Lerp(currentPos, targetPos, 1.0f / numSteps * i);
            yield return null;
        }
        FinishMovement(finishedMove, moveShouldCount);
    }

    IEnumerator Roll(Vector3 anchor, Vector3 axis, float rotationRemaining, bool moveShouldCount)
    {
        bool finishedMove = true;
        for (var i = 0; i < 90 / _rollSpeed; i++)
        {
            if (circuitBreakMovement) { finishedMove = false; break; };

            float rotationAngle = Mathf.Min(_rollSpeed, rotationRemaining);
            _mb.gameObject.transform.RotateAround(anchor, axis, rotationAngle);
            rotationRemaining -= _rollSpeed;
            yield return null;
        }
        FinishMovement(finishedMove, moveShouldCount);
    }

    private void FinishMovement(bool didFinishMove, bool moveShouldCount)
    {
        Vector3 pos = _mb.gameObject.transform.position;
        _mb.gameObject.transform.localPosition = Vector3Int.RoundToInt(pos);
        ResetPhysics();

        // unlock
        _isMoving = false;
        circuitBreakMovement = false;

        _afterRoll?.Invoke(didFinishMove, moveShouldCount);

        DoQueuedRotation();
    }

    public void ResetPhysics()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        RotateToNearestRightAngles();

        void RotateToNearestRightAngles()
        {
            Quaternion roundedRotation = new(
                ClosestRightAngle(_mb.gameObject.transform.rotation.x),
                ClosestRightAngle(_mb.gameObject.transform.rotation.y),
                ClosestRightAngle(_mb.gameObject.transform.rotation.z),
                _mb.gameObject.transform.rotation.w);

            _mb.gameObject.transform.rotation = roundedRotation;

            static int ClosestRightAngle(float rotation)
            {
                bool isPositive = rotation > 0;
                return Mathf.RoundToInt(rotation) * 90 * (isPositive ? 1 : -1);
            }
        }
    }

    public void StopMoving()
    {
        Debug.Log("Clearing queue");
        queuedMovements.Clear();
        ResetPhysics();
        circuitBreakMovement = true;
    }

    public void StartMoving()
    {
        Debug.Log("Starting moving");
        circuitBreakMovement = false;
    }
}
