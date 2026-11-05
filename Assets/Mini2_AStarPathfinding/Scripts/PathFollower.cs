using System.Collections.Generic;
using UnityEngine;

namespace Mini2
{
    // Plans a path with A* and walks the character along it.
    // A new destination can be set at any time: the new path starts where the
    // character stands, so it never jumps, and it turns toward the new heading smoothly.
    public class PathFollower : MonoBehaviour
    {
        public GridMap grid;
        public PathVisualizer visualizer;

        public float speed = 3f;         // metres per second
        public float turnSpeed = 540f;   // degrees per second

        [Tooltip("Walking speed while facing away from the next waypoint, as a fraction of speed.")]
        [Range(0f, 1f)] public float turningSlowdown = 0.3f;

        public string Status { get; private set; } = "Left-click the floor to set a target.";

        readonly List<Vector3> waypoints = new List<Vector3>();
        int next;          // index of the waypoint the character is walking to
        float travelled;   // distance walked along the current path

        public bool SetDestination(Vector3 target)
        {
            if (grid == null || !grid.IsBaked) return false;

            Vector3 pos = transform.position;
            Vector2Int clickedCell = grid.WorldToCell(target);

            // the character or the click may be in a blocked cell (e.g. next to a wall), so use the closest free one
            if (!grid.NearestWalkable(grid.WorldToCell(pos), out Vector2Int start) ||
                !grid.NearestWalkable(clickedCell, out Vector2Int goal))
                return false;

            float t0 = Time.realtimeSinceStartup;
            List<Vector2Int> cells = AStar.FindPath(grid, start, goal);
            float ms = (Time.realtimeSinceStartup - t0) * 1000f;

            if (cells == null)
            {
                Status = "No path to that point.";
                return false;
            }

            // waypoint 0 is the current position; the start cell's centre is skipped
            // so the character does not step back to it.
            waypoints.Clear();
            waypoints.Add(pos);
            for (int i = 1; i < cells.Count; i++)
                waypoints.Add(Flat(grid.CellToWorld(cells[i]), pos.y));

            // finish exactly on the clicked point when it was free, else on the closest free cell
            Vector3 end = Flat(goal == clickedCell ? target : grid.CellToWorld(goal), pos.y);
            if (cells.Count > 1) waypoints[waypoints.Count - 1] = end;
            else waypoints.Add(end);

            next = 1;
            travelled = 0f;

            int dots = visualizer != null ? visualizer.Show(waypoints) : 0;
            Status = $"A*: {cells.Count} cells, {PathLength():0.0} m, {dots} spheres, {ms:0.00} ms";
            return true;
        }

        void Update()
        {
            if (next >= waypoints.Count) return;

            Vector3 pos = transform.position;
            Vector3 to = waypoints[next] - pos;
            to.y = 0f;
            float dist = to.magnitude;

            if (dist < 0.001f)
            {
                Advance();
                return;
            }

            Vector3 dir = to / dist;

            // turn at a limited rate, never snap
            Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);

            // walk slower while still turning toward the waypoint
            float facing = Mathf.Clamp01(Vector3.Dot(transform.forward, dir));
            float step = speed * Mathf.Lerp(turningSlowdown, 1f, facing) * Time.deltaTime;

            float move = Mathf.Min(step, dist);
            transform.position = pos + dir * move;
            travelled += move;

            if (visualizer != null) visualizer.HidePassed(travelled);
            if (move >= dist) Advance();
        }

        void Advance()
        {
            next++;
            if (next >= waypoints.Count && visualizer != null) visualizer.Clear();
        }

        float PathLength()
        {
            float total = 0f;
            for (int i = 1; i < waypoints.Count; i++) total += Vector3.Distance(waypoints[i - 1], waypoints[i]);
            return total;
        }

        static Vector3 Flat(Vector3 p, float y)
        {
            return new Vector3(p.x, y, p.z);
        }

        // Scene view: remaining path in green.
        void OnDrawGizmos()
        {
            if (next >= waypoints.Count) return;
            Gizmos.color = Color.green;
            Vector3 prev = transform.position;
            for (int i = next; i < waypoints.Count; i++)
            {
                Gizmos.DrawLine(prev + Vector3.up * 0.1f, waypoints[i] + Vector3.up * 0.1f);
                prev = waypoints[i];
            }
        }
    }
}
