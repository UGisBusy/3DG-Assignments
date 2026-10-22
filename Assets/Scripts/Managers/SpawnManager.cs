using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    const float WALL_MAX_X = 100;
    const float WALL_MIN_X = -100;
    const float WALL_MAX_Z = 100;
    const float WALL_MIN_Z = -100;
    const float PLATFORM_MAX_X = 10;
    const float PLATFORM_MIN_X = -10;
    const float PLATFORM_MAX_Z = 10;
    const float PLATFORM_MIN_Z = -10;
    const float SPWAN_POS_MARGIN = 2;
    const float SPAWN_Y = 0.5f;
    const float SPAWN_GAP = 0.5f;
    const float DEFAULT_FOOTPRINT_RADIUS = 0.5f;
    const int MAX_SPAWN_ATTEMPTS = 30;

    struct SpawnSlot
    {
        public Vector2 Position;
        public float Radius;
    }

    public int TotalObstacleCount => obstacles != null ? obstacles.Count : 0;
    public int TotalTargetCount => targets != null ? targets.Count : 0;
    public int TargetCount { get; private set; }

    GameObject[] obstaclePrefabs;
    GameObject[] targetPrefabs;
    GameObject[] collectiblePrefabs;
    List<Obstacle> obstacles;
    List<Target> targets;
    List<Collectible> collectibles;

    float cellLength;
    List<Vector2> cellArray;
    List<SpawnSlot> occupiedSlots;
    Dictionary<GameObject, float> footprintRadii;

    public void Init()
    {
        obstacles = new List<Obstacle>();
        targets = new List<Target>();
        collectibles = new List<Collectible>();
        occupiedSlots = new List<SpawnSlot>();
        footprintRadii = new Dictionary<GameObject, float>();

        LoadPrefabs();
        GenerateCells();

        GameplayEvents.TargetScores += OnTargetScores;
    }

    public void SpawnTargets(int count, out int spawnedCount)
    {
        ShuffleCells();

        spawnedCount = 0;
        foreach (Vector2 cell in cellArray.GetRange(0, Mathf.Min(count, cellArray.Count)))
        {
            Target comp = TrySpawn<Target>(targetPrefabs, () => GetRandomPosInCell(cell), GetRandomRot(true));
            if (comp == null)
                continue;

            targets.Add(comp);
            spawnedCount++;
        }

        TargetCount += spawnedCount;
    }

    public void SpawnObstacles(int amount, out int spawnedAmount)
    {
        spawnedAmount = 0;
        for (int i = 0; i < amount; i++)
        {
            Obstacle comp = TrySpawn<Obstacle>(obstaclePrefabs, GetRandomPos, GetRandomRot());
            if (comp == null)
                continue;

            obstacles.Add(comp);
            spawnedAmount++;
        }
    }

    public void SpawnCollectibles(int amount, out int spawnedAmount)
    {
        spawnedAmount = 0;
        for (int i = 0; i < amount; i++)
        {
            Collectible comp = TrySpawn<Collectible>(collectiblePrefabs, GetRandomPos, Quaternion.identity);
            if (comp == null)
                continue;

            collectibles.Add(comp);
            spawnedAmount++;
        }
    }

    public void DespawnAll()
    {
        foreach (Target target in targets)
        {
            if (target == null)
                continue;
            Destroy(target.gameObject);
        }
        targets.Clear();
        TargetCount = 0;

        foreach (Obstacle obstacle in obstacles)
        {
            if (obstacle == null)
                continue;
            Destroy(obstacle.gameObject);
        }
        obstacles.Clear();

        foreach (Collectible collectible in collectibles)
        {
            if (collectible == null)
                continue;
            Destroy(collectible.gameObject);
        }
        collectibles.Clear();

        occupiedSlots.Clear();
    }

    private void OnDestroy()
    {
        DespawnAll();
        GameplayEvents.TargetScores -= OnTargetScores;
    }

    private void OnTargetScores()
    {
        TargetCount--;
    }

    private void LoadPrefabs()
    {
        string obstablesDir = "Prefabs/Obstacles";
        string targetsDir = "Prefabs/Targets";
        string collectibleDir = "Prefabs/Collectibles";

        obstaclePrefabs = Resources.LoadAll<GameObject>(obstablesDir);
        targetPrefabs = Resources.LoadAll<GameObject>(targetsDir);
        collectiblePrefabs = Resources.LoadAll<GameObject>(collectibleDir);
    }

    private void GenerateCells()
    {
        float margin = SPWAN_POS_MARGIN / 2;
        float minX = WALL_MIN_X + margin;
        float maxX = WALL_MAX_X - margin;
        float minZ = WALL_MIN_Z + margin;
        float maxZ = WALL_MAX_Z - margin;

        Rect platform = Rect.MinMaxRect(PLATFORM_MIN_X - margin, PLATFORM_MIN_Z - margin, PLATFORM_MAX_X + margin, PLATFORM_MAX_Z + margin);

        // generate 30*30=900 total cells (will exclude some cells inside of platform)
        cellLength = (maxX - minX) / 30;
        cellArray = new List<Vector2>();

        for (float x = minX; x <= maxX - cellLength; x += cellLength)
        {
            for (float z = minZ; z <= maxZ - cellLength; z += cellLength)
            {
                if (!platform.Contains(new Vector2(x, z)))
                    cellArray.Add(new Vector2(x, z));
            }
        }
    }

    private T TrySpawn<T>(GameObject[] prefabs, System.Func<Vector3> samplePosition, Quaternion rotation) where T : Component
    {
        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        float radius = GetFootprintRadius(prefab);

        if (!TryFindFreePosition(samplePosition, radius, out Vector3 position))
            return null;

        GameObject obj = Instantiate(prefab, position, rotation);
        T comp = obj.GetComponent<T>();
        if (comp == null)
        {
            Destroy(obj);
            return null;
        }

        occupiedSlots.Add(new SpawnSlot { Position = new Vector2(position.x, position.z), Radius = radius });
        return comp;
    }

    private bool TryFindFreePosition(System.Func<Vector3> samplePosition, float radius, out Vector3 position)
    {
        for (int i = 0; i < MAX_SPAWN_ATTEMPTS; i++)
        {
            position = samplePosition();
            if (IsPositionFree(position, radius))
                return true;
        }

        position = Vector3.zero;
        return false;
    }

    private bool IsPositionFree(Vector3 position, float radius)
    {
        Vector2 flatPosition = new Vector2(position.x, position.z);
        foreach (SpawnSlot slot in occupiedSlots)
        {
            if (Vector2.Distance(flatPosition, slot.Position) < radius + slot.Radius + SPAWN_GAP)
                return false;
        }

        Collider[] hits = Physics.OverlapSphere(position, radius, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Ground"))
                return false;
        }

        return true;
    }

    private float GetFootprintRadius(GameObject prefab)
    {
        if (footprintRadii.TryGetValue(prefab, out float cached))
            return cached;

        Vector3 scale = prefab.transform.localScale;
        float horizontalScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
        float radius;

        switch (prefab.GetComponent<Collider>())
        {
            case BoxCollider box:
                Vector2 halfSize = new Vector2(box.size.x * scale.x, box.size.z * scale.z) * 0.5f;
                Vector2 offset = new Vector2(box.center.x * scale.x, box.center.z * scale.z);
                radius = halfSize.magnitude + offset.magnitude;
                break;
            case SphereCollider sphere:
                radius = sphere.radius * horizontalScale;
                break;
            case CapsuleCollider capsule:
                radius = (capsule.direction == 1 ? capsule.radius : capsule.height / 2f) * horizontalScale;
                break;
            default:
                radius = DEFAULT_FOOTPRINT_RADIUS;
                break;
        }

        footprintRadii[prefab] = radius;
        return radius;
    }

    private void ShuffleCells()
    {
        for (int i = cellArray.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (cellArray[i], cellArray[j]) = (cellArray[j], cellArray[i]);
        }
    }

    private Vector3 GetRandomPosInCell(Vector2 cell)
    {
        float margin = SPWAN_POS_MARGIN / 2;
        float x = Random.Range(cell.x + margin, cell.x + cellLength - margin);
        float z = Random.Range(cell.y + margin, cell.y + cellLength - margin);
        return new Vector3(x, SPAWN_Y, z);
    }

    private Vector3 GetRandomPos()
    {
        float x, y, z;
        Rect platform = Rect.MinMaxRect(
            PLATFORM_MIN_X - SPWAN_POS_MARGIN, PLATFORM_MIN_Z - SPWAN_POS_MARGIN,
            PLATFORM_MAX_X + SPWAN_POS_MARGIN, PLATFORM_MAX_Z + SPWAN_POS_MARGIN);

        do
        {
            x = Random.Range(WALL_MIN_X + SPWAN_POS_MARGIN, WALL_MAX_X - SPWAN_POS_MARGIN);
            z = Random.Range(WALL_MIN_Z + SPWAN_POS_MARGIN, WALL_MAX_Z - SPWAN_POS_MARGIN);
        }
        while (platform.Contains(new Vector2(x, z)));

        // y = Random.Range(1, 5);
        y = SPAWN_Y;

        return new Vector3(x, y, z);
    }

    private Quaternion GetRandomRot(bool onlyYAxis = false)
    {
        // TODO
        return Quaternion.identity;
    }
}
