using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


[InitializeOnLoad]
public class SceneBootstrapper
{
    private const string keyLoadBootScene = "3DG/Load Boot Scene";
    private const string keyPreviousScene = "PreviousScene";
    private const string BootScene = "Assets/Scenes/Boot.unity";

    private static bool isLoadBootScene
    {
        get => EditorPrefs.GetBool(keyLoadBootScene);
        set => EditorPrefs.SetBool(keyLoadBootScene, value);
    }

    private static string PreviousScene
    {
        get => EditorPrefs.GetString(keyPreviousScene);
        set => EditorPrefs.SetString(keyPreviousScene, value);
    }

    static SceneBootstrapper()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
    {

        if (!isLoadBootScene)
            return;

        switch (playModeStateChange)
        {
            case PlayModeStateChange.ExitingEditMode:
                PreviousScene = EditorSceneManager.GetActiveScene().path;

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    EditorSceneManager.OpenScene(BootScene);
                break;

            case PlayModeStateChange.EnteredEditMode:
                if (!string.IsNullOrEmpty(PreviousScene))
                    EditorSceneManager.OpenScene(PreviousScene);
                break;
        }
    }

    // menu items
    [MenuItem(keyLoadBootScene)]
    private static void ToggleLoadBootScene()
    {
        isLoadBootScene = !isLoadBootScene;
    }

    [MenuItem(keyLoadBootScene, true)]
    private static bool ValidateToggleLoadBootScene()
    {
        Menu.SetChecked(keyLoadBootScene, isLoadBootScene);
        return true;
    }

}
