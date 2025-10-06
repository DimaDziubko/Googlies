using System;
using System.Collections.Generic;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Loading
{
    public class ShopIconsLoadingOperation : ILoadingOperation
    {
        private readonly ShopIconsContainer _container;
        private readonly IIconConfigRepository _iconConfig;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        public ShopIconsLoadingOperation(
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

        public string Description => "Shop icons loading...";
        public async UniTask Load(Action<float> onProgress)
        {
#if UNITY_EDITOR
            var perfTimer = new PerfTimer(_logger, Description);
            perfTimer.Start();
#endif
            onProgress?.Invoke(0.2f);

            var atlasReference = _iconConfig.GetShopIconsReference();

            _logger.Log("Loading shop icons...", DebugStatus.Warning);

            await _assetRegistry.WarmUp<IList<Sprite>>(atlasReference);

            var atlas = await _assetRegistry.LoadAsset<IList<Sprite>>(atlasReference);

            if (atlas != null)
            {
                foreach (Sprite sprite in atlas)
                {
                    var atlasContainer = _container.GetOrAdd(
                        atlasReference.AssetGUID,
                        _ => new ResourceContainer<string, Sprite>());

                    atlasContainer.TryAdd(sprite.name, sprite);

                    _logger.Log($"Loaded shop Icon '{sprite.name}' for Atlas: {atlas}");
                }
            }
            else
            {
                _logger.Log(
                    $"Shop icon atlas is null", DebugStatus.Fault);
            }

            _logger.Log("Shop Icons loaded.", DebugStatus.Info);

            onProgress?.Invoke(1f);

#if UNITY_EDITOR
            perfTimer.Stop();
#endif
        }
    }
}