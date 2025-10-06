using System.Collections.Generic;
using System.Linq;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using Balancy.Models.SmartObjects;

namespace _Game.Core.Services.IAP
{
    public class PurchaseChecker
    {
        private readonly IUserContainer _userContainer;
        
        private IPurchaseDataStateReadonly PurchaseData => _userContainer.State.PurchaseDataState;
        public PurchaseChecker(IUserContainer userContainer) => 
            _userContainer = userContainer;

        public bool IsBought(Price[] offerAllPrices)
        {
            if (offerAllPrices == null || offerAllPrices.Length == 0)
                return false;

            List<string> allIAPIds = offerAllPrices
                .Where(price => price?.Product != null)
                .Select(price => price.Product.ProductId)
                .ToList();

            List<BoughtIAP> boughtIaPs = PurchaseData.BoughtIAPs;

            return boughtIaPs != null &&
                   boughtIaPs.Any(bought => allIAPIds.Contains(bought.IAPId) && bought.Count > 0);
        }
        
        public bool IsBought(string iapId)
        {
            List<BoughtIAP> boughtIaPs = PurchaseData.BoughtIAPs;
            var boughtIap = boughtIaPs.FirstOrDefault(x => x.IAPId == iapId);
            return boughtIap is { Count: > 0 };
        }
        
        public bool IsBought(IPurchaseDataStateReadonly concretePurchaseData, Price[] offerAllPrices)
        {
            if (offerAllPrices == null || offerAllPrices.Length == 0)
                return false;

            List<string> allIAPIds = offerAllPrices
                .Where(price => price?.Product != null)
                .Select(price => price.Product.ProductId)
                .ToList();
            
            List<BoughtIAP> boughtIaPs = concretePurchaseData.BoughtIAPs;

            return boughtIaPs != null &&
                   boughtIaPs.Any(bought => allIAPIds.Contains(bought.IAPId) && bought.Count > 0);
        }
    }
}