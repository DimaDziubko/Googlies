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
    public class AgeIconsLoadingOperation : ILoadingOperation
    {
        private readonly AgeIconContainer _ageIconContainer;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;
        public string Description => "Age icons loading...";

        private readonly List<IIconReference> _iconsToLoad;

        public AgeIconsLoadingOperation(
            AgeIconContainer ageIconContainer,
            IAssetRegistry assetRegistry,
            List<IIconReference> iconsToLoad,
            IMyLogger logger)
        {
            _ageIconContainer = ageIconContainer;
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

            _logger.Log("Loading Age Icons...", DebugStatus.Warning);

            foreach (var ageConfig in _iconsToLoad)
            {
                await _assetRegistry.WarmUp<IList<Sprite>>(ageConfig.Atlas);
            }

            foreach (var ageConfig in _iconsToLoad)
            {
                var atlas = await _assetRegistry.LoadAsset<IList<Sprite>>(ageConfig.Atlas);

                var icon = atlas.FirstOrDefault(sprite => sprite.name == ageConfig.IconName);

                if (icon != null)
                {
                    var atlasContainer = _ageIconContainer.GetOrAdd(
                        ageConfig.Atlas.AssetGUID,
                        _ => new ResourceContainer<string, Sprite>());

                    atlasContainer.TryAdd(ageConfig.IconName, icon);

                    _logger.Log($"Loaded Age Icon '{ageConfig.IconName}' for Atlas: {ageConfig.Atlas}");
                }
                else
                {
                    _logger.LogError(
                        $"Icon '{ageConfig.IconName}' not found in Atlas: {ageConfig.Atlas}");
                }
            }

            _logger.Log("Age Icons loaded.", DebugStatus.Info);

            onProgress?.Invoke(1f);

#if UNITY_EDITOR
            perfTimer.Stop();
#endif
        }
    }
}