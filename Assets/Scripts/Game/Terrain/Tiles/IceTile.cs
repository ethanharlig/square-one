using UnityEngine;

// TODO how to make this tile clearly marked as ice even when painted?
public class IceTile : PaintTile
{
    private Vector2Int _beforeMovePosition;

    public delegate void SteppedOnAction(Vector3Int direction);
    public event SteppedOnAction WhenSteppedOn;

    public override bool WillMovePlayer()
    {
        return true;
    }

    void OnPlayerMoveStart(Vector2Int playerPosition)
    {
        _beforeMovePosition = playerPosition;
    }

    void OnPlayerMoveFinish(Vector2Int playerPosition, bool _)
    {
        Vector3Int direction;

        if (playerPosition == _beforeMovePosition)
        {
            return;
        }

        if (playerPosition.x != _beforeMovePosition.x)
        {
            if (playerPosition.x > _beforeMovePosition.x)
            {
                direction = Vector3Int.right;
            }
            else
            {
                direction = Vector3Int.left;
            }

        }
        else
        {
            if (playerPosition.y > _beforeMovePosition.y)
            {
                direction = Vector3Int.forward;
            }
            else
            {
                direction = Vector3Int.back;
            }

        }

        if (playerPosition == GetPosition())
        {
            WhenSteppedOn?.Invoke(direction);
        }
    }

#pragma warning disable IDE0051
    void OnEnable()
    {
        PlayerController.OnMoveStart += OnPlayerMoveStart;
        PlayerController.OnSingleMoveFinish += OnPlayerMoveFinish;
    }

    void OnDisable()
    {
        PlayerController.OnMoveStart -= OnPlayerMoveStart;
        PlayerController.OnSingleMoveFinish -= OnPlayerMoveFinish;
    }
#pragma warning restore IDE0051
}
