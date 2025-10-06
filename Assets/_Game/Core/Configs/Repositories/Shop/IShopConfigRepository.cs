using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Models._ShopConfig;
using _Game.Core.Services.IAP;

namespace _Game.Core.Configs.Repositories.Shop
{
    public interface IShopConfigRepository
    {
        IEnumerable<ProductConfig> ProductConfigs { get;}
        GemsBundleConfig GetGemsBundleConfig(int id);
        CoinsBundleConfig GetCoinsBundleConfig(int id);
        SpeedBoostOfferConfig GetSpeedBoostOfferConfig(int id);
        FreeGemsPackConfig GetFreeGemsPackDayConfig(int id);
        ProfitOfferConfig GetProfitOfferConfig(int id);
        IEnumerable<GemsBundleConfig> GetGemsBundleConfigs();
        IEnumerable<CoinsBundleConfig> GetCoinsBundleConfigs();
        IEnumerable<SpeedBoostOfferConfig> GetSpeedBoostOfferConfigs();
        IEnumerable<FreeGemsPackConfig> GetFreeGemsPackConfigs();
        IEnumerable<ProfitOfferConfig> GetProfitOfferConfigs();
        IEnumerable<AdsGemsPackConfig> GetAdsGemsPackConfigs();
        IEnumerable<ProductWrapper> GetProductKeys();
    }
}