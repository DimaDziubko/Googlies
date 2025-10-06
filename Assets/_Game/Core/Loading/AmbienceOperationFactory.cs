using System.Collections.Generic;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.Loading
{
    public class AmbienceOperationFactory
    {
        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;
        private readonly IConfigRepository _config;
        private readonly AmbienceContainer _container;
        private readonly IAssetRegistry _assetRegistry;

        public AmbienceOperationFactory(
            IUserContainer userContainer,
            IConfigRepository config,
            IAssetRegistry assetRegistry,
            AmbienceContainer container,
            IMyLogger logger)
        {
            _assetRegistry = assetRegistry;
            _container = container;
            _config = config;
            _userContainer = userContainer;
            _logger = logger;
        }

        public ILoadingOperation CreateInitialAmbienceLoadingOperation() =>
            new InitialAmbienceLoadingOperation(_userContainer, _config.TimelineConfigRepository, _container,
                _assetRegistry, _logger);

        public ILoadingOperation CreateAmbienceLoadingOperation(List<string> ambienceToLoad) => 
            new AmbienceLoadingOperation(_container, _assetRegistry, ambienceToLoad, _logger);

        public ILoadingOperation CreateAmbienceReleasingOperation(List<string> ambienceToRelease) => 
            new AmbienceReleasingOperation(_container, _assetRegistry, ambienceToRelease, _logger);
    }
}