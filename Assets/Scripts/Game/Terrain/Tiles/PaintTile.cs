using UnityEngine;

public class PaintTile : TileController
{
    public Color? LastColor { get; private set; }

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
        LastColor = GetColor();
        gameObject.GetComponent<MeshRenderer>().material.color = color;
        IsPainted = true;
    }

    public void PaintLastColor()
    {
        if (LastColor.HasValue)
        {
            Paint(LastColor.Value);
        }
    }
}
