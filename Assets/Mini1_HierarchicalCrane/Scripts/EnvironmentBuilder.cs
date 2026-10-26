using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Places prefabs around this object's position when Play starts, using loops:
/// a ring of fence posts, then rocks and trees at random spots that do not overlap.
/// Attach to an empty GameObject at the crane's position (on top of your ground).
/// </summary>
public class EnvironmentBuilder : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] fencePrefabs;
    public GameObject rockPrefab;
    public GameObject treePrefab;

    [Header("Fence")]
    public int fencePosts = 28;
    public float fenceRadius = 7.5f;

    [Header("Rocks")]
    public int rocks = 45;
    public Vector2 rockRing = new Vector2(11f, 40f);
    public Vector2 rockScale = new Vector2(1f, 4f);
    public float rockRadius = 0.4f;

    [Header("Trees")]
    public int trees = 35;
    public Vector2 treeRing = new Vector2(20f, 48f);
    public Vector2 treeScale = new Vector2(1.5f, 3f);
    public float treeRadius = 0.6f;

    System.Random rng;
    readonly List<Vector3> occupied = new List<Vector3>(); // (x, z, radius) of placed objects

    void Awake()
    {
        rng = new System.Random((int)System.DateTime.Now.Ticks);
        occupied.Add(new Vector3(0f, 0f, fenceRadius + 1f)); // keep the crane area clear

        BuildFence();
        Scatter(rockPrefab, "Rocks", rocks, rockRing, rockScale, rockRadius);
        Scatter(treePrefab, "Trees", trees, treeRing, treeScale, treeRadius);
    }

    void BuildFence()
    {
        if (fencePrefabs == null || fencePrefabs.Length == 0) return;
        var parent = Group("Fence");

        for (int i = 0; i < fencePosts; i++)
        {
            float a = i * Mathf.PI * 2f / fencePosts;
            var prefab = fencePrefabs[i % fencePrefabs.Length];
            Place(prefab, parent, $"Post_{i:00}", new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * fenceRadius, 0f, 1f);
        }
    }

    void Scatter(GameObject prefab, string groupName, int count, Vector2 ring, Vector2 scaleRange, float radius)
    {
        if (prefab == null) return;
        var parent = Group(groupName);

        for (int i = 0; i < count; i++)
        {
            float scale = Rand(scaleRange.x, scaleRange.y);
            if (!FindFreeSpot(ring.x, ring.y, radius * scale, out Vector3 pos)) continue;
            Place(prefab, parent, $"{prefab.name}_{i:00}", pos, Rand(0f, 360f), scale);
        }
    }

    void Place(GameObject prefab, Transform parent, string name, Vector3 pos, float yaw, float scale)
    {
        var go = Instantiate(prefab, parent);
        go.name = name;
        Transform p = prefab.transform;
        go.transform.SetLocalPositionAndRotation(
            new Vector3(pos.x, p.localPosition.y * scale, pos.z),
            Quaternion.Euler(0f, yaw, 0f) * p.localRotation);
        go.transform.localScale = p.localScale * scale;
    }

    float Rand(float min, float max)
    {
        return min + (float)rng.NextDouble() * (max - min);
    }


    bool FindFreeSpot(float minR, float maxR, float radius, out Vector3 pos)
    {
        for (int attempt = 0; attempt < 40; attempt++)
        {
            float a = Rand(0f, Mathf.PI * 2f);
            float d = Mathf.Sqrt(Rand(minR * minR, maxR * maxR));
            pos = new Vector3(Mathf.Cos(a) * d, 0f, Mathf.Sin(a) * d);

            bool free = true;
            foreach (var o in occupied)
            {
                float dx = pos.x - o.x, dz = pos.z - o.y;
                if (dx * dx + dz * dz < (radius + o.z) * (radius + o.z)) { free = false; break; }
            }
            if (!free) continue;

            occupied.Add(new Vector3(pos.x, pos.z, radius));
            return true;
        }
        pos = Vector3.zero;
        return false;
    }

    Transform Group(string name)
    {
        var t = new GameObject(name).transform;
        t.SetParent(transform, false);
        return t;
    }
}
