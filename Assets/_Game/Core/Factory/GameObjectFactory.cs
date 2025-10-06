using _Game._AssetProvider;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace _Game.Core.Factory
{
    public abstract class GameObjectFactory : ScriptableObject
    {
        private Scene _scene;

        private readonly AssetProvider _assetProvider = new(); 
                            
        protected async UniTask<T> CreateGameObjectInstanceAsync<T>(string prefabKey) where T : MonoBehaviour
        {
            GetOrCreateScene();
            
            GameObject gameObject = await _assetProvider.Instantiate(prefabKey);
            
            T instance = gameObject.GetComponent<T>();
            
            SceneManager.MoveGameObjectToScene(gameObject, _scene);
    
            return instance;
        }

        protected async UniTask<T> CreateGameObjectInstanceAsync<T>(AssetReference address) where T : MonoBehaviour
        {
            GetOrCreateScene();
            
            GameObject gameObject = await _assetProvider.Instantiate(address);
            
            T instance = gameObject.GetComponent<T>();
            
            SceneManager.MoveGameObjectToScene(gameObject, _scene);
    
            return instance;
        }

        
        protected async UniTask<T> CreateGameObjectInstanceAsync<T>(string prefabKey, Transform parent) where T : MonoBehaviour
        {
            GameObject gameObject = await _assetProvider.Instantiate(prefabKey, parent);
            
            T instance = gameObject.GetComponent<T>();
    
            return instance;
        }
        
        protected async UniTask<T> CreateGameObjectInstanceAsync<T>(string prefabKey, Vector3 at) where T : MonoBehaviour
        {
            GameObject gameObject = await _assetProvider.Instantiate(prefabKey, at);
            
            T instance = gameObject.GetComponent<T>();
    
            return instance;
        }
        
        protected T CreateGameObjectInstance<T>(T prefab) where T : MonoBehaviour
        {
            GetOrCreateScene();

            T instance = Instantiate(prefab);
            SceneManager.MoveGameObjectToScene(instance.gameObject, _scene);
            return instance;
        }
        
        protected T CreateGameObjectInstance<T>(T prefab, Transform parent) where T : MonoBehaviour
        {
            T instance = Instantiate(prefab, parent);
            return instance.GetComponent<T>();
        }

        public async UniTask Unload()
        {
            if (_scene.isLoaded)
            {
                var unloadOp = SceneManager.UnloadSceneAsync(_scene);
                while (unloadOp.isDone == false)
                {
                    await UniTask.Delay(1);
                }
            }
        }

        private void GetOrCreateScene() 
        {
            if (_scene.isLoaded)
            {
                if (Application.isEditor)
                {
                    _scene = SceneManager.GetSceneByName(name);
                    if (!_scene.isLoaded)
                    {
                        _scene = SceneManager.CreateScene(name);
                    }
                }
            }
            else
            {
                _scene = SceneManager.CreateScene(name);
            }
        }

        public virtual void Cleanup() => _assetProvider.CleanUp();
    }
}