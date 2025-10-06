using System;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class ShopIconsReleasingOperation : ILoadingOperation
    {
        private readonly ShopIconsContainer _container;
        private readonly IIconConfigRepository _iconConfig;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        public ShopIconsReleasingOperation(
            ShopIconsContainer container,
            IIconConfigRepository iconConfig,
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _container = container;
            _iconConfig = iconConfig;
            _assetRegistry = assetRegistry;
            _logger = logger;
        }

        public string Description => "Shop icons releasing...";
        public UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.2f);

            var atlasReference = _iconConfig.GetShopIconsReference();
            
            _logger.Log("Releasing Warriors Icons...", DebugStatus.Warning);

            var subContainer = _container.Get(atlasReference.AssetGUID);
            foreach (var key in subContainer.Keys)
            {
                subContainer.TryRemove(key);
            }
            
            if (_container.Contains(atlasReference.AssetGUID))
            {
                if (_assetRegistry.Release(atlasReference.AssetGUID))
                {
                    _container.TryRemove(atlasReference.AssetGUID);
                    
                    _logger.Log($"Released Shop item Atlas: {atlasReference}", DebugStatus.Warning);
                }
            }

            _logger.Log("Shop item Atlas released.");

            onProgress?.Invoke(1f);
            
            return UniTask.CompletedTask;
        }
    }
}