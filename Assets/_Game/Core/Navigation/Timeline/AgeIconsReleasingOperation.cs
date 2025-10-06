using System;
using System.Collections.Generic;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Core.Loading;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Navigation.Timeline
{
    public class AgeIconsReleasingOperation : ILoadingOperation
    {
        private readonly AgeIconContainer _ageIconContainer;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;
        public string Description => "Age icons unloading...";

        private readonly List<IIconReference> _iconsToRelease;

        public AgeIconsReleasingOperation(
            AgeIconContainer ageIconContainer,
            IAssetRegistry assetRegistry,
            List<IIconReference> iconsToRelease,
            IMyLogger logger)
        {
            _ageIconContainer = ageIconContainer;
            _assetRegistry = assetRegistry;
            _iconsToRelease = iconsToRelease;
            _logger = logger;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            if (_iconsToRelease == null)
            {
                onProgress?.Invoke(1);
                return;
            }
            
            onProgress?.Invoke(0.2f);
            
            _logger.Log("Releasing Age Icons...");
            
            foreach (var ageConfig in _iconsToRelease)
            {
                if (_ageIconContainer.Contains(ageConfig.Atlas.AssetGUID))
                {
                    if (_assetRegistry.Release(ageConfig.Atlas))
                    {
                        _ageIconContainer.TryRemove(ageConfig.Atlas.AssetGUID);
                        _logger.Log($"Removed Age Icon Container for Atlas: {ageConfig.Atlas}", DebugStatus.Warning);
                    }
                }
            }

            _logger.Log("Age Icons released.", DebugStatus.Info);
            
            onProgress?.Invoke(1f);
        }
    }
}