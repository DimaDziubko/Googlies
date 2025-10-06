using System.Collections.Generic;

namespace _Game.Core.Services.IAP
{
    public class ProfitOfferContainer
    {
        private readonly Dictionary<string, ProfitOfferModel> _profitOffers = new();

        public void AddOrUpdate(string productId, ProfitOfferModel gemsBundle) =>
            _profitOffers[productId] = gemsBundle;

        public ProfitOfferModel Get(string productId) =>
            _profitOffers.ContainsKey(productId) ? _profitOffers[productId] : null;

        public bool TryGetValue(string productId, out ProfitOfferModel bundle) =>
            _profitOffers.TryGetValue(productId, out bundle);

        public bool Contains(string productId) =>
            _profitOffers.ContainsKey(productId);

        public void Clear() =>
            _profitOffers.Clear();

        public IEnumerable<ProfitOfferModel> GetAll() =>
            _profitOffers.Values;
    }

}