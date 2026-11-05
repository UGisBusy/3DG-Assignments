using System.Collections.Generic;
using UnityEngine;

namespace Mini2
{
    // A* search on a GridMap.
    //   g(n) = cost from the start to n
    //   h(n) = estimated cost from n to the goal (octile distance)
    //   f(n) = g(n) + h(n), the open cell with the lowest f is expanded next
    public static class AStar
    {
        const float Straight = 1f;
        const float Diagonal = 1.41421356f;

        static readonly Vector2Int[] Neighbours =
        {
            new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1),
        };

        // Returns the cells from start to goal (both included), or null if the goal cannot be reached.
        public static List<Vector2Int> FindPath(GridMap grid, Vector2Int start, Vector2Int goal)
        {
            if (!grid.IsWalkable(start) || !grid.IsWalkable(goal)) return null;

            int w = grid.size.x, h = grid.size.y;
            var g = new float[w, h];
            var parent = new Vector2Int[w, h];
            var closed = new bool[w, h];
            var inOpen = new bool[w, h];
            for (int x = 0; x < w; x++)
                for (int z = 0; z < h; z++)
                    g[x, z] = float.MaxValue;

            // The grid is small (40 x 40), so a plain list with a linear search for the
            // lowest f is fast enough and easy to follow.
            var open = new List<Vector2Int> { start };
            g[start.x, start.y] = 0f;
            inOpen[start.x, start.y] = true;

            while (open.Count > 0)
            {
                // 1. take the open cell with the lowest f (ties: lowest h, which is closer to the goal)
                int bestIndex = 0;
                float bestF = float.MaxValue, bestH = float.MaxValue;
                for (int i = 0; i < open.Count; i++)
                {
                    Vector2Int c = open[i];
                    float hc = Heuristic(c, goal);
                    float f = g[c.x, c.y] + hc;
                    if (f < bestF || (f == bestF && hc < bestH)) { bestF = f; bestH = hc; bestIndex = i; }
                }
                Vector2Int current = open[bestIndex];
                open[bestIndex] = open[open.Count - 1]; // swap-remove
                open.RemoveAt(open.Count - 1);
                inOpen[current.x, current.y] = false;

                // 2. reached the goal: walk the parent links back to the start
                if (current == goal) return BuildPath(parent, start, goal);

                closed[current.x, current.y] = true;

                // 3. relax the 8 neighbours
                foreach (Vector2Int d in Neighbours)
                {
                    Vector2Int n = current + d;
                    if (!grid.IsWalkable(n) || closed[n.x, n.y]) continue;

                    bool diagonal = d.x != 0 && d.y != 0;
                    // no corner cutting: a diagonal step needs both side cells to be free
                    if (diagonal && (!grid.IsWalkable(new Vector2Int(current.x + d.x, current.y)) ||
                                     !grid.IsWalkable(new Vector2Int(current.x, current.y + d.y))))
                        continue;

                    float newG = g[current.x, current.y] + (diagonal ? Diagonal : Straight);
                    if (newG >= g[n.x, n.y]) continue; // not a better route to n

                    g[n.x, n.y] = newG;
                    parent[n.x, n.y] = current;
                    if (!inOpen[n.x, n.y]) { open.Add(n); inOpen[n.x, n.y] = true; }
                }
            }
            return null; // open list empty: goal is walled off
        }

        // Octile distance: exact cost on an empty 8-direction grid, so it never overestimates.
        static float Heuristic(Vector2Int a, Vector2Int b)
        {
            int dx = Mathf.Abs(a.x - b.x), dz = Mathf.Abs(a.y - b.y);
            return Straight * (dx + dz) + (Diagonal - 2f * Straight) * Mathf.Min(dx, dz);
        }

        static List<Vector2Int> BuildPath(Vector2Int[,] parent, Vector2Int start, Vector2Int goal)
        {
            var path = new List<Vector2Int>();
            for (Vector2Int c = goal; c != start; c = parent[c.x, c.y]) path.Add(c);
            path.Add(start);
            path.Reverse();
            return path;
        }
    }
}
