using UnityEditor;
using UnityEditor.SceneManagement;

namespace Mini2.EditorTools
{
    // Opens the mini-2 scene when the project is opened, so the reviewer does not
    // start in an empty "Untitled" scene. It only runs once per editor session and
    // only when no saved scene is open, so it never replaces a scene you opened yourself.
    [InitializeOnLoad]
    static class OpenMini2SceneOnLoad
    {
        const string ScenePath = "Assets/Mini2_AStarPathfinding/Mini2_AStarPathfinding.unity";
        const string SessionKey = "Mini2.OpenSceneOnLoad.Done";

        static OpenMini2SceneOnLoad()
        {
            // SessionState is cleared when the editor closes, so script recompiles do not trigger this again
            if (SessionState.GetBool(SessionKey, false)) return;
            SessionState.SetBool(SessionKey, true);

            // wait until the editor has finished loading
            EditorApplication.delayCall += Open;
        }

        static void Open()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!string.IsNullOrEmpty(EditorSceneManager.GetActiveScene().path)) return; // a saved scene is already open
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) == null) return;

            EditorSceneManager.OpenScene(ScenePath);
        }
    }
}
