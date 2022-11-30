using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : Singleton<CameraController>
{
    public float spd = 0.3f;

    private Vector3 targetEulerAngles;
    private Quaternion startRotation;
    private float progress = 0;

#pragma warning disable IDE0051
    void Start()
    {
        // NEED to set this otherwise framerate is uncapped
        Application.targetFrameRate = 100;
    }

    // this is handling raw input for ideally only webGL
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryRotate(1.0f);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            TryRotate(-1.0f);
        }
    }

    // OnRotate comes from the InputActions action defined Rotate
    void OnRotate(InputValue movementValue)
    {

        float moveDirection = movementValue.Get<float>();
        TryRotate(moveDirection);
    }
    void TryRotate(float moveDirection)
    {
        if (_isRotating)
        {
            return;
        }

        Vector2Int relativeMoveDirection;

        if (moveDirection > 0)
        {
            Vector3 rot = transform.eulerAngles;
            targetEulerAngles = new Vector3(rot.x, rot.y - 90, rot.z);
            relativeMoveDirection = Vector2Int.right;
        }
        else if (moveDirection < 0)
        {
            Vector3 rot = transform.eulerAngles;
            targetEulerAngles = new Vector3(rot.x, rot.y + 90, rot.z);
            relativeMoveDirection = Vector2Int.left;
        }
        else
        {
            Debug.LogWarning("Move direction of 0 provided in camera controller!");
            return;
        }

        _isRotating = true;
        OnCameraRotate?.Invoke(relativeMoveDirection);
        StartCoroutine(Rotate());
    }
#pragma warning restore IDE0051

    public void CenterCameraOnOffset(int x, int z)
    {
        Vector3 pos = transform.position;
        transform.localPosition = new Vector3(pos.x + x, pos.y, pos.z + z);
    }

    // currentPosition here is before player moves
    public delegate void CameraRotateAction(Vector2Int rotateDirection);
    public static event CameraRotateAction OnCameraRotate;

    private bool _isRotating = false;

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
