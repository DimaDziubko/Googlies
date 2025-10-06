using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Game.Core._SceneLoader
{
    public class SceneLoader
    {
        public AsyncOperation LoadSceneAsync(string name, LoadSceneMode mode)
        {
            var loadOp = SceneManager.LoadSceneAsync(name, mode);
            return loadOp;
        }

        public Scene GetSceneByName(string name) => 
            SceneManager.GetSceneByName(name);

        public AsyncOperation UnloadSceneAsync(string sceneName) => 
            SceneManager.UnloadSceneAsync(sceneName);
    }
}