using _Game.Core.UserState._State;
using _Game.Gameplay._Boosts.Scripts;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Game.Core.Configs.Repositories._IconConfigRepository
{
    public interface IIconConfigRepository
    {
        Sprite FoodIcon();
        Sprite ForBaseIcon();
        Sprite ForBoostIcon(BoostType boostType);
        Sprite GetAllUnitAttackIcon();
        Sprite GetAllUnitHealthIcon();
        Sprite GetIcon(UpgradeItemType upgradeItemType);
        Sprite ForStatIcon(StatType statType);
        Sprite GetCurrencyIconFor(string key);
        Sprite GetCurrencyIconFor(CurrencyType type);
        Sprite GetAdsIcon();
        AssetReference GetShopIconsReference();
        Sprite GetItemIconFor(string configIconKey);
        Sprite GetItemIconFor(int itemId);
        Sprite GetEventIconFor(string eventName);
        int GetPointInlineIndex(CurrencyType eventCurrencyCellType);
        Sprite GetUnitType(bool isMelee);
        AssetReference GetWarriorsFundAtlasReference();
        Sprite GetBoostIconCardsFor(BoostType type);
    }
}