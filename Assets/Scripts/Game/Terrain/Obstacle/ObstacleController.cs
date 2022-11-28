using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    protected Vector2Int spawnPosition;
    protected Cube Cube { get; set; }

    public void SetName(string name)
    {
        gameObject.name = name;
    }

#pragma warning disable IDE0051
    private void Awake()
    {
        // no need for any before/after roll actions right now
        Cube = new(this, 1.0f, () => { }, (_, _, _) => { });
    }
#pragma warning restore IDE0051

    public Vector2Int GetPositionAsVector2Int()
    {
        return Vector2Int.RoundToInt(new Vector2(transform.position.x, transform.position.z));
    }

    // TODO should be in moving obstacle?
    protected void OnTriggerEnter(Collider other)
    {
        if (!PlayerController.IsColliderPlayer(other)) return;

        PlayerController playerController = other.GetComponent<PlayerController>();
        playerController.StartUpdatingLocation();
        playerController.StopMoving();
    }

    protected void OnTriggerExit(Collider other)
    {
        if (!PlayerController.IsColliderPlayer(other)) return;

        PlayerController playerController = other.GetComponent<PlayerController>();
        playerController.StartMoving();
    }
}