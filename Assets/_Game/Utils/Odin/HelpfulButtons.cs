#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace _Game.Utils.Odin
{
    public class HelpfulButtons : OdinEditorWindow
    {

        [MenuItem("Tools/Helpful Buttons")]
        public static void OpenWindow()
        {
            GetWindow<HelpfulButtons>().Show();
        }

        [ButtonGroup]
        private void StartupScene()
        {
            LoadScene("Assets/_Game/Scenes/Startup.unity");
        }

        [ButtonGroup]
        private void BattleScene()
        {
            LoadScene("Assets/_Game/Scenes/BattleMode.unity");
        }

        [ButtonGroup]
        private void ZombieRushScene()
        {
            LoadScene("Assets/_Game/Scenes/ZombieRushMode.unity");
        }

        [ButtonGroup]
        private void TestScene()
        {
            LoadScene("Assets/_Game/Scenes/Test.unity");
        }

        private void LoadScene(string path)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(path);
        }
    }
}
#endif
