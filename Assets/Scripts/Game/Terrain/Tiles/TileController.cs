using UnityEngine;

public class TileController : MonoBehaviour
{

    public virtual bool WillMovePlayer()
    {
        return false;
    }

    public GameObject GetTile()
    {
        return gameObject;
    }

    protected Vector2Int GetPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(gameObject.transform.position.x), Mathf.RoundToInt(gameObject.transform.position.z));
    }
}