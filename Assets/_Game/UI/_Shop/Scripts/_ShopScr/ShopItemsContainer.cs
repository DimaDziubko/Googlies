using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.UI.Factory;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI._Shop.Scripts._ShopScr
{
    public enum ShopSubContainerType
    {
        ForOffers,
        ForCoins,
        ForGems,
        ForSkillPetFeeds
    }

    public class ShopItemsContainer : MonoBehaviour
    {
        [SerializeField, Required] private ShopSubContainer _offersContainer;
        [SerializeField, Required] private ShopSubContainer _skillPetFeedsContainer;
        [SerializeField, Required] private ShopSubContainer _coinsContainer;
        [SerializeField, Required] private ShopSubContainer _gemsContainer;

        private Dictionary<ShopSubContainerType, ShopSubContainer> _subContainers;

        private IUIFactory _uiFactory;
        private IMyLogger _logger;

        public void Construct(
            IUIFactory uiFactory,
            IMyLogger logger)
        {
            _uiFactory = uiFactory;
            _logger = logger;

            _offersContainer.Construct(uiFactory);
            _skillPetFeedsContainer.Construct(uiFactory);
            _coinsContainer.Construct(uiFactory);
            _gemsContainer.Construct(uiFactory);

            _subContainers = new Dictionary<ShopSubContainerType, ShopSubContainer>()
            {
                {ShopSubContainerType.ForOffers, _offersContainer},
                {ShopSubContainerType.ForCoins, _coinsContainer},
                {ShopSubContainerType.ForGems, _gemsContainer},
                {ShopSubContainerType.ForSkillPetFeeds, _skillPetFeedsContainer},
            };
        }

        public void UpdateDecorationElements()
        {
            _skillPetFeedsContainer.AddPlugsIfNeeded();
            _coinsContainer.AddPlugsIfNeeded();
            _gemsContainer.AddPlugsIfNeeded();

            _offersContainer.ForceRebuildLayoutImmediate();
            _skillPetFeedsContainer.ForceRebuildLayoutImmediate();
            _coinsContainer.ForceRebuildLayoutImmediate();
            _gemsContainer.ForceRebuildLayoutImmediate();

        }

        public T SpawnShopItemView<T>(int id, ShopSubContainerType subContainerType) where T : ShopItemView
        {
            T view = _uiFactory.GetShopItem<T>(id, _subContainers[subContainerType].Transform);
            _subContainers[subContainerType].AddView(view);
            return view;
        }

        public void Cleanup()
        {
            _coinsContainer.Cleanup();
            _gemsContainer.Cleanup();
            _skillPetFeedsContainer.Cleanup();
            _offersContainer.Cleanup();
        }

        public void Remove(ShopItemView view)
        {
            _coinsContainer.TryRemove(view);
            _gemsContainer.TryRemove(view);
            _skillPetFeedsContainer.TryRemove(view);
            _offersContainer.TryRemove(view);
        }
    }
}