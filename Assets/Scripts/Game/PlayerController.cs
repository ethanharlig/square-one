using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    public delegate void MoveStartAction();
    public static event MoveStartAction OnMoveStart;

    public delegate void MoveFinishAction();
    public static event MoveFinishAction OnMoveFinish;

    private Rigidbody rb;
    private float movementX, movementY;
    private Vector3 originalPosition;

    private bool shouldCountMoves = true;
    private int moveCount;
    private GameObject playerInstanceGameObject;

    public void SpawnPlayer(int row, int col)
    {
        originalPosition = new Vector3(row, 1.5f, col);
        transform.localPosition = originalPosition;
        moveCount = 0;

        playerInstanceGameObject = gameObject;

        // playerInstanceGameObject = Instantiate(gameObject, originalPosition, Quaternion.identity);
        // assumes that a Rigidbody exists on this GameObject
        rb = playerInstanceGameObject.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public int GetMoveCount()
    {
        return moveCount;
    }

    public void ResetPosition()
    {
        playerInstanceGameObject.transform.localPosition = originalPosition;
        ResetPlayerPhysics();
    }

    // ripped this code, rethink it
    [SerializeField] private readonly float _rollSpeed = 5;
    private bool _isMoving;

    // OnMove comes from the InputActions action defined Move
    void OnMove(InputValue movementValue)
    {
        if (_isMoving) return;

        Vector2 movementVector = movementValue.Get<Vector2>();
        if (Mathf.Abs(movementVector.x) != 1.0f && Mathf.Abs(movementVector.y) != 1.0f) return;
        movementX = movementVector.x;
        movementY = movementVector.y;


        // Debug.LogFormat("Moving player w this vector: {0}", movementVector);
        if (!_isMoving)
        {
            // lock
            _isMoving = true;
            // call delegate
            OnMoveStart?.Invoke();

            if (movementX == -1) Assemble(Vector3.left);
            else if (movementX == 1) Assemble(Vector3.right);
            else if (movementY == 1) Assemble(Vector3.forward);
            else if (movementY == -1) Assemble(Vector3.back);
        }
        else
        {
            return;
        }

        void Assemble(Vector3 dir)
        {
            var anchor = playerInstanceGameObject.transform.localPosition + (Vector3.down + dir) * 0.5f;
            var axis = Vector3.Cross(Vector3.up, dir);
            // I think I want less of a Roll and more of a fixed one unit movement
            float rotationRemaining = 90;

            // TODO different math for tiny player?
            StartCoroutine(Roll(anchor, axis));

            IEnumerator Roll(Vector3 anchor, Vector3 axis)
            {
                for (var i = 0; i < 90 / _rollSpeed; i++)
                {
                    float rotationAngle = Mathf.Min(_rollSpeed, rotationRemaining);
                    playerInstanceGameObject.transform.RotateAround(anchor, axis, rotationAngle);
                    rotationRemaining -= _rollSpeed;
                    // yield return new WaitForSeconds(0.01f);
                    yield return null;
                }

                Vector3 pos = playerInstanceGameObject.transform.position;
                playerInstanceGameObject.transform.localPosition = Vector3Int.RoundToInt(pos);
                ResetPlayerPhysics();

                // player should have their move count increased once they've finished moving
                if (shouldCountMoves) moveCount++;

                // call delegate
                OnMoveFinish?.Invoke();

                _isMoving = false;
                yield return null;
            }

        }
    }

    void ResetPlayerPhysics()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        RotateToNearestRightAngles();

        void RotateToNearestRightAngles()
        {
            Quaternion roundedRotation = new(
                ClosestRightAngle(playerInstanceGameObject.transform.rotation.x),
                ClosestRightAngle(playerInstanceGameObject.transform.rotation.y),
                ClosestRightAngle(playerInstanceGameObject.transform.rotation.z),
                playerInstanceGameObject.transform.rotation.w);

            playerInstanceGameObject.transform.rotation = roundedRotation;

            static int ClosestRightAngle(float rotation)
            {
                bool isPositive = rotation > 0;
                return Mathf.RoundToInt(rotation) * 90 * (isPositive ? 1 : -1);
            }
        }
    }

    public void StopCountingMoves()
    {
        shouldCountMoves = false;
    }

    public Vector2Int GetRoundedPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(playerInstanceGameObject.transform.position.x), Mathf.RoundToInt(playerInstanceGameObject.transform.position.z));
    }

}
