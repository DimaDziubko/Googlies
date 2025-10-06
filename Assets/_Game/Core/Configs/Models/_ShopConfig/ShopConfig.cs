using System.Collections.Generic;
using _Game.Core.Configs.Models._BattleSpeedConfig;
using _Game.Core.Configs.Models._Functions;
using _Game.Core.Services.IAP;
using _Game.Core.UserState._State;
using UnityEngine.Purchasing;

namespace _Game.Core.Configs.Models._ShopConfig
{
    public class ShopConfig
    {
        public List<GemsBundleConfig> GemsBundleConfigs;
        public List<CoinsBundleConfig> CoinsBundleConfigs;
        public List<SpeedBoostOfferConfig> SpeedBoostOfferConfigs;
        public List<ProfitOfferConfig> ProfitOfferConfigs;
        public List<FreeGemsPackConfig> FreeGemsPackConfigs;
        public List<AdsGemsPackConfig> AdsGemsPackConfigs;
        public List<ProductWrapper> Products;
    }

    public class ProductWrapper
    {
        public int Id;
        public string IAP_ID;
        public string IAP_ID_IOS;
        public ProductType ProductType;

        public string GetProductKey()
        {
#if UNITY_ANDROID
            return IAP_ID;
#elif UNITY_IOS
            return IAP_ID_IOS;
#endif
        }
    }

    public class GemsBundleConfig : ProductConfig
    {
        public int Id;
        public string MajorIconKey;
        public string MinorIconKey;
        public float Quantity;
        public string IAP_ID;
        public string IAP_ID_IOS;
        public int MaxPurchaseCount;
        public int ShopItemViewId;
        public ProductType ProductType;

        public string GetProductKey()
        {
#if UNITY_ANDROID
            return IAP_ID;
#elif UNITY_IOS
            return IAP_ID_IOS;
#endif
        }
    }

    public class CoinsBundleConfig
    {
        public int Id;
        public string MajorIconKey;
        public string MinorIconKey;
        public int Price;
        public Exponential QuantityExponential;
        public string CurrencyIconKey;
        public string IGP_ID;
        public int MaxPurchaseCount;
        public int ShopItemViewId;
        public int MinishopItemViewId;
        public List<LinearFunction> LinearFunctions;

        public float GetQuantity(int ageId, int foodProductionLevel) =>
            LinearFunctions[ageId].GetIntValue(foodProductionLevel);

    }

    public class SpeedBoostOfferConfig : ProductConfig
    {
        public int Id;
        public string MajorIconKey;
        public string Description;
        public BattleSpeedConfig BattleSpeed;
        public string IAP_ID;
        public int MaxPurchaseCount;
        public int ShopItemViewId;
        public string RequiredIdBought;
        public ProductType ProductType;
    }

    public class AdsGemsPackConfig
    {
        public int Id;
        public int DailyGemsPackCount;
        public int RecoverTimeMinutes;
        public string MajorIconKey;
        public string MinorIconKey;
        public string AdIconKey;
        public int ShopItemViewId;
        public int Quantity;
    }

    public class ProfitOfferConfig : ProductConfig
    {
        public int Id;
        public string MajorIconKey;
        public string MinorIconKey;
        public string Name;
        public string IAP_ID;
        public string IAP_ID_IOS;
        public int MaxPurchaseCount;
        public int ShopItemViewId;
        public string Description;
        public int CoinBoostFactor;
        public List<MoneyBox> MoneyBoxes;
        public int RequiredIdBought;
        public ProductType ProductType;
        public string GetProductKey()
        {
#if UNITY_ANDROID
            return IAP_ID;
#elif UNITY_IOS
            return IAP_ID_IOS;
#endif
        }
    }

    public class MoneyBox
    {
        public int Id;
        public string IconKey;
        public float Quantity;
        public CurrencyType CurrencyType;
    }
}