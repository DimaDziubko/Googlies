using System.Collections.Generic;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Core.Configs.Repositories;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.Loading
{
    public class WarriorIconOperationFactory
    {
        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;
        private readonly IConfigRepository _config;
        private readonly WarriorIconContainer _container;
        private readonly IAssetRegistry _assetRegistry;

        public WarriorIconOperationFactory(
            IUserContainer userContainer,
            IConfigRepository config,
            IAssetRegistry assetRegistry,
            WarriorIconContainer warriorIconContainer,
            IMyLogger logger)
        {
            _assetRegistry = assetRegistry;
            _container = warriorIconContainer;
            _config = config;
            _userContainer = userContainer;
            _logger = logger;
        }

        public ILoadingOperation CreateInitialWarriorIconsLoadingOperation() =>
            new InitialWarriorIconsLoadingOperation(_userContainer, _config.TimelineConfigRepository, _container,
                _assetRegistry, _logger);

        public ILoadingOperation CreateAgeWarriorIconsLoadingOperation(int timelineId, int ageId) =>
            new AgeWarriorIconsLoadingOperation(_container, _config.TimelineConfigRepository, _assetRegistry,
                timelineId, ageId, _userContainer, _logger);

        public ILoadingOperation CreateAgeWarriorIconsReleasingOperation(int timelineId, int ageId) =>
            new AgeWarriorIconsReleasingOperation(_container, _config.TimelineConfigRepository, _assetRegistry,
                timelineId, ageId, _logger);

        public ILoadingOperation CreateWarriorIconsLoadingOperation(IEnumerable<IIconReference> iconsToLoad) => 
            new WarriorIconsLoadingOperation(_container, _assetRegistry, iconsToLoad, _logger);

        public ILoadingOperation CreateWarriorIconsReleasingOperation(IEnumerable<IIconReference> iconsToRelease) => 
            new WarriorIconsReleasingOperation(_container, _assetRegistry, iconsToRelease, _logger);
    }
}