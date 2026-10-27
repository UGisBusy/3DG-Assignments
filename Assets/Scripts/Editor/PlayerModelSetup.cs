using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PlayerModelSetup
{
    const string MENU_PATH = "3DG/Setup Player Model";
    const string PACK_DIR = "Assets/Haons SD series Pack";
    const string CHARACTER_PREFAB =
        PACK_DIR + "/Prefab/CharacterSet/prf_Set Costume01 Unity-Chan.prefab";
    const string IDLE_FBX = PACK_DIR + "/Animation/Common/StandA_idleA.FBX";
    const string WALK_FBX = PACK_DIR + "/Animation/Common/WalkA_Front.FBX";
    const string RUN_FBX = PACK_DIR + "/Animation/Common/RunA_Front.FBX";
    const string IDLE_CLIP = "StandA_idleA";
    const string WALK_CLIP = "Walk_front@loop";
    const string RUN_CLIP = "RunA_front@loop";

    const string CONTROLLER_DIR = "Assets/Resources/Animations";
    const string CONTROLLER_PATH = CONTROLLER_DIR + "/PlayerAnimator.controller";

    const string MODEL_NAME = "Model";
    const float MODEL_NATIVE_HEIGHT = 1.0f;

    const float WALK_THRESHOLD = 2f;
    const float RUN_THRESHOLD = 6f;

    [MenuItem(MENU_PATH)]
    private static void Setup()
    {
        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();
        if (player == null)
        {
            EditorUtility.DisplayDialog(
                "Setup Player Model",
                "No PlayerMovement found in the open scene.",
                "OK"
            );
            return;
        }

        CreateController();
        PlaceModel(player);

        EditorSceneManager.MarkSceneDirty(player.gameObject.scene);
        Debug.Log("[PlayerModelSetup] player model ready, save the scene to keep it");
    }

    private static void CreateController()
    {
        if (!AssetDatabase.IsValidFolder(CONTROLLER_DIR))
            AssetDatabase.CreateFolder("Assets/Resources", "Animations");

        AssetDatabase.DeleteAsset(CONTROLLER_PATH);
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(
            CONTROLLER_PATH
        );

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter(
            new AnimatorControllerParameter
            {
                name = "MoveSpeedScale",
                type = AnimatorControllerParameterType.Float,
                defaultFloat = 1f,
            }
        );

        AnimatorState locomotion = controller.CreateBlendTreeInController(
            "Locomotion",
            out BlendTree tree,
            0
        );
        tree.blendType = BlendTreeType.Simple1D;
        tree.blendParameter = "Speed";
        tree.useAutomaticThresholds = false;
        tree.AddChild(LoadClip(IDLE_FBX, IDLE_CLIP), 0f);
        tree.AddChild(LoadClip(WALK_FBX, WALK_CLIP), WALK_THRESHOLD);
        tree.AddChild(LoadClip(RUN_FBX, RUN_CLIP), RUN_THRESHOLD);
        locomotion.speedParameter = "MoveSpeedScale";
        locomotion.speedParameterActive = true;

        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
        stateMachine.defaultState = locomotion;

        AssetDatabase.SaveAssets();
    }

    private static AnimationClip LoadClip(string fbxPath, string clipName)
    {
        foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(fbxPath))
        {
            if (asset is AnimationClip clip && clip.name == clipName)
                return clip;
        }

        throw new System.Exception($"[PlayerModelSetup] clip '{clipName}' not found in {fbxPath}");
    }

    private static void PlaceModel(PlayerMovement player)
    {
        Transform playerTransform = player.transform;

        Transform oldModel = playerTransform.Find(MODEL_NAME);
        if (oldModel != null)
            Undo.DestroyObjectImmediate(oldModel.gameObject);

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CHARACTER_PREFAB);
        if (prefab == null)
            throw new System.Exception(
                $"[PlayerModelSetup] prefab not found at {CHARACTER_PREFAB}"
            );

        GameObject model = (GameObject)PrefabUtility.InstantiatePrefab(prefab, playerTransform);
        Undo.RegisterCreatedObjectUndo(model, MENU_PATH);
        model.name = MODEL_NAME;

        CapsuleCollider capsule = player.GetComponent<CapsuleCollider>();
        float scale = capsule.height / MODEL_NATIVE_HEIGHT;
        float feetY = capsule.center.y - capsule.height / 2f;

        model.transform.localPosition = new Vector3(0f, feetY, 0f);
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one * scale;

        MeshRenderer capsuleRenderer = player.GetComponent<MeshRenderer>();
        if (capsuleRenderer != null)
        {
            Undo.RecordObject(capsuleRenderer, MENU_PATH);
            capsuleRenderer.enabled = false;
        }

        if (player.GetComponent<PlayerAnimation>() == null)
            Undo.AddComponent<PlayerAnimation>(player.gameObject);

        Selection.activeGameObject = model;
    }
}
