using UnityEngine;

namespace Mini2
{
    // Splits the floor into square cells and marks each one walkable or blocked.
    // The grid is centred on this object's position, on the XZ plane.
    public class GridMap : MonoBehaviour
    {
        public Vector2Int size = new Vector2Int(40, 40); // cells in x and z
        public float cellSize = 1f;

        [Tooltip("Colliders on these layers block a cell.")]
        public LayerMask obstacleMask = ~0;

        [Tooltip("Obstacles are grown by this much, so the character does not clip corners.")]
        public float agentRadius = 0.3f;

        [Tooltip("Only colliders between 0.05 and this height block a cell (the floor is ignored).")]
        public float checkHeight = 1f;

        bool[,] walkable;

        public bool IsBaked => walkable != null;

        // Runs after every Awake, so the obstacles from EnvironmentBuilder already exist.
        void Start()
        {
            Bake();
        }

        public void Bake()
        {
            Physics.SyncTransforms(); // objects spawned this frame are not in the physics scene yet
            walkable = new bool[size.x, size.y];

            float half = cellSize * 0.5f + agentRadius;
            var halfExtents = new Vector3(half, (checkHeight - 0.05f) * 0.5f, half);

            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.y; z++)
                {
                    Vector3 c = CellToWorld(new Vector2Int(x, z));
                    c.y = transform.position.y + 0.05f + halfExtents.y;
                    walkable[x, z] = !Physics.CheckBox(c, halfExtents, Quaternion.identity, obstacleMask, QueryTriggerInteraction.Ignore);
                }
            }
        }

        public bool InBounds(Vector2Int c)
        {
            return c.x >= 0 && c.y >= 0 && c.x < size.x && c.y < size.y;
        }

        public bool IsWalkable(Vector2Int c)
        {
            return InBounds(c) && walkable[c.x, c.y];
        }

        // Bottom-left corner of the grid in world space.
        Vector3 Origin => transform.position - new Vector3(size.x, 0f, size.y) * cellSize * 0.5f;

        public Vector2Int WorldToCell(Vector3 p)
        {
            Vector3 local = p - Origin;
            int x = Mathf.Clamp(Mathf.FloorToInt(local.x / cellSize), 0, size.x - 1);
            int z = Mathf.Clamp(Mathf.FloorToInt(local.z / cellSize), 0, size.y - 1);
            return new Vector2Int(x, z);
        }

        public Vector3 CellToWorld(Vector2Int c)
        {
            return Origin + new Vector3((c.x + 0.5f) * cellSize, 0f, (c.y + 0.5f) * cellSize);
        }

        // Closest walkable cell, searching outward in square rings. Returns false if none.
        public bool NearestWalkable(Vector2Int from, out Vector2Int result)
        {
            int maxRing = Mathf.Max(size.x, size.y);
            for (int r = 0; r <= maxRing; r++)
            {
                float best = float.MaxValue;
                result = from;
                for (int dx = -r; dx <= r; dx++)
                {
                    for (int dz = -r; dz <= r; dz++)
                    {
                        if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dz)) != r) continue; // ring edge only
                        var c = new Vector2Int(from.x + dx, from.y + dz);
                        if (!IsWalkable(c)) continue;
                        float d = dx * dx + dz * dz;
                        if (d < best) { best = d; result = c; }
                    }
                }
                if (best < float.MaxValue) return true;
            }
            result = from;
            return false;
        }

        // Scene view: grid outline, red = blocked cells (after Play has baked the grid).
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Vector3 centre = transform.position + Vector3.up * 0.02f;
            Gizmos.DrawWireCube(centre, new Vector3(size.x, 0f, size.y) * cellSize);

            if (walkable == null) return;
            Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
            var cube = new Vector3(cellSize * 0.9f, 0.05f, cellSize * 0.9f);
            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.y; z++)
                    if (!walkable[x, z])
                        Gizmos.DrawCube(CellToWorld(new Vector2Int(x, z)) + Vector3.up * 0.05f, cube);
        }
    }
}
