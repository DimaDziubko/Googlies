using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public abstract class ProductConfig
    {
        public int Id;
        public string IAP_ID;
        public string IAP_ID_IOS;
        public ProductType ProductType;
        public int MaxPurchaseCount;
    }
}