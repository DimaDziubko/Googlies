using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Loading;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Navigation.Timeline
{
    public class InitialAmbienceLoadingOperation : ILoadingOperation
    {
        private readonly AmbienceContainer _ambienceContainer;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;
        private readonly ITimelineConfigRepository _config;
        public string Description => "Initial ambience loading...";

        private List<string> _ambienceToLoad;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public InitialAmbienceLoadingOperation(
            IUserContainer userContainer,
            ITimelineConfigRepository config,
            AmbienceContainer ageIconContainer,
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _config = config;
            _ambienceContainer = ageIconContainer;
            _assetRegistry = assetRegistry;
            _logger = logger;
        }

        public async UniTask Load(Action<float> onProgress)
        {
#if UNITY_EDITOR
            var perfTimer = new PerfTimer(_logger, Description);
            perfTimer.Start();
#endif
            _ambienceToLoad =
                _config.GetBattleConfigs(TimelineState.TimelineId)
                    .Select(x => x.AmbienceKey).ToList();

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
                if (_ambienceContainer.Contains(key))
                {
                    _logger.Log($"Ambience '{key}' already loaded.");
                    continue;
                }

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

            _logger.Log("Ambience loaded.");

            onProgress?.Invoke(1f);
            
#if UNITY_EDITOR
            perfTimer.Stop();
#endif
        }
    }
}