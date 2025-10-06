using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Loading
{
    public class AgeWarriorIconsLoadingOperation : ILoadingOperation
    {
        private readonly WarriorIconContainer _container;
        private readonly ITimelineConfigRepository _config;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        private readonly int _timelineId;
        private readonly int _ageId;

        public AgeWarriorIconsLoadingOperation(
            WarriorIconContainer container,
            ITimelineConfigRepository config,
            IAssetRegistry assetRegistry,
            int timelineId,
            int ageId,
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _container = container;
            _config = config;
            _assetRegistry = assetRegistry;
            _timelineId = timelineId;
            _ageId = ageId;
            _logger = logger;
        }

        public string Description => "Age Warrior Icons Loading...";

        public async UniTask Load(Action<float> onProgress)
        {
#if UNITY_EDITOR
            var perfTimer = new PerfTimer(_logger, Description);
            perfTimer.Start();
#endif
            IEnumerable<IIconReference> iconsToLoad = _config.GetRelatedAgeWarriors(_timelineId, _ageId)
                .Select(x => x.GetIconReferenceFor(Skin.Ally));

            onProgress?.Invoke(0.2f);

            _logger.Log("Loading Age Icons...", DebugStatus.Warning);

            foreach (var iconReference in iconsToLoad)
            {
                await _assetRegistry.WarmUp<IList<Sprite>>(iconReference.Atlas);
            }

            foreach (var iconReference in iconsToLoad)
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

            _logger.Log("Warriors Icons loaded.");

            onProgress?.Invoke(1f);

#if UNITY_EDITOR
            perfTimer.Stop();
#endif
        }
    }
}