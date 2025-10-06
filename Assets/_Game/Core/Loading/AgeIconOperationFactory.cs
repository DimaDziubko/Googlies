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
    public class AgeIconOperationFactory
    {
        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;
        private readonly IConfigRepository _config;
        private readonly AgeIconContainer _container;
        private readonly IAssetRegistry _assetRegistry;

        public AgeIconOperationFactory(
            IUserContainer userContainer,
            IConfigRepository config,
            IAssetRegistry assetRegistry,
            AgeIconContainer container,
            IMyLogger logger)
        {
            _assetRegistry = assetRegistry;
            _container = container;
            _config = config;
            _userContainer = userContainer;
            _logger = logger;
        }

        public ILoadingOperation CreateInitialAgeIconsLoadingOperation() =>
            new InitialAgeIconsLoadingOperation(_userContainer, _config.TimelineConfigRepository, _container,
                _assetRegistry, _logger);

        public ILoadingOperation CreateAgeIconsLoadingOperation(List<IIconReference> iconsToLoad) => 
            new AgeIconsLoadingOperation(_container, _assetRegistry, iconsToLoad, _logger);

        public ILoadingOperation CreateAgeIconsReleasingOperation(List<IIconReference> iconsToRelease) =>
            new AgeIconsReleasingOperation(_container, _assetRegistry, iconsToRelease, _logger);
    }
}