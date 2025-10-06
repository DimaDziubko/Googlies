using System.Collections.Generic;
using _Game.UI._Shop.Scripts._DecorAndUtils;
using _Game.UI.Factory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Shop.Scripts._ShopScr
{
    public class ShopSubContainer : MonoBehaviour
    {
        private const int FIXED_COLUMNS_COUNT = 3;
        
        [SerializeField, Required] private RectTransform _transform;
        [SerializeField, Required] private Delimiter _delimiter;

        private readonly List<ShopItemView> _shopItems = new();
        private readonly List<Plug> _plugs = new();
        
        private IUIFactory _uiFactory;

        public void Construct(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }

        public RectTransform Transform => _transform;

        private void ShowDelimiter() => _delimiter.Show();

        private void HideDelimiter() => _delimiter.Hide();

        private int GetCountInCategory(RectTransform container) => 
            _shopItems.FindAll(item => item.transform.parent == container).Count;

        public void AddView(ShopItemView shopItem)
        {
            _shopItems.Add(shopItem);
            if (_shopItems.Count > 0) ShowDelimiter();
        }

        public void ForceRebuildLayoutImmediate() => 
            LayoutRebuilder.ForceRebuildLayoutImmediate(_transform);

        public void Cleanup()
        {
            _delimiter.Hide();
            
            foreach (var shopItem in _shopItems)
            {
                shopItem.Release();
            }
            
            _shopItems.Clear();
            
            foreach (var plug in _plugs) plug.Release();
            
            _plugs.Clear();
        }

        public void TryRemove(ShopItemView view)
        {
            if (_shopItems.Contains(view))
            {
                view.Cleanup();
                view.Release();
                _shopItems.Remove(view);
            }
        }

        public void AddPlugsIfNeeded()
        {
            var amount = _shopItems.Count;
            
            int remainder = amount % FIXED_COLUMNS_COUNT;
            if (remainder != 0)
            {
                int plugsNeeded = FIXED_COLUMNS_COUNT - remainder;
                for (int i = 0; i < plugsNeeded; i++)
                {
                    var plug = _uiFactory.GetShopItemPlug(_transform);
                    _plugs.Add(plug);
                }
            }
        }
    }
}