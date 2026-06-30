using Safari;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// Represents a Jeep that can navigate a map and carry the passengers.
/// </summary>
public class Jeep
{
    /// <summary>
    /// Gets the current X-coordinate of the Jeep.
    /// </summary>
    public float PixelX { get; private set; }

    /// <summary>
    /// Gets the current Y-coordinate of the Jeep.
    /// </summary>
    public float PixelY { get; private set; }

    /// <summary>
    /// Gets the number of passengers currently in the Jeep.
    /// </summary>
    public int Passengers { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the Jeep is currently moving.
    /// </summary>
    public bool IsMoving { get; private set; }

    private List<(int x, int y)> path;
    private int currentPathIndex;
    private float speed = 10f;
    private const int TileSize = 32;

    /// <summary>
    /// Initializes a new instance of the <see cref="Jeep"/> class.
    /// </summary>
    public Jeep()
    {
        Passengers = 0;
        path = new List<(int x, int y)>();
        IsMoving = false;
    }

    /// <summary>
    /// Starts the Jeep's route by finding a path on the given map.
    /// </summary>
    /// <param name="map">The map on which the Jeep will navigate.</param>
    public void StartRoute(Map map)
    {
        if (!FindPath(map))
        {
            // No valid path found
            return;
        }

        if (path.Count > 0)
        {
            PixelX = path[0].x * TileSize + TileSize / 2;
            PixelY = path[0].y * TileSize + TileSize / 2;
            currentPathIndex = 0;
            IsMoving = true;
        }
    }

    /// <summary>
    /// Determines whether a valid path exists on the given map.
    /// </summary>
    /// <param name="map">The map to check for a valid path.</param>
    /// <returns><c>true</c> if a valid path exists; otherwise, <c>false</c>.</returns>
    public static bool HasPath(Map map)
    {
        Jeep tempJeep = new Jeep();
        return tempJeep.FindPath(map);
    }

    /// <summary>
    /// Updates the Jeep's position along its path.
    /// </summary>
    public void UpdatePosition()
    {
        if (!IsMoving || path.Count == 0) return;

        int targetX = path[currentPathIndex].x * TileSize + TileSize / 2;
        int targetY = path[currentPathIndex].y * TileSize + TileSize / 2;

        // Calculate direction
        float dx = targetX - PixelX;
        float dy = targetY - PixelY;
        float distance = (float)Math.Sqrt(dx * dx + dy * dy);

        // Move toward target
        if (distance > speed)
        {
            PixelX += dx / distance * speed;
            PixelY += dy / distance * speed;
        }
        else
        {
            // Reached current waypoint
            PixelX = targetX;
            PixelY = targetY;
            currentPathIndex++;

            // Check if reached end of path
            if (currentPathIndex >= path.Count)
            {
                IsMoving = false;
                // Jeep has completed its route
            }
        }
    }

    /// <summary>
    /// Finds a valid path for the Jeep on the given map.
    /// </summary>
    /// <param name="map">The map on which to find a path.</param>
    /// <returns><c>true</c> if a valid path is found; otherwise, <c>false</c>.</returns>
    public bool FindPath(Map map)
    {
        path.Clear();

        // Find all door tiles
        List <(int,int)> doors = FindAllDoors(map);
        if (doors.Count < 2) return false;

        // Find top door (entry) - smallest Y value
        (int,int) topDoor = doors.OrderBy(d => d.Item2).First();

        // Find bottom doors (exits) - exclude the top door
        var bottomDoors = doors.Where(d => d.Item2 > topDoor.Item2).ToList();
        if (bottomDoors.Count == 0) return false;

        // Randomly select a bottom door
        Random random = new Random();
        (int, int) bottomDoor = bottomDoors[random.Next(bottomDoors.Count)];

        // Find path from top to bottom door
        return CalculatePath(map, topDoor, bottomDoor);
    }

    /// <summary>
    /// Calculates a path from the start to the end point using BFS.
    /// </summary>
    /// <param name="map">The map on which to calculate the path.</param>
    /// <param name="start">The starting point of the path.</param>
    /// <param name="end">The ending point of the path.</param>
    /// <returns><c>true</c> if a path is successfully calculated; otherwise, <c>false</c>.</returns>
    private bool CalculatePath(Map map, (int x, int y) start, (int x, int y) end)
    {
        var cameFrom = new Dictionary<(int, int), (int, int)>();
        Queue<(int,int)> queue = new Queue<(int, int)>();
        queue.Enqueue(start);
        cameFrom[start] = (-1, -1);

        (int, int)[] directions = { (-1, 0), (1, 0), (0, -1), (0, 1) };

        while (queue.Count > 0)
        {
            (int, int) current = queue.Dequeue();

            if (current == end)
            {
                path = ReconstructPath(cameFrom, start, end);
                return true;
            }

            foreach (var dir in directions)
            {
                (int, int) neighbor = (current.Item1 + dir.Item1, current.Item2 + dir.Item2);

                if (IsValidRoadTile(map, neighbor) && !cameFrom.ContainsKey(neighbor))
                {
                    cameFrom[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Reconstructs the path from the start to the end point.
    /// </summary>
    /// <param name="cameFrom">The dictionary containing the path information.</param>
    /// <param name="start">The starting point of the path.</param>
    /// <param name="end">The ending point of the path.</param>
    /// <returns>A list of coordinates representing the path.</returns>
    private List<(int x, int y)> ReconstructPath(
        Dictionary<(int, int), (int, int)> cameFrom,
        (int, int) start,
        (int, int) end)
    {
        var path = new List<(int, int)>();
        (int, int) current = end;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }

    /// <summary>
    /// Finds all door tiles on the map.
    /// </summary>
    /// <param name="map">The map to search for door tiles.</param>
    /// <returns>A list of coordinates representing door tiles.</returns>
    private List<(int, int)> FindAllDoors(Map map)
    {
        var doors = new List<(int, int)>();
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                if (map.GetTile(x, y) == 6) // Door
                {
                    doors.Add((x, y));
                }
            }
        }
        return doors;
    }

    /// <summary>
    /// Determines whether a given tile is a valid road or door tile.
    /// </summary>
    /// <param name="map">The map to check.</param>
    /// <param name="position">The position of the tile to check.</param>
    /// <returns><c>true</c> if the tile is valid; otherwise, <c>false</c>.</returns>
    private bool IsValidRoadTile(Map map, (int, int) position)
    {
        if (position.Item1 < 0 || position.Item1 >= map.Width ||
            position.Item2 < 0 || position.Item2 >= map.Height)
        {
            return false;
        }

        int tile = map.GetTile(position.Item1, position.Item2);
        return tile == 5 || tile == 6; // Road or door
    }
}
