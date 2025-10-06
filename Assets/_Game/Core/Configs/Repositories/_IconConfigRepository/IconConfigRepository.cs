using System;
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Boosts.Scripts;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Game.Core.Configs.Repositories._IconConfigRepository
{
    public class IconConfigRepository : IIconConfigRepository
    {
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;

        public IconConfigRepository(
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _logger = logger;
        }

        public Sprite FoodIcon() =>
            _userContainer.GameConfig.IconConfig.GetFoodIcon();
        public Sprite ForBaseIcon() =>
            _userContainer.GameConfig.IconConfig.BaseIcon;

        public Sprite ForBoostIcon(BoostType boostType) =>
            _userContainer.GameConfig.IconConfig.GetBoostIconFor(boostType);

        public Sprite GetAllUnitAttackIcon() =>
            _userContainer.GameConfig.IconConfig.GetUnitAttackIcon();

        public Sprite GetAllUnitHealthIcon() =>
            _userContainer.GameConfig.IconConfig.GetUnitHealthIcon();

        public Sprite GetIcon(UpgradeItemType upgradeItemType) =>
            _userContainer.GameConfig.IconConfig.GetUpgradeIcon(upgradeItemType);

        public Sprite ForStatIcon(StatType statType) =>
            _userContainer.GameConfig.IconConfig.GetStatIcon(statType);

        public Sprite GetCurrencyIconFor(string key) =>
            _userContainer.GameConfig.IconConfig.GetCurrencyIconFor(key);

        public Sprite GetCurrencyIconFor(CurrencyType type) =>
            _userContainer.GameConfig.IconConfig.GetCurrencyIconFor(type);

        public Sprite GetAdsIcon() => _userContainer.GameConfig.IconConfig.GetAdsIcon();
        public AssetReference GetShopIconsReference() => _userContainer.GameConfig.IconConfig.ShopItemsReference;
        public Sprite GetItemIconFor(string itemIconKey) =>
            _userContainer.GameConfig.IconConfig.GetItemIconFor(itemIconKey);

        public Sprite GetItemIconFor(int itemId) =>
            _userContainer.GameConfig.IconConfig.GetItemIconFor(itemId);

        public Sprite GetEventIconFor(string eventName) =>
            _userContainer.GameConfig.IconConfig.GetEventIconFor(eventName);

        public int GetPointInlineIndex(CurrencyType eventCurrencyCellType) =>
            _userContainer.GameConfig.IconConfig.GetPointInlineIndex(eventCurrencyCellType);

        public AssetReference GetWarriorsFundAtlasReference() => 
            _userContainer.GameConfig.IconConfig.GetWarriorsFundAtlasReference();
    }
}