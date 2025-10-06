using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Loading;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Navigation.Timeline
{
    public class InitialWarriorIconsLoadingOperation : ILoadingOperation
    {
        private readonly WarriorIconContainer _container;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IUserContainer _userContainer;
        private readonly ITimelineConfigRepository _config;
        private readonly IMyLogger _logger;
        public string Description => "Initial warriors icons loading...";

        private List<IIconReference> _iconsToLoad;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        
        public InitialWarriorIconsLoadingOperation(
            IUserContainer userContainer,
            ITimelineConfigRepository config,
            WarriorIconContainer container,
            IAssetRegistry assetRegistry,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _config = config;
            _container = container;
            _assetRegistry = assetRegistry;
            _logger = logger;
        }

        public async UniTask Load(Action<float> onProgress)
        {
#if UNITY_EDITOR
            var perfTimer = new PerfTimer(_logger, Description);
            perfTimer.Start();
#endif
            _iconsToLoad = new List<IIconReference>();
            
            _iconsToLoad.AddRange(
                _config.GetRelatedAgeWarriors(TimelineState.TimelineId, TimelineState.AgeId)
                    .Select(x => x.GetIconReferenceFor(Skin.Ally))
            );

            _iconsToLoad.AddRange(
                _config.GetAllBattleWarriors(TimelineState.TimelineId)
                    .Select(x => x.GetIconReferenceFor(Skin.Hostile))
            );

            
            if (_iconsToLoad == null)
            {
                onProgress?.Invoke(1);
                return;
            }
            
            onProgress?.Invoke(0.2f);
            
            _logger.Log("Loading Age Icons...", DebugStatus.Warning);
            
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

            _logger.Log("Warriors Icons loaded.");
            
            onProgress?.Invoke(1f);

#if UNITY_EDITOR
            perfTimer.Stop();
#endif
        }
    }
}