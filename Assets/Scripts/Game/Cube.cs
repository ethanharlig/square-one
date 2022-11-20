using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube
{
    // TODO ethan this seems super dangerous but I can't think of better solution for now
    private readonly MonoBehaviour _mb;
    private readonly float _rollSpeed = 1.0f;

    public Cube(MonoBehaviour _mb)
    {
        this._mb = _mb;
    }

    public Cube(MonoBehaviour _mb, float rollSpeed)
    {
        this._mb = _mb;
        _rollSpeed = rollSpeed;
    }

    protected Rigidbody rb;

    private bool _isRotating;

    delegate void BeforeRollAction();
    private event BeforeRollAction BeforeRoll;
    delegate void AfterRollAction();
    private event AfterRollAction AfterRoll;

    public void Assemble(Vector3 dir)
    {
        if (_isRotating) return;

        // lock
        _isRotating = true;

        // call delegate
        BeforeRoll?.Invoke();

        if (rb == null)
        {
            rb = _mb.gameObject.GetComponent<Rigidbody>();
        }


        var anchor = _mb.gameObject.transform.localPosition + (Vector3.down + dir) * 0.5f;
        var axis = Vector3.Cross(Vector3.up, dir);
        // I think I want less of a Roll and more of a fixed one unit movement
        float rotationRemaining = 90;

        // TODO different math for tiny player?
        _mb.StartCoroutine(Roll(anchor, axis));

        IEnumerator Roll(Vector3 anchor, Vector3 axis)
        {
            for (var i = 0; i < 90 / _rollSpeed; i++)
            {
                float rotationAngle = Mathf.Min(_rollSpeed, rotationRemaining);
                _mb.gameObject.transform.RotateAround(anchor, axis, rotationAngle);
                rotationRemaining -= _rollSpeed;

                // downwards force disallows wall climbing, constant was chosen because it plays well
                // this solution isn't great but seems good enough, feel free to update it to be cleaner
                rb.AddForce(Vector3.down * 25, ForceMode.Force);

                yield return null;
            }

            Vector3 pos = _mb.gameObject.transform.position;
            _mb.gameObject.transform.localPosition = Vector3Int.RoundToInt(pos);
            ResetPhysics();

            // call delegate
            AfterRoll?.Invoke();

            _isRotating = false;
        }

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
}
