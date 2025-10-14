using System;
using System.Linq;
using _Game.Core.UserState._State;
using _Game.Gameplay._Boosts.Scripts;
using _Game.UI._StatsPopup._Scripts;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Game.Core.Configs.Models._IconConfig
{
    [CreateAssetMenu(fileName = "IcoConfig", menuName = "Configs/Icons")]
    [Serializable]
    public class IconConfig : ScriptableObject
    {
        public int Id;

        [Space, Required]
        public Sprite FoodIcon;

        [Space]
        [Header("Upgrades")]
        [Required] public Sprite BaseIcon;

        [Space]
        [Header("Currency")]
        [Required] public Sprite GemIcon;
        [Required] public Sprite CoinIcon;
        [Required] public Sprite SkillPotionIcon;


        [Required] public Sprite LeaderPassPoint;

        [Space]
        [Header("Keys")]
        [Required] public Sprite GreenKey;
        [Required] public Sprite YellowKey;
        [Required] public Sprite RedKey;

        [Space]
        [Header("Cards")]
        [Required] public Sprite CardIcon;
        [Header("Cards Boosts")]
        [SerializeField, Required] private CardBoostIconConfig[] _cardBoosts;
        [Space]
        [Header("Boosts")]
        [Required] public Sprite AllUnitDamageIcon;
        [Required] public Sprite AllUnitHealthIcon;
        [Required] public Sprite CoinsGainedIcon;
        [Required] public Sprite BaseHealthIcon;
        [Space]
        [Header("Unity Type")]
        [SerializeField, Required] private Sprite _meleeSprite;
        [SerializeField, Required] private Sprite _rangedSprite;

        [Space]
        [Required] public Sprite StatAttackIcon;

        [Space]
        [Required] public Sprite StatHealthIcon;

        [Space]
        public AssetReference ShopItemsReference;

        public Sprite GetFoodIcon() => FoodIcon;
        public Sprite GetAttackIcon() => StatAttackIcon;
        public Sprite GetHealthIcon() => StatHealthIcon;

        [Space][Header("Ads")] public Sprite AdsIcon;

        public GameEventIconConfig GameEventIconConfig;

        public Sprite GetBoostIconFor(BoostType boostType)
        {
            switch (boostType)
            {
                case BoostType.AllUnitDamage:
                    return AllUnitDamageIcon;
                case BoostType.AllUnitHealth:
                    return AllUnitHealthIcon;
                case BoostType.FoodProduction:
                    return FoodIcon;
                case BoostType.BaseHealth:
                    return BaseHealthIcon;
                case BoostType.CoinsGained:
                    return CoinsGainedIcon;
                default:
                    return AllUnitDamageIcon;
            }
        }

        public Sprite GetCurrencyIconFor(string key)
        {
            return key switch
            {
                "Currencies[Coin]" => CoinIcon,
                "Currencies[Gem]" => GemIcon,
                "Currencies[SkillPotion]" => SkillPotionIcon,
                "Currencies[LeaderPassPoint]" => LeaderPassPoint,
                _ => GemIcon,
            };
        }

        public Sprite GetCurrencyIconFor(CurrencyType type)
        {
            return type switch
            {
                CurrencyType.Coins => CoinIcon,
                CurrencyType.Gems => GemIcon,
                CurrencyType.SkillPotion => SkillPotionIcon,
                CurrencyType.LeaderPassPoint => LeaderPassPoint,
                _ => GemIcon,
            };
        }

        public Sprite GetAdsIcon() => AdsIcon;


        public Sprite GetStatIcon(StatType statType)
        {
            switch (statType)
            {
                case StatType.Damage:
                    return StatAttackIcon;
                case StatType.Health:
                    return StatHealthIcon;
                default:
                    return StatAttackIcon;
            }
        }

        public Sprite GetUpgradeIcon(UpgradeItemType type)
        {
            return type switch
            {
                UpgradeItemType.FoodProduction => FoodIcon,
                UpgradeItemType.BaseHealth => BaseHealthIcon,
                _ => FoodIcon,
            };
        }

        public Sprite GetUnitHealthIcon() => AllUnitHealthIcon;

        public Sprite GetUnitAttackIcon() => AllUnitDamageIcon;


        public Sprite GetItemIconFor(string itemIconKey)
        {
            return itemIconKey switch
            {
                "Coin_0" => CoinIcon,
                "Gem_0" => GemIcon,
                "SkillPotion_0" => SkillPotionIcon,
                _ => GemIcon
            };
        }

        public Sprite GetItemIconFor(int itemId)
        {
            return itemId switch
            {
                Constants.ItemId.RED_KEY => RedKey,
                Constants.ItemId.YELLOW_KEY => YellowKey,
                Constants.ItemId.GREEN_KEY => GreenKey,
                Constants.ItemId.COINS => CoinIcon,
                Constants.ItemId.GEMS => GemIcon,
                Constants.ItemId.SKILL_POTION => SkillPotionIcon,
                Constants.ItemId.CARD => CardIcon,

                _ => GemIcon
            };
        }

        public Sprite GetEventIconFor(string key)
        {
            Debug.Log($"GET ICON FOR {key}");
            return GameEventIconConfig.GetIconFor(key);
        }

        public int GetPointInlineIndex(CurrencyType type)
        {
            return type switch
            {
                CurrencyType.Coins => 0,
                CurrencyType.Gems => 1,
                CurrencyType.SkillPotion => 2,

                CurrencyType.LeaderPassPoint => 7,
                _ => 0,
            };
        }

        public AssetReference GetWarriorsFundAtlasReference() =>
            GameEventIconConfig.WarriorsFundAtlasReference;

        public Sprite GetMeleeOrRangedIcon(bool isMelee)
        {
            if (isMelee)
                return _meleeSprite;
            else
                return _rangedSprite;
        }

        public Sprite GetBoostIconCardsFor(BoostType type)
        {
            return _cardBoosts.FirstOrDefault(x => x.Type == type)?.Icon;
        }
    }

    [Serializable]
    public class GameEventIconConfig
    {
        [Required] public AssetReference WarriorsFundAtlasReference;

        [Required] public Sprite LeaderPassIcon;
        [Required] public Sprite ClassicOfferIcon;
        [Required] public Sprite SacredLegionIcon;


        public Sprite GetIconFor(string key)
        {
            Debug.Log($"GET ICON FOR {key}");

            return key switch
            {
                "Leader Pass" => LeaderPassIcon,
                "Classic Offer" => ClassicOfferIcon,
                "Sacred Legion" => SacredLegionIcon,
                _ => LeaderPassIcon,
            };
        }
    }
    [Serializable]
    internal class CardBoostIconConfig
    {
        public BoostType Type;
        public Sprite Icon;
    }
}