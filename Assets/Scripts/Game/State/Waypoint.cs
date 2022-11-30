using UnityEngine;

public class Waypoint
{
    // really only required field
    public Vector2Int Position { get; private set; }

    public bool HasTriggeredAction { get; private set; }
    public WaypointController.OnTriggeredAction OnTriggeredAction { get; private set; }

    public WaypointOptions Options { get; private set; }

    public static Waypoint Of(Vector2Int pos)
    {
        return new Waypoint(pos);
    }

    public static Waypoint Of(int x, int y)
    {
        return Of(new Vector2Int(x, y));
    }

    public Waypoint WithOnTriggeredAction(WaypointController.OnTriggeredAction onTriggeredAction)
    {
        HasTriggeredAction = true;
        OnTriggeredAction = onTriggeredAction;
        return this;
    }

    public Waypoint WithOptions(WaypointOptions options)
    {
        Options = options;
        return this;
    }

    private Waypoint(Vector2Int position)
    {
        Position = position;
    }

    public class WaypointOptions
    {
        public float? Size { get; private set; }
        public Color? WaypointColor { get; private set; }

        public static WaypointOptions Of(float size)
        {
            return new WaypointOptions(size);
        }

        public static WaypointOptions Of(float size, Color color)
        {
            return new WaypointOptions(size, color);
        }

        private WaypointOptions(float size)
        {
            Size = size;
        }

        private WaypointOptions(float size, Color color)
        {
            Size = size;
            WaypointColor = color;
        }
    }
}