using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Services.IGPService;
using _Game.UI._Shop.Scripts._CoinBundles;

namespace _Game.UI._Shop._MiniShop.Scripts
{
    public class MiniShopPresenter : IMiniShopPresenter
    {
        private readonly IIGPService _igpService;
        private readonly IMyLogger _logger;
        private readonly CoinsBundlePresenter.Factory _coinsBundlePresenterFactory;

        private readonly Dictionary<CoinsBundle, CoinsBundlePresenter> _coinsBundlePresenters = new();

        public MiniShop MiniShop { get; set; }

        public MiniShopPresenter(
            IIGPService igpService,
            IMyLogger logger,
            CoinsBundlePresenter.Factory coinsBundlePresenterFactory)
        {
            _igpService = igpService;
            _logger = logger;
            _coinsBundlePresenterFactory = coinsBundlePresenterFactory;
        }

        public void OnMiniShopOpened()
        {
            UpdateItems();
        }

        public void OnMiniShopClosed() => ClearCoinsBundles();

        private void ClearCoinsBundles()
        {
            foreach (var pair in _coinsBundlePresenters)
            {
                pair.Value.Dispose();
                MiniShop.Container.Remove(pair.Value.View);
            }
            
            _coinsBundlePresenters.Clear();
        }

        private void UpdateItems()
        {
            if(MiniShop == null) return;
            UpdateCoinsBundles();
        }

        private void UpdateCoinsBundles()
        {
            _igpService.UpdateProducts();
            List<CoinsBundle> bundles = _igpService.CoinsBundles;

            foreach (var bundle in bundles)
            {
                AddCoinsBundle(bundle);
            }
        }

        private void AddCoinsBundle(CoinsBundle bundle)
        {
            if (!_coinsBundlePresenters.ContainsKey(bundle))
            {
                CoinsBundleView view = MiniShop.Container.SpawnCoinBundleView(bundle.Config.MinishopItemViewId);
                CoinsBundlePresenter presenter = _coinsBundlePresenterFactory.Create(bundle, view);
                _coinsBundlePresenters.Add(bundle, presenter);
                presenter.Initialize();
            }
        }
    }
}