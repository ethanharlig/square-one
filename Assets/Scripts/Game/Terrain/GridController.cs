using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class GridController : Singleton<GridController>
{
    // TODO will change to BaseTile and use a bare TileController
#pragma warning disable IDE0044
    [SerializeField] private GameObject paintTilePrefab;
    [SerializeField] private GameObject iceTilePrefab;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject waypointPrefab;
#pragma warning disable IDE0044

    private List<List<TileController>> gridRows;
    private Color startingColor;
    private GameObject _obstacleGameObject;
    private HashSet<Vector2Int> stationaryObstaclePositions = new();
    // TODO need to keep track of moving obstacles too
    private OrderedDictionary movingObstaclePositionToControllerMap = new();

    public bool TileWillMovePlayer(int x, int y)
    {
        TileController tile = TileAtLocation(x, y);

        return tile != null && tile.WillMovePlayer();
    }

    public void SetupGrid(int xSize, int ySize)
    {
        List<List<TileController>> rows = new();

        for (int x = 0; x < xSize; x++)
        {
            List<TileController> thisRow = new();
            GameObject xRowObj = new(string.Format("X{0}", x));
            for (int y = 0; y < ySize; y++)
            {
                GameObject tile;

                tile = Instantiate(paintTilePrefab);
                tile.transform.localPosition = new Vector3(x, 0, y);
                tile.name = string.Format("Y{0} - Paint", y);
                tile.transform.parent = xRowObj.transform;

                thisRow.Add(tile.GetComponent<TileController>());

                if (startingColor == null)
                {
                    startingColor = thisRow[0].GetComponent<MeshRenderer>().material.color;
                }

            }
            xRowObj.transform.parent = transform;
            rows.Add(thisRow);
        }
        gridRows = rows;
    }

    public IceTile SpawnIceTile(int x, int y, IceTile.SteppedOnAction steppedOnAction)
    {
        if (IsWithinGrid(x, y))
        {
            TileAtLocation(x, y).GetTile().SetActive(false);
            Transform parent = transform.GetChild(x).transform;

            GameObject tile = Instantiate(iceTilePrefab);
            tile.transform.localPosition = new Vector3(x, 0, y);
            tile.name = string.Format("y{0} - Ice", y);
            tile.transform.parent = parent;

            IceTile iceTile = tile.GetComponent<IceTile>();
            iceTile.WhenSteppedOn += steppedOnAction;

            gridRows[x][y] = iceTile;
            return iceTile;
        }

        return null;
    }

    public List<IceTile> SpawnIceTilesAroundPosition(int x, int y, IceTile.SteppedOnAction steppedOnAction)
    {
        List<IceTile> iceTilesCreated = new();
        for (int xAttempt = x - 1; xAttempt <= x + 1; xAttempt++)
        {
            for (int yAttempt = y - 1; yAttempt <= y + 1; yAttempt++)
            {
                // don't spawn ice tile here
                if (xAttempt == x && yAttempt == y) continue;

                IceTile iceTile = SpawnIceTile(xAttempt, yAttempt, steppedOnAction);
                if (iceTile == null) continue;

                iceTilesCreated.Add(iceTile);
            }
        }
        return iceTilesCreated;
    }

    public ObstacleController AddStationaryObstacleAtPosition(int x, int y)
    {
        if (_obstacleGameObject == null)
        {
            _obstacleGameObject = new("Obstacles");
        }
        // obstacles spawn on top of floor
        GameObject obj = Instantiate(obstaclePrefab, new Vector3Int(x, 1, y), Quaternion.identity);
        ObstacleController obstacle = obj.AddComponent<ObstacleController>();
        obstacle.SetName($"static - row{x}col{y}");

        obstacle.transform.parent = _obstacleGameObject.transform;

        stationaryObstaclePositions.Add(new Vector2Int(x, y));
        return obstacle;
    }

    public MovingObstacle AddMovingObstacleAtPosition(int x, int y)
    {
        if (_obstacleGameObject == null)
        {
            _obstacleGameObject = new("Obstacles");
        }

        GameObject obj = Instantiate(obstaclePrefab, new Vector3Int(x, 1, y), Quaternion.identity);
        MovingObstacle obstacle = obj.AddComponent<MovingObstacle>();
        obstacle.SetName($"moving - row{x}col{y}");
        obstacle.SetAfterRollAction((didFinishMove, _, beforeMovePosition) => AfterMovingObjectMoves(beforeMovePosition));
        movingObstaclePositionToControllerMap.Add(obstacle.GetPositionAsVector2Int(), obstacle);

        // TODO could add this obstacle into a queue and make all obstacles move in order

        // moving obstacle should call function to update something with its location
        // obstacle.SetLocationUpdaterFunction();

        obstacle.transform.parent = _obstacleGameObject.transform;

        // moving obstacles are yellow
        obj.GetComponent<MeshRenderer>().material.color = Color.yellow;

        return obstacle;
    }

    // todo just throw shit into a queue and process it there
    void AfterMovingObjectMoves(Vector3 beforeMovePosition3d)
    {
        Vector2Int beforeMovePosition = new(Mathf.RoundToInt(beforeMovePosition3d.x), Mathf.RoundToInt(beforeMovePosition3d.z));
        if (!movingObstaclePositionToControllerMap.Contains(beforeMovePosition))
        {
            Debug.LogWarningFormat("Didn't find this obstacle previously in this position {0}", beforeMovePosition);
            return;
        }
        MovingObstacle obstacle = (MovingObstacle)movingObstaclePositionToControllerMap[beforeMovePosition];

        if (!movingObstaclePositionToControllerMap.Contains(obstacle.GetPositionAsVector2Int()))
        {
            movingObstaclePositionToControllerMap.Remove(beforeMovePosition);
            movingObstaclePositionToControllerMap.Add(obstacle.GetPositionAsVector2Int(), obstacle);
            return;
        }
        else
        {
            // TODO if two move to the same square, they can both end up in this else block... how? 
            Debug.LogFormat("Something is already here {0}, going back to old spot!", beforeMovePosition);
            obstacle.UndoLastMove();
        }
    }

    private HashSet<Vector2Int> GetCurrentStationaryObstacles()
    {
        return stationaryObstaclePositions;
    }

    public Func<HashSet<Vector2Int>> GetCurrentStationaryObstaclesAction()
    {
        return () => GetCurrentStationaryObstacles();
    }

    public TileController TileAtLocation(int x, int y)
    {
        return TileAtLocation(new Vector2Int(x, y));
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

    public Color? TileColorAtLocation(Vector2Int position)
    {
        if (!IsWithinGrid(position))
        {
            return null;
        }
        if (TileAtLocation(position) is not PaintTile)
        {
            return null;
        }

        return ((PaintTile)TileAtLocation(position)).GetColor();
    }

    public bool PaintTileAtLocation(Vector2Int position, Color color)
    {
        return PaintTileAtLocation(position.x, position.y, color);
    }

    /**
    Returns bool indicating if a tile at this location was painted
    */
    public bool PaintTileAtLocation(int x, int y, Color color)
    {
        if (!PaintTileExists(x, y)) { return false; }

        ((PaintTile)gridRows[x][y]).Paint(color);
        return true;
    }

    public bool PaintLastColorForTileAtLocation(Vector2Int position)
    {
        return PaintLastColorForTileAtLocation(position.x, position.y);
    }

    /**
    Returns bool indicating if a tile at this location was painted
    */
    public bool PaintLastColorForTileAtLocation(int x, int y)
    {
        if (!PaintTileExists(x, y)) { return false; }

        ((PaintTile)gridRows[x][y]).PaintLastColor();
        return true;
    }

    private bool PaintTileExists(int x, int y)
    {
        if (!IsWithinGrid(x, y))
        {
            return false;
        }
        if (TileAtLocation(x, y) is not PaintTile)
        {
            Debug.Log("Paint tile at location returned a non-paint tile!");
            return false;
        }
        return true;
    }


    public void SpawnWaypoint(Waypoint waypoint, WaypointController.OnTriggeredAction onTriggeredAction)
    {
        int x = waypoint.Position.x;
        int y = waypoint.Position.y;

        GameObject waypointGameObject = Instantiate(waypointPrefab);
        waypointGameObject.transform.localPosition = new Vector3(x, 1.0f, y);
        waypointGameObject.name = string.Format("Waypoint [{0}, {1}]", x, y);
        WaypointController waypointController = waypointGameObject.GetComponent<WaypointController>();

        if (waypoint.HasTriggeredAction)
        {
            waypointController.OnTriggered += waypoint.OnTriggeredAction;
        }
        else
        {
            waypointController.OnTriggered += onTriggeredAction;
        }

        // TODO animate this lil guy and make them spin or something
        if (waypoint.Options != null)
        {
            if (waypoint.Options.Size.HasValue)
            {
                waypointController.SetSize(waypoint.Options.Size.Value);
            }
            if (waypoint.Options.WaypointColor.HasValue)
            {
                waypointController.SetColor(waypoint.Options.WaypointColor.Value);
            }
            if (waypoint.Options.EnableAudio.HasValue)
            {
                waypointController.EnableAudio = waypoint.Options.EnableAudio.Value;
            }
        }
    }

    public List<Vector2Int> PaintTilesAdjacentToLocation(Vector2 position, Color color)
    {
        return PaintTilesAdjacentToLocation((int)position.x, (int)position.y, new() { color });
    }

    public List<Vector2Int> PaintTilesAdjacentToLocation(Vector2 position, List<Color> colors)
    {
        // TODO return positions
        return PaintTilesAdjacentToLocation((int)position.x, (int)position.y, colors);
    }

    public List<Vector2Int> PaintTilesAdjacentToLocation(int x, int y, List<Color> colors)
    {
        Vector2Int[] possibleLocations = new[]{
            new Vector2Int(x-1,y),
            new Vector2Int(x+1,y),
            new Vector2Int(x,y-1),
            new Vector2Int(x,y+1)
        };

        List<Vector2Int> paintedTiles = new();
        for (int ndx = 0; ndx < possibleLocations.Length; ndx++)
        {
            Vector2Int location = possibleLocations[ndx];
            Color color;
            if (ndx >= colors.Count)
            {
                color = colors[ndx];
            }
            else
            {
                color = colors[ndx];
            }

            if (PaintTileAtLocation(location, color))
            {
                paintedTiles.Add(location);
            }
        }
        return paintedTiles;
    }

    public bool IsWithinGrid(Vector2Int position)
    {
        return IsWithinGrid(position.x, position.y);
    }

    public bool IsWithinGrid(int x, int y)
    {
        return x >= 0 && x < gridRows.Count && y >= 0 && y < gridRows.Count;
    }
}
