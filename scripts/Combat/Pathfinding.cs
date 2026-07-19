using System.Collections.Generic;
using System.Linq;

namespace MercenaryBand.Combat;

public class Pathfinding
{
    private readonly HexGrid _grid;

    public Pathfinding(HexGrid grid)
    {
        _grid = grid;
    }

    public List<HexCoord>? FindPath(HexCoord start, HexCoord target, int maxMovement)
    {
        if (!_grid.IsPassable(target) || !_grid.IsPassable(start))
            return null;

        var openSet = new PriorityQueue<HexCoord, float>();
        var cameFrom = new Dictionary<HexCoord, HexCoord>();
        var gScore = new Dictionary<HexCoord, float> { [start] = 0 };

        openSet.Enqueue(start, Heuristic(start, target));

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current == target)
                return ReconstructPath(cameFrom, current);

            foreach (var neighbor in current.Neighbors())
            {
                if (!_grid.IsPassable(neighbor))
                    continue;

                float tentativeG = gScore[current] + _grid.GetMoveCost(neighbor);
                if (tentativeG > maxMovement)
                    continue;

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    float f = tentativeG + Heuristic(neighbor, target);
                    openSet.Enqueue(neighbor, f);
                }
            }
        }

        return null;
    }

    public HashSet<HexCoord> GetReachableTiles(HexCoord start, int maxMovement)
    {
        var reachable = new HashSet<HexCoord>();
        var frontier = new Queue<(HexCoord coord, int cost)>();
        var visited = new Dictionary<HexCoord, int>();

        frontier.Enqueue((start, 0));
        visited[start] = 0;

        while (frontier.Count > 0)
        {
            var (current, currentCost) = frontier.Dequeue();
            reachable.Add(current);

            foreach (var neighbor in current.Neighbors())
            {
                if (!_grid.IsPassable(neighbor))
                    continue;

                int newCost = currentCost + _grid.GetMoveCost(neighbor);
                if (newCost > maxMovement)
                    continue;

                if (!visited.ContainsKey(neighbor) || newCost < visited[neighbor])
                {
                    visited[neighbor] = newCost;
                    frontier.Enqueue((neighbor, newCost));
                }
            }
        }

        return reachable;
    }

    private static float Heuristic(HexCoord a, HexCoord b) => a.DistanceTo(b);

    private static List<HexCoord> ReconstructPath(Dictionary<HexCoord, HexCoord> cameFrom, HexCoord current)
    {
        var path = new List<HexCoord> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }
}
