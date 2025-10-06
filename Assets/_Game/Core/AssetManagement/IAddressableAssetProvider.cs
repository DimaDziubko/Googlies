using System;
using _Game.Core.Loading;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace _Game.Core.AssetManagement
{
    public interface IAddressableAssetProvider :  ILoadingOperation
    {
        UniTask<SceneInstance> LoadSceneAdditive(string sceneId);
        UniTask UnloadAdditiveScene(SceneInstance scene);
        string Description { get; }
        UniTask Load(Action<float> onProgress);
    }
}