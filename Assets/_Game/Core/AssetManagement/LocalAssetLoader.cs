using System;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace _Game.Core.AssetManagement
{
    public class LocalAssetLoader
    {
        private GameObject _cacheObject;

        private async UniTask<T> Load<T>(string assetId, Transform parent = null) where T : Component
        {
            var handle = Addressables.InstantiateAsync(assetId, parent);
            _cacheObject = await handle.Task.AsUniTask();

            if (_cacheObject.TryGetComponent(out T component) == false)
            {
                throw new NullReferenceException(
                    $"Component of type {typeof(T)} is null on attempt to load it from addressables");
            }

            return component;
        }

        protected async UniTask<T> Load<T>(string assetId, string sceneName, Transform parent = null)
            where T : Component
        {
            var scene = GetOrCreateScene(sceneName);

            var handle = Addressables.InstantiateAsync(assetId, parent);
            _cacheObject = await handle.Task.AsUniTask();

            if (_cacheObject.TryGetComponent(out T component) == false)
            {
                throw new NullReferenceException(
                    $"Component of type {typeof(T)} is null on attempt to load it from addressables");
            }

            SceneManager.MoveGameObjectToScene(_cacheObject, scene);

            return component;
        }

        protected async UniTask<Disposable<T>> LoadDisposable<T>(string assetId, Transform parent = null)
            where T : Component
        {
            var component = await Load<T>(assetId, parent);
            return Disposable<T>.Borrow(component, _ => Unload());
        }

        protected async UniTask<Disposable<T>> LoadDisposable<T>(string assetId, string sceneName,
            Transform parent = null)
            where T : Component
        {
            var component = await Load<T>(assetId, sceneName, parent);
            return Disposable<T>.Borrow(component, _ => Unload());
        }

        protected void Unload()
        {
            if (_cacheObject == null)
                return;

            _cacheObject.SetActive(false);
            Addressables.ReleaseInstance(_cacheObject);
            _cacheObject = null;
        }

        private Scene GetOrCreateScene(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);

            if (!scene.isLoaded)
            {
                scene = SceneManager.CreateScene(sceneName);
            }

            return scene;
        }
    }
}