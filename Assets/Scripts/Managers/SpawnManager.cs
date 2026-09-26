using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    const float MAX_X = 100;
    const float MIN_X = -100;
    const float MAX_Z = 100;
    const float MIN_Z = -100;

    GameObject[] obstaclePrefabs;
    GameObject[] targetPrefabs;
    List<Obstacle> obstacles;
    List<Target> targets;

    float wallMargin = 2;

    public void Init()
    {
        obstacles = new List<Obstacle>();
        targets = new List<Target>();

        LoadPrefabs();
    }

    public void SpawnTargets(int amount, out int spawnedAmount)
    {
        spawnedAmount = 0;
        for (int i = 0; i < amount; i++)
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
            spawnedAmount++;
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
        return new Vector3(
            Random.Range(MIN_X + wallMargin, MAX_X - wallMargin),
            Random.Range(1, 5),
            Random.Range(MIN_Z + wallMargin, MAX_Z - wallMargin)
        );
    }

    private Quaternion GetRandomRot()
    {
        // TODO
        return Quaternion.identity;
    }
}
