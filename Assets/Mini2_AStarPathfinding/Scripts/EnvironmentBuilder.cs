using System.Collections.Generic;
using UnityEngine;

namespace Mini2
{
    // Adapted from mini-1's EnvironmentBuilder.
    // On Play, builds the walls from `walls` and scatters rocks and trees in loops
    // at random spots that do not overlap each other, the walls, or the start area.
    public class EnvironmentBuilder : MonoBehaviour
    {
        [System.Serializable]
        public struct Wall
        {
            public Vector2 from; // (x, z)
            public Vector2 to;
        }

        [Header("Prefabs")]
        public GameObject rockPrefab;
        public GameObject treePrefab;

        [Header("Walls")]
        public Material wallMaterial;
        public float wallHeight = 1.5f;
        public float wallThickness = 0.8f;
        public Wall[] walls =
        {
            new Wall { from = new Vector2(-20f, -8f), to = new Vector2(8f, -8f) },
            new Wall { from = new Vector2(-8f, 2f), to = new Vector2(20f, 2f) },
            new Wall { from = new Vector2(-20f, 11f), to = new Vector2(8f, 11f) },
            new Wall { from = new Vector2(12f, -20f), to = new Vector2(12f, -13f) },
            new Wall { from = new Vector2(-12f, -8f), to = new Vector2(-12f, -3f) },
            new Wall { from = new Vector2(0f, 14f), to = new Vector2(0f, 20f) },
        };

        [Header("Area")]
        public float halfSize = 19f;                        // objects are placed in [-halfSize, halfSize]
        public Vector2 keepClear = new Vector2(0f, -16f);   // the character's start spot (x, z)
        public float keepClearRadius = 2.5f;

        [Header("Rocks")]
        public int rocks = 35;
        public Vector2 rockScale = new Vector2(1f, 2.5f);
        public float rockRadius = 0.4f;

        [Header("Trees")]
        public int trees = 25;
        public Vector2 treeScale = new Vector2(1.5f, 2.5f);
        public float treeRadius = 0.5f;

        [Header("Path check")]
        [Tooltip("Used to reject rocks / trees that would cut off part of the map. Found automatically if empty.")]
        public GridMap grid;
        public int triesPerObject = 5;

        System.Random rng;
        readonly List<Vector3> occupied = new List<Vector3>(); // (x, z, radius) of placed objects
        Vector2Int startCell;
        int unreachable; // walkable cells the character cannot reach, must never go up

        void Awake()
        {
            rng = new System.Random((int)System.DateTime.Now.Ticks);
            occupied.Add(new Vector3(keepClear.x, keepClear.y, keepClearRadius));
            if (grid == null) grid = FindFirstObjectByType<GridMap>();

            BuildWalls();

            if (grid != null)
            {
                startCell = grid.WorldToCell(transform.TransformPoint(new Vector3(keepClear.x, 0f, keepClear.y)));
                grid.Bake();
                unreachable = grid.CountUnreachable(startCell);
            }

            Scatter(rockPrefab, "Rocks", rocks, rockScale, rockRadius);
            Scatter(treePrefab, "Trees", trees, treeScale, treeRadius);
        }

        void BuildWalls()
        {
            var parent = Group("Walls");
            for (int i = 0; i < walls.Length; i++)
            {
                Vector3 a = new Vector3(walls[i].from.x, 0f, walls[i].from.y);
                Vector3 b = new Vector3(walls[i].to.x, 0f, walls[i].to.y);
                Vector3 dir = b - a;
                if (dir.sqrMagnitude < 0.0001f) continue;

                var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = $"Wall_{i:00}";
                wall.transform.SetParent(parent, false);
                // extra thickness on the length so walls that meet at a corner overlap
                wall.transform.SetLocalPositionAndRotation(
                    (a + b) * 0.5f + Vector3.up * wallHeight * 0.5f,
                    Quaternion.LookRotation(dir));
                wall.transform.localScale = new Vector3(wallThickness, wallHeight, dir.magnitude + wallThickness);
                if (wallMaterial != null) wall.GetComponent<Renderer>().sharedMaterial = wallMaterial;
            }
        }

        void Scatter(GameObject prefab, string groupName, int count, Vector2 scaleRange, float radius)
        {
            if (prefab == null) return;
            var parent = Group(groupName);

            for (int i = 0; i < count; i++)
            {
                for (int attempt = 0; attempt < triesPerObject; attempt++)
                {
                    float scale = Rand(scaleRange.x, scaleRange.y);
                    if (!FindFreeSpot(radius * scale, out Vector3 pos)) break;

                    var go = Place(prefab, parent, $"{prefab.name}_{i:00}", pos, Rand(0f, 360f), scale);
                    if (KeepsMapConnected()) break;

                    // it walled something off: remove it and try another spot
                    go.SetActive(false); // takes its colliders out of physics right away
                    Destroy(go);
                    occupied.RemoveAt(occupied.Count - 1);
                }
            }
        }

        // Re-bakes the grid and checks that no extra cells became unreachable from the start.
        bool KeepsMapConnected()
        {
            if (grid == null) return true;
            grid.Bake();
            int now = grid.CountUnreachable(startCell);
            if (now > unreachable) return false;
            unreachable = now;
            return true;
        }

        GameObject Place(GameObject prefab, Transform parent, string name, Vector3 pos, float yaw, float scale)
        {
            var go = Instantiate(prefab, parent);
            go.name = name;
            Transform p = prefab.transform;
            go.transform.SetLocalPositionAndRotation(
                new Vector3(pos.x, p.localPosition.y * scale, pos.z),
                Quaternion.Euler(0f, yaw, 0f) * p.localRotation);
            go.transform.localScale = p.localScale * scale;
            return go;
        }

        float Rand(float min, float max)
        {
            return min + (float)rng.NextDouble() * (max - min);
        }

        bool FindFreeSpot(float radius, out Vector3 pos)
        {
            for (int attempt = 0; attempt < 40; attempt++)
            {
                pos = new Vector3(Rand(-halfSize, halfSize), 0f, Rand(-halfSize, halfSize));
                if (!IsFree(pos, radius)) continue;

                occupied.Add(new Vector3(pos.x, pos.z, radius));
                return true;
            }
            pos = Vector3.zero;
            return false;
        }

        bool IsFree(Vector3 pos, float radius)
        {
            foreach (var o in occupied)
            {
                float dx = pos.x - o.x, dz = pos.z - o.y;
                if (dx * dx + dz * dz < (radius + o.z) * (radius + o.z)) return false;
            }

            // keep about one cell between objects and walls, so corridors stay open
            var p = new Vector2(pos.x, pos.z);
            float clearance = radius + wallThickness * 0.5f + 1f;
            foreach (var w in walls)
                if (DistanceToSegment(p, w.from, w.to) < clearance) return false;

            return true;
        }

        static float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float t = ab.sqrMagnitude > 0f ? Mathf.Clamp01(Vector2.Dot(p - a, ab) / ab.sqrMagnitude) : 0f;
            return Vector2.Distance(p, a + ab * t);
        }

        Transform Group(string name)
        {
            var t = new GameObject(name).transform;
            t.SetParent(transform, false);
            return t;
        }

        // Scene view: wall layout in grey, start area in green.
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.gray;
            foreach (var w in walls)
                Gizmos.DrawLine(new Vector3(w.from.x, 0.1f, w.from.y), new Vector3(w.to.x, 0.1f, w.to.y));
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(new Vector3(keepClear.x, 0f, keepClear.y), keepClearRadius);
        }
    }
}
