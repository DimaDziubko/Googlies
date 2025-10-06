using System;
using System.Collections.Generic;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Loading;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Navigation.Timeline
{
    public class AmbienceLoadingOperation : ILoadingOperation
    {
        private readonly AmbienceContainer _ambienceContainer;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;
        public string Description => "Ambience loading...";

        private List<string> _ambienceToLoad;

        public AmbienceLoadingOperation(
            AmbienceContainer ageIconContainer,
            IAssetRegistry assetRegistry,
            List<string> ambienceToLoad,
            IMyLogger logger)
        {
            _ambienceContainer = ageIconContainer;
            _assetRegistry = assetRegistry;
            _logger = logger;
            _ambienceToLoad = ambienceToLoad;
        }

        public async UniTask Load(Action<float> onProgress)
        {
#if UNITY_EDITOR
            var perfTimer = new PerfTimer(_logger, Description);
            perfTimer.Start();
#endif
            if (_ambienceToLoad == null)
            {
                onProgress?.Invoke(1);
                return;
            }

            onProgress?.Invoke(0.2f);

            _logger.Log("Loading Ambience...", DebugStatus.Warning);


            foreach (var key in _ambienceToLoad)
            {
                await _assetRegistry.WarmUp<AudioClip>(key);
            }

            foreach (var key in _ambienceToLoad)
            {

                var clip = await _assetRegistry.LoadAsset<AudioClip>(key);

                if (clip != null)
                {
                    if (_ambienceContainer.TryAdd(key, clip))
                    {
                        _logger.Log($"Loaded Ambience '{key}'.");
                    }
                    else
                    {
                        _logger.LogWarning($"Ambience '{key}' already exists in the container.");
                    }
                }
                else
                {
                    _logger.LogError($"Failed to load Ambience '{key}'.");
                }
            }

            _logger.Log("Ambience loaded.", DebugStatus.Info);

            onProgress?.Invoke(1f);

#if UNITY_EDITOR
            perfTimer.Stop();
#endif
        }
    }
}