using UnityEditor;
using UnityEngine;

// Menu: 3DG > Build Lever Prefab
// Rebuilds Lever.prefab's look: a box base, a cylinder handle on a pivot, and a
// sphere knob on the tip. Creates the materials and wires them into Lever.
// Safe to run again; it replaces the previously built parts.
public static class LeverPrefabBuilder
{
    const string MENU_PATH = "3DG/Build Lever Prefab";
    const string PREFAB_PATH = "Assets/Resources/Level/Lever.prefab";
    const string MATERIAL_DIR = "Assets/Materials";
    const string VISUAL_NAME = "Visual";
    const float TILT_ANGLE = 30f;

    [MenuItem(MENU_PATH)]
    private static void Build()
    {
        Material baseMaterial = CreateMaterial("LeverBase", new Color(0.25f, 0.25f, 0.28f), 0f, 0.3f);
        Material handleMaterial = CreateMaterial("LeverHandle", new Color(0.75f, 0.75f, 0.78f), 0.8f, 0.6f);
        Material offMaterial = CreateMaterial("LeverKnobOff", new Color(0.85f, 0.1f, 0.1f), 0f, 0.7f);
        Material onMaterial = CreateMaterial("LeverKnobOn", new Color(0.15f, 0.8f, 0.2f), 0f, 0.7f);

        GameObject root = PrefabUtility.LoadPrefabContents(PREFAB_PATH);
        try
        {
            // the old look was the root cube itself
            Object.DestroyImmediate(root.GetComponent<MeshRenderer>());
            Object.DestroyImmediate(root.GetComponent<MeshFilter>());

            // remove parts from a previous run
            Transform oldVisual = root.transform.Find(VISUAL_NAME);
            if (oldVisual != null)
                Object.DestroyImmediate(oldVisual.gameObject);

            // root sits 0.5 above the ground, so local y -0.5 is the floor
            var visual = new GameObject(VISUAL_NAME).transform;
            visual.SetParent(root.transform, false);

            // base: wide along X, the direction the handle tilts
            CreatePart(PrimitiveType.Cube, "Base", visual,
                new Vector3(0f, -0.3f, 0f), new Vector3(1f, 0.4f, 0.6f), baseMaterial);

            // pivot on top of the base; handle and knob rotate with it
            var pivot = new GameObject("HandlePivot").transform;
            pivot.SetParent(visual, false);
            pivot.localPosition = new Vector3(0f, -0.1f, 0f);
            pivot.localRotation = Quaternion.Euler(0f, 0f, TILT_ANGLE); // off = lean left

            // Unity's cylinder is 2 units tall, so y scale 0.5 = 1 unit long
            CreatePart(PrimitiveType.Cylinder, "Handle", pivot,
                new Vector3(0f, 0.5f, 0f), new Vector3(0.12f, 0.5f, 0.12f), handleMaterial);

            Renderer knob = CreatePart(PrimitiveType.Sphere, "Knob", pivot,
                new Vector3(0f, 1.05f, 0f), Vector3.one * 0.3f, offMaterial);

            // hit box covering the base and both handle positions
            var box = root.GetComponent<BoxCollider>();
            if (box != null)
            {
                box.center = new Vector3(0f, 0.35f, 0f);
                box.size = new Vector3(1.4f, 1.7f, 0.6f);
            }

            var lever = root.GetComponent<Lever>();
            var so = new SerializedObject(lever);
            so.FindProperty("handlePivot").objectReferenceValue = pivot;
            so.FindProperty("knob").objectReferenceValue = knob;
            so.FindProperty("offMaterial").objectReferenceValue = offMaterial;
            so.FindProperty("onMaterial").objectReferenceValue = onMaterial;
            so.FindProperty("tiltAngle").floatValue = TILT_ANGLE;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PREFAB_PATH);
            AssetDatabase.SaveAssets(); // write the material assets to disk
            Debug.Log("[LeverPrefabBuilder] Lever prefab rebuilt");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    // primitive without its own collider (the root BoxCollider handles hits)
    private static Renderer CreatePart(PrimitiveType type, string name, Transform parent,
        Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        Object.DestroyImmediate(part.GetComponent<Collider>());

        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = localScale;

        var renderer = part.GetComponent<Renderer>();
        renderer.sharedMaterial = material;
        return renderer;
    }

    // creates (or updates) a URP Lit material asset
    private static Material CreateMaterial(string name, Color color, float metallic, float smoothness)
    {
        string path = $"{MATERIAL_DIR}/{name}.mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;
        material.SetFloat("_Metallic", metallic);
        material.SetFloat("_Smoothness", smoothness);
        EditorUtility.SetDirty(material);
        return material;
    }
}
