using System;
using System.Collections.Generic;
using _Game.Core.Boosts;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Gameplay._Weapon.Scripts;
using _Game.UI._StatsPopup._Scripts;
using _Game.UI._UpgradesScreen.Scripts;
using Pathfinding.RVO;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public interface IUnitData
    {
        float CoinsPerKill { get; }
        float AttackDistance { get; }
        float Speed { get; }
        float AttackPerSecond { get; }
        UnitType Type { get; }
        IWeaponData WeaponData { get; }
        string PrefabKey { get; }
        IReadOnlyList<CurrencyData> Price { get; }
        int FoodPrice { get; }
        string Name { get; }
        float Health { get; }
        Sprite Icon { get; }
        int Layer { get; }
        int AttackLayer { get; }
        int AggroLayer { get; }
        RVOLayer RVOLayer { get; }
        Skin Skin { get; }
        float HealthBoost { get; }
        WarriorObjectsPositionSettings PositionSettings { get; }
        bool IsPushable { get; }
        bool IsWeaponMelee { get; }

        StatInfo GetStatInfo(StatType statType)
        {
            switch (statType)
            {
                case StatType.Damage:
                    return new StatInfo()
                    {
                        Type = StatType.Damage,
                        Value = WeaponData.Damage,
                        BoostValue = WeaponData.DamageBoost
                    };
                case StatType.Health:
                    return new StatInfo()
                    {
                        Type = StatType.Health,
                        Value = Health,
                        BoostValue = HealthBoost
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(statType), statType, null);
            }
        }
    }
}