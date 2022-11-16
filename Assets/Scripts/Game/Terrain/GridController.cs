using System.Collections.Generic;
using UnityEngine;

public class GridController : Singleton<GridController>
{
    public GameObject tilePrefab;

    private List<List<TileController>> gridRows;

    private Color startingColor;

    public void SetupGrid(int width, int length)
    {
        List<List<TileController>> rows = new();

        for (int row = 0; row < width; row++)
        {
            List<TileController> thisRow = new();
            GameObject rowObj = new(string.Format("row{0}", row));
            for (int col = 0; col < length; col++)
            {
                GameObject tile = Instantiate(tilePrefab);
                tile.transform.localPosition = new Vector3(row, 0, col);

                tile.name = string.Format("col{0}", col);
                tile.transform.parent = rowObj.transform;

                thisRow.Add(tile.GetComponent<TileController>());

                if (startingColor == null)
                {
                    startingColor = thisRow[0].GetComponent<MeshRenderer>().material.color;
                }
            }
            rowObj.transform.parent = transform;
            rows.Add(thisRow);
        }
        gridRows = rows;

    }

    /**
    * Get Tile at Vector2Int position. Returns null if position is not within grid.
    */
    public TileController TileAtLocation(Vector2Int position)
    {
        if (!IsWithinGrid(position))
        {
            return null;
        }
        return gridRows[position.x][position.y];
    }

    public void ResetTileColorAtLocation(Vector2Int position)
    {
        PaintTileAtLocation(position, startingColor);
    }

    public Color? TileColorAtLocation(Vector2Int position)
    {
        if (!IsWithinGrid(position))
        {
            return null;
        }

        return TileAtLocation(position).GetColor();
    }

    public void PaintTileAtLocation(Vector2Int position, Color color)
    {
        PaintTileAtLocation(position.x, position.y, color);
    }

    public void PaintTileAtLocation(int x, int z, Color color)
    {
        if (IsWithinGrid(x, z))
            gridRows[x][z].Paint(color);
    }

    public void PaintTilesAdjacentToLocation(Vector2 position, Color color)
    {
        PaintTilesAdjacentToLocation((int)position.x, (int)position.y, color);
    }

    public void PaintTilesAdjacentToLocation(int x, int z, Color color)
    {
        PaintTileAtLocation(x - 1, z, color);
        PaintTileAtLocation(x + 1, z, color);
        PaintTileAtLocation(x, z - 1, color);
        PaintTileAtLocation(x, z + 1, color);
    }

    public bool IsWithinGrid(Vector2Int position)
    {
        return IsWithinGrid(position.x, position.y);
    }

    public bool IsWithinGrid(int x, int z)
    {
        return x >= 0 && x < gridRows.Count && z >= 0 && z < gridRows.Count;
    }
}
