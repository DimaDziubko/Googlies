using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Loading;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Navigation.Timeline
{
    public class InitialAgeIconsLoadingOperation : ILoadingOperation
    {
        private readonly AgeIconContainer _ageIconContainer;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;
        private readonly ITimelineConfigRepository _config;
        public string Description => "Initial age icons loading...";

        private List<IIconReference> _iconsToLoad;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public InitialAgeIconsLoadingOperation(
            IUserContainer userContainer,
            ITimelineConfigRepository config,
            AgeIconContainer ageIconContainer,
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _config = config;
            _ageIconContainer = ageIconContainer;
            _assetRegistry = assetRegistry;
            _logger = logger;
        }

        public async UniTask Load(Action<float> onProgress)
        {
#if UNITY_EDITOR
            var perfTimer = new PerfTimer(_logger, Description);
            perfTimer.Start();
#endif
            _iconsToLoad = _config.GetAgeConfigs(TimelineState.TimelineId)
                .Select(x => x.GetIconReference()).ToList();

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

                    _logger.Log($"Loaded Initial Age Icon '{ageConfig.IconName}' for Atlas: {ageConfig.Atlas}",
                        DebugStatus.Success);
                }
                else
                {
                    _logger.LogError(
                        $"Icon '{ageConfig.IconName}' not found in Atlas: {ageConfig.Atlas}");
                }
            }

            _logger.Log("Age Icons loaded.");

            onProgress?.Invoke(1f);
            
#if UNITY_EDITOR
            perfTimer.Stop();
#endif
        }
    }
}