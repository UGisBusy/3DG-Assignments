using System.IO;
using UnityEditor;
using UnityEngine;

public static class SpawnPrefabBuilder
{
    const string MENU_PATH = "3DG/Build Target And Obstacle Prefabs";

    const string TARGET_DIR = "Assets/Resources/Prefabs/Targets";
    const string OBSTACLE_DIR = "Assets/Resources/Prefabs/Obstacles";
    const string TEMPLATE_PARENT_DIR = "Assets/Prefabs";
    const string TEMPLATE_DIR = TEMPLATE_PARENT_DIR + "/Templates";
    const string TARGET_TEMPLATE = "TargetPrefab";
    const string OBSTACLE_TEMPLATE = "ObstaclePrefabR";
    static readonly string[] targetCubePrefabs = { "TargetPrefab" };
    static readonly string[] obstacleCubePrefabs = { "ObstaclePrefabR", "ObstaclePrefabG", "ObstaclePrefabB" };

    const string FISH_DIR = "Assets/PixitGames/Pixit3D/PixitUnderseaCollection/LowPolyFishFree/Prefabs";
    const string ROCK_DIR = "Assets/JC_StylizedRocks_Lite/Prefabs";
    const string ROCK_MATERIAL_DIR = "Assets/JC_StylizedRocks_Lite/Materials";
    const string URP_LIT_SHADER = "Universal Render Pipeline/Lit";

    const string TARGET_PREFIX = "Target_";
    const string OBSTACLE_PREFIX = "Obstacle_";
    const float TARGET_SIZE = 1.2f;
    const float OBSTACLE_SIZE = 1.6f;
    const float MIN_COLLIDER_SIZE = 0.6f;
    const float MODEL_BOTTOM_Y = -0.5f;
    const float TARGET_SINK = 0f;
    const float OBSTACLE_SINK = 0.1f;

    [MenuItem(MENU_PATH)]
    private static void Build()
    {
        ConvertMaterialsToUrp(ROCK_MATERIAL_DIR);

        MoveToTemplates(TARGET_DIR, targetCubePrefabs);
        MoveToTemplates(OBSTACLE_DIR, obstacleCubePrefabs);

        GameObject targetTemplate = LoadPrefab($"{TEMPLATE_DIR}/{TARGET_TEMPLATE}.prefab");
        GameObject obstacleTemplate = LoadPrefab($"{TEMPLATE_DIR}/{OBSTACLE_TEMPLATE}.prefab");

        DeleteGenerated(TARGET_DIR, TARGET_PREFIX);
        DeleteGenerated(OBSTACLE_DIR, OBSTACLE_PREFIX);

        int targetCount = BuildAll(FISH_DIR, targetTemplate, TARGET_DIR, TARGET_PREFIX, TARGET_SIZE, TARGET_SINK);
        int obstacleCount = BuildAll(ROCK_DIR, obstacleTemplate, OBSTACLE_DIR, OBSTACLE_PREFIX, OBSTACLE_SIZE, OBSTACLE_SINK);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[SpawnPrefabBuilder] built {targetCount} target prefabs and {obstacleCount} obstacle prefabs");
    }

    private static void ConvertMaterialsToUrp(string dir)
    {
        Shader lit = Shader.Find(URP_LIT_SHADER);
        if (lit == null)
            throw new System.Exception($"[SpawnPrefabBuilder] shader '{URP_LIT_SHADER}' not found");

        foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { dir }))
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
            if (material.shader == lit)
                continue;

            Texture baseMap = material.HasProperty("_MainTex") ? material.GetTexture("_MainTex") : null;
            Texture normalMap = material.HasProperty("_BumpMap") ? material.GetTexture("_BumpMap") : null;
            Color color = material.HasProperty("_Color") ? material.GetColor("_Color") : Color.white;
            float smoothness = material.HasProperty("_Glossiness") ? material.GetFloat("_Glossiness") : 0f;

            material.shader = lit;
            material.SetTexture("_BaseMap", baseMap);
            material.SetColor("_BaseColor", color);
            material.SetFloat("_Smoothness", smoothness);

            if (normalMap != null)
            {
                material.SetTexture("_BumpMap", normalMap);
                material.EnableKeyword("_NORMALMAP");
            }

            EditorUtility.SetDirty(material);
        }
    }

    private static void MoveToTemplates(string dir, string[] names)
    {
        if (!AssetDatabase.IsValidFolder(TEMPLATE_PARENT_DIR))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder(TEMPLATE_DIR))
            AssetDatabase.CreateFolder(TEMPLATE_PARENT_DIR, "Templates");

        foreach (string name in names)
        {
            string from = $"{dir}/{name}.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(from) == null)
                continue;

            string error = AssetDatabase.MoveAsset(from, $"{TEMPLATE_DIR}/{name}.prefab");
            if (!string.IsNullOrEmpty(error))
                throw new System.Exception($"[SpawnPrefabBuilder] could not move {from}: {error}");
        }
    }

    private static GameObject LoadPrefab(string path)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
            throw new System.Exception($"[SpawnPrefabBuilder] prefab not found at {path}");
        return prefab;
    }

    private static void DeleteGenerated(string dir, string prefix)
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { dir }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileName(path).StartsWith(prefix))
                AssetDatabase.DeleteAsset(path);
        }
    }

    private static int BuildAll(string sourceDir, GameObject template, string outputDir, string prefix, float size, float sink)
    {
        int count = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { sourceDir }))
        {
            GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            string name = prefix + source.name.Replace("_Rigged_Unity", "").Replace("_Static_Unity", "");
            BuildPrefab(template, source, $"{outputDir}/{name}.prefab", name, size, sink);
            count++;
        }
        return count;
    }

    private static void BuildPrefab(GameObject template, GameObject source, string path, string name, float size, float sink)
    {
        GameObject root = Object.Instantiate(template);
        root.name = name;
        root.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        Object.DestroyImmediate(root.GetComponent<MeshRenderer>());
        Object.DestroyImmediate(root.GetComponent<MeshFilter>());

        GameObject model = CreateStaticModel(source);
        model.transform.SetParent(root.transform, false);
        FitModel(model, size, MODEL_BOTTOM_Y - sink);
        FitCollider(root.GetComponent<BoxCollider>(), model, MODEL_BOTTOM_Y);

        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }

    private static GameObject CreateStaticModel(GameObject source)
    {
        GameObject clone = Object.Instantiate(source);
        clone.transform.position = Vector3.zero;

        GameObject model = new GameObject("Model");

        foreach (Renderer renderer in clone.GetComponentsInChildren<Renderer>())
        {
            Mesh mesh = GetMesh(renderer);
            if (mesh == null || !renderer.enabled)
                continue;

            GameObject part = new GameObject(renderer.name);
            part.transform.SetParent(model.transform, false);
            part.transform.SetPositionAndRotation(renderer.transform.position, renderer.transform.rotation);
            part.transform.localScale = renderer.transform.lossyScale;

            part.AddComponent<MeshFilter>().sharedMesh = mesh;
            part.AddComponent<MeshRenderer>().sharedMaterials = renderer.sharedMaterials;
        }

        Object.DestroyImmediate(clone);
        return model;
    }

    private static Mesh GetMesh(Renderer renderer)
    {
        if (renderer is SkinnedMeshRenderer skinned)
            return skinned.sharedMesh;

        MeshFilter filter = renderer.GetComponent<MeshFilter>();
        return filter != null ? filter.sharedMesh : null;
    }

    private static void FitModel(GameObject model, float size, float bottomY)
    {
        Bounds bounds = CalculateBounds(model);
        float largest = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        model.transform.localScale = Vector3.one * (size / largest);

        bounds = CalculateBounds(model);
        model.transform.localPosition = new Vector3(-bounds.center.x, bottomY - bounds.min.y, -bounds.center.z);
    }

    private static void FitCollider(BoxCollider collider, GameObject model, float bottomY)
    {
        Bounds bounds = CalculateBounds(model);
        Vector3 size = Vector3.Max(new Vector3(bounds.size.x, bounds.max.y - bottomY, bounds.size.z), Vector3.one * MIN_COLLIDER_SIZE);

        collider.size = size;
        collider.center = new Vector3(bounds.center.x, bottomY + size.y / 2f, bounds.center.z);
    }

    private static Bounds CalculateBounds(GameObject model)
    {
        bool hasBounds = false;
        Bounds bounds = new Bounds();

        foreach (MeshFilter filter in model.GetComponentsInChildren<MeshFilter>())
        {
            Bounds local = filter.sharedMesh.bounds;
            Matrix4x4 matrix = filter.transform.localToWorldMatrix;

            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = local.center + Vector3.Scale(local.extents, new Vector3(
                    (i & 1) == 0 ? -1 : 1,
                    (i & 2) == 0 ? -1 : 1,
                    (i & 4) == 0 ? -1 : 1));
                Vector3 point = matrix.MultiplyPoint3x4(corner);

                if (!hasBounds)
                {
                    bounds = new Bounds(point, Vector3.zero);
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(point);
                }
            }
        }

        return bounds;
    }
}
