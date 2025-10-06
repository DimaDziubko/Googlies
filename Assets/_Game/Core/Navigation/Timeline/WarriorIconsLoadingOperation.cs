using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Core.Loading;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Navigation.Timeline
{
    public class WarriorIconsLoadingOperation : ILoadingOperation
    {
        private readonly WarriorIconContainer _container;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;
        public string Description => "Warriors icons loading...";

        private IEnumerable<IIconReference> _iconsToLoad;

        public WarriorIconsLoadingOperation(
            WarriorIconContainer container,
            IAssetRegistry assetRegistry,
            IEnumerable<IIconReference> iconsToLoad,
            IMyLogger logger)
        {
            _container = container;
            _assetRegistry = assetRegistry;
            _logger = logger;
            _iconsToLoad = iconsToLoad;
        }

        public async UniTask Load(Action<float> onProgress)
        {
#if UNITY_EDITOR
            var perfTimer = new PerfTimer(_logger, Description);
            perfTimer.Start();
#endif
            if (_iconsToLoad == null)
            {
                onProgress?.Invoke(1);
                return;
            }

            onProgress?.Invoke(0.2f);

            _logger.Log("Loading Warrior Icons...", DebugStatus.Warning);

            foreach (var iconReference in _iconsToLoad)
            {
                await _assetRegistry.WarmUp<IList<Sprite>>(iconReference.Atlas);
            }

            foreach (var iconReference in _iconsToLoad)
            {
                var atlas = await _assetRegistry.LoadAsset<IList<Sprite>>(iconReference.Atlas);

                var icon = atlas.FirstOrDefault(sprite => sprite.name == iconReference.IconName);

                if (icon != null)
                {
                    var atlasContainer = _container.GetOrAdd(
                        iconReference.Atlas.AssetGUID,
                        _ => new ResourceContainer<string, Sprite>());

                    atlasContainer.TryAdd(iconReference.IconName, icon);

                    _logger.Log($"Loaded Warrior Icon '{iconReference.IconName}' for Atlas: {iconReference.Atlas}");
                }
                else
                {
                    _logger.LogError(
                        $"Icon '{iconReference.IconName}' not found in Atlas: {iconReference.Atlas}");
                }
            }

            _logger.Log("Warriors Icons loaded.", DebugStatus.Info);

            onProgress?.Invoke(1f);

#if UNITY_EDITOR
            perfTimer.Stop();
#endif
        }
    }
}