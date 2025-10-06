using System;
using System.Collections.Generic;
using System.Linq;
using _Game.UI._Shop.Scripts.Common;
using Zenject;

namespace _Game.UI._Shop.Scripts._ShopScr
{
    public class ShopItemManager<TModel, TView, TPresenter>
        where TModel : ShopItem
        where TView : ShopItemView
        where TPresenter : IProductPresenter, IDisposable
    {
        private readonly Dictionary<TModel, TPresenter> _presenters = new();
        private readonly IFactory<TModel, TView, TPresenter> _presenterFactory;

        private ShopItemsContainer _shopItemsContainer;

        public ShopItemManager(
            ShopItemsContainer shopItemsContainer,
            IFactory<TModel, TView, TPresenter> presenterFactory)
        {
            _shopItemsContainer = shopItemsContainer;
            _presenterFactory = presenterFactory;
        }

        public void AddItem(TModel model, ShopSubContainerType containerType)
        {
            if (!_presenters.ContainsKey(model))
            {
                TView view = _shopItemsContainer.SpawnShopItemView<TView>(model.ShopItemViewId, containerType);
                TPresenter presenter = _presenterFactory.Create(model, view);
                _presenters.Add(model, presenter);
                presenter.Initialize();
            }
        }

        public void RemoveItem(TModel model)
        {
            if (_presenters.ContainsKey(model))
            {
                var presenter = _presenters[model];
                presenter.Dispose();
                _shopItemsContainer.Remove(presenter.View);
                _presenters.Remove(model);
            }
        }

        public void ClearItems()
        {
            foreach (var pair in _presenters)
            {
                pair.Value.Dispose();
                _shopItemsContainer.Remove(pair.Value.View);
            }

            _presenters.Clear();
        }
        

        public void SetShopContainer(ShopItemsContainer shopContainer) => 
            _shopItemsContainer = shopContainer;
    }
}