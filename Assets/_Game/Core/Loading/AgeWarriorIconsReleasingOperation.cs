using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Configs.Repositories.Timeline;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class AgeWarriorIconsReleasingOperation : ILoadingOperation
    {
        private readonly WarriorIconContainer _container;
        private readonly ITimelineConfigRepository _config;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IMyLogger _logger;

        private readonly int _timelineId;
        private readonly int _ageId;

        public AgeWarriorIconsReleasingOperation(
            WarriorIconContainer container,
            ITimelineConfigRepository config,
            IAssetRegistry assetRegistry,
            int timelineId,
            int ageId,
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
            IEnumerable<IIconReference> iconsToRelease = _config.GetRelatedAgeWarriors(_timelineId, _ageId)
                .Select(x => x.GetIconReferenceFor(Skin.Ally));

            onProgress?.Invoke(0.2f);
            
            _logger.Log("Releasing Warriors Icons...", DebugStatus.Warning);
            
            foreach (var iconReference in iconsToRelease)
            {
                if (_container.Contains(iconReference.Atlas.AssetGUID))
                {
                    if (_assetRegistry.Release(iconReference.Atlas.AssetGUID))
                    {
                        _container.TryRemove(iconReference.Atlas.AssetGUID);
                        _logger.Log($"Released Warriors Atlas: {iconReference.Atlas}", DebugStatus.Warning);
                    }
                }
            }

            _logger.Log("Warriors Icons released.");
            
            onProgress?.Invoke(1f);
        }
    }
}