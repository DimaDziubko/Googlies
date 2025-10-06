using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.UI._Shop.Scripts._CoinBundles;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI.Factory;
using UnityEngine;

namespace _Game.UI._Shop._MiniShop.Scripts
{
    public class MiniItemShopContainer : MonoBehaviour
    {
        [SerializeField] private Transform _parent;
        
        private readonly List<ShopItemView> _coinBundles = new List<ShopItemView>();
        
        private IUIFactory _uiFactory;
        private IMyLogger _logger;

        public void Construct(
            IUIFactory uiFactory,
            IMyLogger logger)
        {
            _uiFactory = uiFactory;
            _logger = logger;
        }
        
        public void Cleanup()
        {
            foreach (var item in _coinBundles)
            {
                item.Cleanup();
                item.Release();
            }
            _coinBundles.Clear();
        }

        public CoinsBundleView SpawnCoinBundleView(int id)
        {
            CoinsBundleView view = _uiFactory.GetShopItem<CoinsBundleView>(id, _parent);
            _coinBundles.Add(view);
            return view;
        }
        
        public void Remove(ShopItemView view)
        {
            if (_coinBundles.Contains(view))
            {
                view.Cleanup();
                view.Release();
                _coinBundles.Remove(view);
            }
        }
    }
}