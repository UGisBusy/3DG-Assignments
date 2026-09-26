using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    const float WALL_MAX_X = 100;
    const float WALL_MIN_X = -100;
    const float WALL_MAX_Z = 100;
    const float WALL_MIN_Z = -100;
    const float PLATFORM_MAX_X = 5;
    const float PLATFORM_MIN_X = -5;
    const float PLATFORM_MAX_Z = 5;
    const float PLATFORM_MIN_Z = -5;

    GameObject[] obstaclePrefabs;
    GameObject[] targetPrefabs;
    List<Obstacle> obstacles;
    List<Target> targets;

    public void Init()
    {
        obstacles = new List<Obstacle>();
        targets = new List<Target>();

        LoadPrefabs();
    }

    public void SpawnTargets(int count, out int spawnedCount)
    {
        spawnedCount = 0;
        for (int i = 0; i < count; i++)
        {
            int pickId = (int)(Random.value * targetPrefabs.Length);
            GameObject obj = Instantiate(targetPrefabs[pickId], GetRandomPos(), GetRandomRot());

            Target comp = obj.GetComponent<Target>();
            if (comp == null)
            {
                Destroy(obj);
                continue;
            }

            targets.Add(comp);
            spawnedCount++;
        }
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
    }

    private void OnDestroy()
    {
        DespawnAll();
    }

    private void LoadPrefabs()
    {
        string obstablesDir = "Prefabs/Obstacles";
        string targetsDir = "Prefabs/Targets";

        obstaclePrefabs = Resources.LoadAll<GameObject>(obstablesDir);
        targetPrefabs = Resources.LoadAll<GameObject>(targetsDir);
    }

    private Vector3 GetRandomPos()
    {
        float margin = 2;
        float x, y, z;

        do
        {
            x = Random.Range(WALL_MIN_X + margin, WALL_MAX_X - margin);
            z = Random.Range(WALL_MIN_Z + margin, WALL_MAX_Z - margin);
        }
        while (
            (x > PLATFORM_MIN_X - margin) && (x < PLATFORM_MAX_X + margin) &&
            (z > PLATFORM_MIN_Z - margin) && (z < PLATFORM_MAX_Z + margin)
        );

        y = Random.Range(1, 5);

        return new Vector3(x, y, z);
    }

    private Quaternion GetRandomRot()
    {
        // TODO
        return Quaternion.identity;
    }
}
