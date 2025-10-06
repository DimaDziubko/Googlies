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
    public class WarriorIconsReleasingOperation : ILoadingOperation
    {
        private readonly WarriorIconContainer _container;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;
        public string Description => "Warrior icons unloading...";

        private readonly IEnumerable<IIconReference> _iconsToRelease;

        public WarriorIconsReleasingOperation(
            WarriorIconContainer container,
            IAssetRegistry assetRegistry,
            IEnumerable<IIconReference> iconsToRelease,
            IMyLogger logger)
        {
            _container = container;
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
            
            _logger.Log("Releasing Warriors Icons...", DebugStatus.Warning);
            
            foreach (var iconReference in _iconsToRelease)
            {
                if (_container.Contains(iconReference.Atlas.AssetGUID))
                {
                    if (_assetRegistry.Release(iconReference.Atlas))
                    {
                        _container.TryRemove(iconReference.Atlas.AssetGUID);
                        _logger.Log($"Released Warriors Atlas: {iconReference.Atlas}", DebugStatus.Warning);
                    }
                }
            }

            _logger.Log("Warriors Icons released.", DebugStatus.Info);
            
            onProgress?.Invoke(1f);
        }
    }
}