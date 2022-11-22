using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : Singleton<CameraController>
{
    public float spd = 0.3f;

    private Vector3 targetEulerAngles;
    private Quaternion startRotation;
    private float progress = 0;

    void Start()
    {
        // NEED to set this otherwise framerate is uncapped
        Application.targetFrameRate = 144;
    }

    public void CenterCameraOnOffset(int x, int z)
    {
        Vector3 pos = transform.position;
        transform.localPosition = new Vector3(pos.x + x, pos.y, pos.z + z);
    }

    // currentPosition here is before player moves
    public delegate void CameraRotateAction(Vector2Int rotateDirection);
    public static event CameraRotateAction OnCameraRotate;

    private bool _isRotating = false;

    // OnRotate comes from the InputActions action defined Rotate
    void OnRotate(InputValue movementValue)
    {
        if (_isRotating)
        {
            Debug.Log("Tried to call rotate when _isRotating is true");
            return;
        }

        float moveDirection = movementValue.Get<float>();
        Vector2Int relativeMoveDirection;

        if (moveDirection > 0)
        {
            Debug.Log("positive move direction");
            Vector3 rot = transform.eulerAngles;
            targetEulerAngles = new Vector3(rot.x, rot.y - 90, rot.z);
            relativeMoveDirection = Vector2Int.right;
        }
        else if (moveDirection < 0)
        {
            Debug.Log("negative move direction");
            Vector3 rot = transform.eulerAngles;
            targetEulerAngles = new Vector3(rot.x, rot.y + 90, rot.z);
            relativeMoveDirection = Vector2Int.left;
        }
        else
        {
            Debug.LogError("Move direction of 0 provided in camera controller!");
            return;
        }

        _isRotating = true;
        OnCameraRotate?.Invoke(Vector2Int.right);
        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        startRotation = transform.rotation;
        while (_isRotating && progress < 1)
        {
            transform.rotation = Quaternion.Lerp(startRotation, Quaternion.Euler(targetEulerAngles), progress);

            // move with frame rate
            progress += spd * Time.fixedDeltaTime;

            // halt iteration until next frame
            yield return null;
        }

        _isRotating = false;
        progress = 0;

        yield return null;
    }

}
