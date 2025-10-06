using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Game._AssetProvider
{
    public interface IAssetProvider
    {
        UniTask<GameObject> Instantiate(string path);
        UniTask<GameObject> Instantiate(string path, Vector3 at);
        UniTask<T> Load<T>(AssetReference assetReference) where T : class;
        UniTask<T> Load<T>(string address) where T : class;
        void CleanUp();
        UniTask<GameObject> Instantiate(string address, Transform under);
        UniTask<GameObject> Instantiate(AssetReference assetReference);
        void Release(string key);
    }
}