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
    const float TARGET_SPAWN_Y = 0.5f;

    public int TotalObstacleCount => obstacles != null ? obstacles.Count : 0;
    public int TotalTargetCount => targets != null ? targets.Count : 0;
    public int TargetCount { get; private set; }

    GameObject[] obstaclePrefabs;
    GameObject[] targetPrefabs;
    List<Obstacle> obstacles;
    List<Target> targets;

    float cellLength;
    List<Vector2> cellArray;

    public void Init()
    {
        obstacles = new List<Obstacle>();
        targets = new List<Target>();

        LoadPrefabs();
        GenerateCells();

        GameplayEvents.TargetScores += OnTargetScores;
    }

    public void SpawnTargets(int count, out int spawnedCount)
    {
        // shuffle cellArray
        for (int i = cellArray.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (cellArray[i], cellArray[j]) = (cellArray[j], cellArray[i]);
        }

        float margin = SPWAN_POS_MARGIN / 2;
        spawnedCount = 0;

        foreach (Vector2 rawPos in cellArray.GetRange(0, count))
        {
            float minX = rawPos.x + margin;
            float maxX = rawPos.x + cellLength - margin;
            float minZ = rawPos.y + margin;
            float maxZ = rawPos.y + cellLength - margin;

            Vector3 pos = new Vector3(Random.Range(minX, maxX), TARGET_SPAWN_Y, Random.Range(minZ, maxZ));
            Quaternion rot = GetRandomRot(true);
            int pickId = (int)(Random.value * targetPrefabs.Length);
            GameObject obj = Instantiate(targetPrefabs[pickId], pos, rot);

            Target comp = obj.GetComponent<Target>();
            if (comp == null)
            {
                Destroy(obj);
                continue;
            }

            targets.Add(comp);
            spawnedCount++;
        }

        TargetCount = spawnedCount;
    }

    public void SpawnObstacles(int amount, out int spawnedAmount)
    {
        spawnedAmount = 0;
        for (int i = 0; i < amount; i++)
        {
            int pickId = (int)(Random.value * obstaclePrefabs.Length);
            GameObject obj = Instantiate(obstaclePrefabs[pickId], GetRandomPos(), GetRandomRot());

            Obstacle comp = obj.GetComponent<Obstacle>();
            if (comp == null)
            {
                Destroy(obj);
                continue;
            }

            obstacles.Add(comp);
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

        foreach (Obstacle obj in obstacles)
        {
            if (obj == null)
                continue;

            Destroy(obj.gameObject);
        }
        obstacles.Clear();

        TargetCount = 0;
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

        obstaclePrefabs = Resources.LoadAll<GameObject>(obstablesDir);
        targetPrefabs = Resources.LoadAll<GameObject>(targetsDir);
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

        y = Random.Range(1, 5);

        return new Vector3(x, y, z);
    }

    private Quaternion GetRandomRot(bool onlyYAxis = false)
    {
        // TODO
        return Quaternion.identity;
    }
}
