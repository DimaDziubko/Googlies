using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Models._ShopConfig;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.Configs.Repositories.Shop
{
    public class ShopConfigRepository : IShopConfigRepository
    {
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;

        public ShopConfigRepository(
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _logger = logger;
        }

        public GemsBundleConfig GetGemsBundleConfig(int id) =>
            _userContainer.GameConfig.ShopConfig.GemsBundleConfigs
                .FirstOrDefault(x => x.Id == id);

        public CoinsBundleConfig GetCoinsBundleConfig(int id) =>
            _userContainer.GameConfig.ShopConfig.CoinsBundleConfigs
                .FirstOrDefault(x => x.Id == id);

        public SpeedBoostOfferConfig GetSpeedBoostOfferConfig(int id) =>
            _userContainer.GameConfig.ShopConfig.SpeedBoostOfferConfigs
                .FirstOrDefault(x => x.Id == id);

        public FreeGemsPackConfig GetFreeGemsPackDayConfig(int id) =>
            _userContainer.GameConfig.ShopConfig.FreeGemsPackConfigs
                .FirstOrDefault(x => x.Id == id);

        public ProfitOfferConfig GetProfitOfferConfig(int id)
        {
            return _userContainer.GameConfig.ShopConfig.ProfitOfferConfigs
                .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<GemsBundleConfig> GetGemsBundleConfigs() =>
            _userContainer.GameConfig.ShopConfig.GemsBundleConfigs;

        public IEnumerable<CoinsBundleConfig> GetCoinsBundleConfigs() =>
            _userContainer.GameConfig.ShopConfig.CoinsBundleConfigs;

        public IEnumerable<SpeedBoostOfferConfig> GetSpeedBoostOfferConfigs() =>
            _userContainer.GameConfig.ShopConfig.SpeedBoostOfferConfigs;

        public IEnumerable<FreeGemsPackConfig> GetFreeGemsPackConfigs() =>
            _userContainer.GameConfig.ShopConfig.FreeGemsPackConfigs;
        
        public IEnumerable<ProfitOfferConfig> GetProfitOfferConfigs() =>
            _userContainer.GameConfig.ShopConfig.ProfitOfferConfigs;

        public IEnumerable<AdsGemsPackConfig> GetAdsGemsPackConfigs() => 
            _userContainer.GameConfig.ShopConfig.AdsGemsPackConfigs;

        public IEnumerable<ProductWrapper> GetProductKeys() => 
            _userContainer.GameConfig.ShopConfig.Products;

        public IEnumerable<ProductConfig> ProductConfigs 
            => GetGemsBundleConfigs()
                    .Cast<ProductConfig>()
                    .Concat(GetSpeedBoostOfferConfigs())
                    .Concat(GetProfitOfferConfigs());
        
    }
}