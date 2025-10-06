using System;
using System.Collections.Generic;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Loading;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Navigation.Timeline
{
    public class AmbienceReleasingOperation : ILoadingOperation
    {
        private readonly AmbienceContainer _ambienceContainer;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;
        public string Description => "Ambience unloading...";

        private readonly List<string> _ambienceToRelease;

        public AmbienceReleasingOperation(
            AmbienceContainer ageIconContainer,
            IAssetRegistry assetRegistry,
            List<string> ambienceToRelease,
            IMyLogger logger)
        {
            _ambienceContainer = ageIconContainer;
            _assetRegistry = assetRegistry;
            _logger = logger;
            _ambienceToRelease = ambienceToRelease;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            if (_ambienceToRelease == null)
            {
                onProgress?.Invoke(1);
                return;
            }
            
            onProgress?.Invoke(0.2f);
            
            _logger.Log("Releasing Ambience...", DebugStatus.Warning);

            foreach (var key in _ambienceToRelease)
            {
                if (_ambienceContainer.Contains(key))
                {
                    if (_assetRegistry.Release(key))
                    {
                        _ambienceContainer.TryRemove(key);
                        _logger.Log($"Removed Ambience '{key}' from the container.", DebugStatus.Warning);
                    }
                }
            }

            _logger.Log("Ambience released.", DebugStatus.Info);
            
            onProgress?.Invoke(1f);
        }
    }
}