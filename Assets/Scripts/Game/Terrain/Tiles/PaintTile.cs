using UnityEngine;

// TODO remove this and just use BaseTile once we are using GameStateMager with waypoints
public class PaintTile : TileController
{
    public bool IsPainted
    {
        get; private set;
    }

    public Color GetColor()
    {
        return gameObject.GetComponent<MeshRenderer>().material.color;
    }

    public void Paint(Color color)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = color;
        IsPainted = true;
    }
}
