using Benjathemaker;
using UnityEditor;
using UnityEngine;

// Menu: 3DG > Build Collectible Models
// Gives our Key and Crystal pickups their look from the
// "Simple Gems and Items" pack (BTM_Assets):
//   Key     <- pack Key
//   Crystal <- pack HeartGem (renamed "Crystal")
// Our prefabs are kept (boxes and monsters reference them); only the sphere look
// is replaced by a nested copy of the pack model, keeping its spin/float/pulse animation.
// Safe to run again; it replaces the previously built model.
public static class CollectibleModelBuilder
{
    const string MENU_PATH = "3DG/Build Collectible Models";
    const string PACK_DIR = "Assets/BTM_Assets/BTM_Items_Gems/Prefabs";
    const string COLLECTIBLE_DIR = "Assets/Resources/Collectibles";
    const string VISUAL_NAME = "Visual";

    [MenuItem(MENU_PATH)]
    private static void Build()
    {
        Rebuild($"{COLLECTIBLE_DIR}/Key.prefab", $"{PACK_DIR}/Key.prefab", "Key", size: 0.6f);
        Rebuild($"{COLLECTIBLE_DIR}/Crystal.prefab", $"{PACK_DIR}/HeartGem.prefab", "Crystal", size: 0.5f);
        AssetDatabase.SaveAssets();
    }

    // size: the model's longest side, in world units
    private static void Rebuild(string prefabPath, string modelPath, string modelName, float size)
    {
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        if (model == null)
        {
            Debug.LogError($"[CollectibleModelBuilder] Missing pack model: {modelPath}");
            return;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            // old look: the root was a scaled-down sphere
            Object.DestroyImmediate(root.GetComponent<MeshRenderer>());
            Object.DestroyImmediate(root.GetComponent<MeshFilter>());
            Transform oldVisual = root.transform.Find(VISUAL_NAME);
            if (oldVisual != null)
                Object.DestroyImmediate(oldVisual.gameObject);

            root.transform.localScale = Vector3.one;

            // pickup trigger, a bit bigger than the model so it's easy to collect
            var trigger = root.GetComponent<SphereCollider>();
            if (trigger != null)
            {
                trigger.center = Vector3.zero;
                trigger.radius = 0.5f;
            }

            // Visual holds the size; the pack model stays at scale 1 because its
            // pulse animation works out its target size from its own starting scale
            var visual = new GameObject(VISUAL_NAME).transform;
            visual.SetParent(root.transform, false);

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(model, visual);
            instance.name = modelName;
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one;

            // shrink to the wanted size, then center the model on the pickup
            Bounds bounds = RendererBounds(instance);
            float longest = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            if (longest > 0f)
                visual.localScale = Vector3.one * (size / longest);
            bounds = RendererBounds(instance);
            instance.transform.position += root.transform.position - bounds.center;

            // the pack floats ~2 units up; keep the bob small for our small item
            var anim = instance.GetComponent<SimpleGemsAnim>();
            if (anim != null)
            {
                anim.floatHeight = 0.25f;
                PrefabUtility.RecordPrefabInstancePropertyModifications(anim);
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Debug.Log($"[CollectibleModelBuilder] {prefabPath} now uses {modelPath}");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static Bounds RendererBounds(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(target.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);
        return bounds;
    }
}
