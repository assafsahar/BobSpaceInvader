using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

public class CODOpenLoaderTool
{
    public class OpenLoaderTool
    {
        [MenuItem("COD/PlayGame")]
        //[System.Obsolete]
        public static void OpenLoader()
        {
            string currentSceneName = "GameScene";
            File.WriteAllText(".lastScene", currentSceneName);
            EditorSceneManager.OpenScene($"{Directory.GetCurrentDirectory()}/Assets/COD/Scenes/Loader.unity");
            EditorApplication.isPlaying = true;
        }

        [MenuItem("COD/LoadEditedScene")]
        public static void ReturnToLastScene()
        {
            string lastScene = File.ReadAllText(".lastScene");
            EditorSceneManager.OpenScene($"{Directory.GetCurrentDirectory()}/Assets/COD/Scenes/{lastScene}.unity");
        }

    }
}
