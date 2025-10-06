using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories;

namespace _Game.Core.Loading
{
    public class ShopIconOperationFactory
    {
        private readonly IMyLogger _logger;
        private readonly IConfigRepository _config;
        private readonly ShopIconsContainer _container;
        private readonly IAssetRegistry _assetRegistry;

        public ShopIconOperationFactory(
            IConfigRepository config,
            IAssetRegistry assetRegistry,
            ShopIconsContainer container,
            IMyLogger logger)
        {
            _assetRegistry = assetRegistry;
            _container = container;
            _config = config;
            _logger = logger;
        }

        public ILoadingOperation CreateShopIconsLoadingOperation() => 
            new ShopIconsLoadingOperation(_container, _config.IconConfigRepository, _assetRegistry, _logger);

        public ILoadingOperation ShopIconsReleasingOperation() => 
            new ShopIconsReleasingOperation(_container, _config.IconConfigRepository, _assetRegistry, _logger);
    }
}