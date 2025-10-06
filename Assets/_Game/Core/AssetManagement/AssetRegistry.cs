using System.Collections.Generic;
using _Game._AssetProvider;
using _Game.Core._Logger;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;


namespace _Game.Core.AssetManagement
{
    public interface IAssetRegistry
    {
        UniTask WarmUp<T>(AssetReference reference) where T : class;
        UniTask WarmUp<T>(string key) where T : class;
        UniTask<T> LoadAsset<T>(AssetReference assetReference) where T : class;
        UniTask<T> LoadAsset<T>(string key) where T : class;
        bool Release(AssetReference reference);
        bool Release(string key);
        int GetReferenceCount(AssetReference reference);
        int GetReferenceCount(string key);
    }

    public class AssetRegistry : IAssetRegistry
    {
        [ShowInInspector, ReadOnly] private readonly Dictionary<string, bool> _warmupCacheForStrings = new();

        [ShowInInspector, ReadOnly] private readonly Dictionary<string, int> _referenceCountsForStrings = new();

        private readonly IAssetProvider _assetProvider;
        private readonly IMyLogger _logger;

        public AssetRegistry(
            IAssetProvider assetProvider,
            IMyLogger logger)
        {
            _assetProvider = assetProvider;
            _logger = logger;
        }

        public async UniTask<T> LoadAsset<T>(AssetReference assetReference) where T : class
        {
            await WarmUp<T>(assetReference);
            return await LoadInternal<T>(assetReference);
        }

        public async UniTask<T> LoadAsset<T>(string key) where T : class
        {
            await WarmUp<T>(key);
            return await LoadInternal<T>(key);
        }

        public bool Release(AssetReference assetReference)
        {
            if (_referenceCountsForStrings.TryGetValue(assetReference.AssetGUID, out var count))
            {
                count--;

                if (count == 0)
                {
                    _referenceCountsForStrings.Remove(assetReference.AssetGUID);
                    _assetProvider.Release(assetReference.AssetGUID);
                    _warmupCacheForStrings[assetReference.AssetGUID] = false;
                    return true;
                }

                _referenceCountsForStrings[assetReference.AssetGUID] = count;
            }

            return false;
        }

        public bool Release(string key)
        {
            if (_referenceCountsForStrings.TryGetValue(key, out var count))
            {
                count--;

                if (count == 0)
                {
                    _referenceCountsForStrings.Remove(key);
                    _assetProvider.Release(key);
                    _warmupCacheForStrings[key] = false;
                    return true;
                }

                _referenceCountsForStrings[key] = count;
            }

            return false;
        }

        public int GetReferenceCount(AssetReference assetReference) =>
            _referenceCountsForStrings.GetValueOrDefault(assetReference.AssetGUID, 0);

        public int GetReferenceCount(string key) =>
            _referenceCountsForStrings.GetValueOrDefault(key, 0);

        public async UniTask WarmUp<T>(string key) where T : class
        {
            if (_warmupCacheForStrings.TryGetValue(key, out bool isWarmedUp) && isWarmedUp)
            {
                return;
            }

            var asset = await _assetProvider.Load<T>(key);
            if (asset != null)
            {
                _warmupCacheForStrings[key] = true;
                _logger.Log($"Resource {key} warmed up successfully.", DebugStatus.Success);
            }
            else
            {
                _warmupCacheForStrings[key] = false;
                _logger.Log($"Failed to warm up resource {key}.", DebugStatus.Fault);
            }
        }

        public async UniTask WarmUp<T>(AssetReference reference) where T : class
        {
            if (_warmupCacheForStrings.TryGetValue(reference.AssetGUID, out bool isWarmedUp) && isWarmedUp)
            {
                return;
            }

            var asset = await _assetProvider.Load<T>(reference);
            if (asset != null)
            {
                _warmupCacheForStrings[reference.AssetGUID] = true;
                _logger.Log($"Resource {reference.AssetGUID} warmed up successfully.", DebugStatus.Success);
            }
            else
            {
                _warmupCacheForStrings[reference.AssetGUID] = false;
                _logger.Log($"Failed to warm up resource {reference.AssetGUID}", DebugStatus.Fault);
            }
        }

        private async UniTask<T> LoadInternal<T>(AssetReference assetReference) where T : class
        {
            if (_referenceCountsForStrings.ContainsKey(assetReference.AssetGUID))
            {
                _referenceCountsForStrings[assetReference.AssetGUID]++;
            }
            else
            {
                _referenceCountsForStrings[assetReference.AssetGUID] = 1;
            }

            return await _assetProvider.Load<T>(assetReference);
        }

        private async UniTask<T> LoadInternal<T>(string key) where T : class
        {
            if (_referenceCountsForStrings.ContainsKey(key))
            {
                _referenceCountsForStrings[key]++;
            }
            else
            {
                _referenceCountsForStrings[key] = 1;
            }

            return await _assetProvider.Load<T>(key);
        }
    }
}