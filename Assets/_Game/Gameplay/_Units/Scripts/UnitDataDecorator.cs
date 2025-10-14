using System.Collections.Generic;
using _Game.Core.Boosts;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Weapon.Scripts;
using Pathfinding.RVO;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public abstract class UnitDataDecorator : IUnitData
    {
        protected readonly IUnitData _unitData;

        protected UnitDataDecorator(IUnitData unitData) =>
            _unitData = unitData;

        public virtual float CoinsPerKill => _unitData.CoinsPerKill;
        public float AttackDistance => _unitData.AttackDistance;
        public float Speed => _unitData.Speed;
        public float AttackPerSecond => _unitData.AttackPerSecond;
        public UnitType Type => _unitData.Type;
        public IWeaponData WeaponData => _unitData.WeaponData;
        public string PrefabKey => _unitData.PrefabKey;
        public IReadOnlyList<CurrencyData> Price => _unitData.Price;
        public int FoodPrice => _unitData.FoodPrice;
        public string Name => _unitData.Name;
        public virtual float Health => _unitData.Health;
        public Sprite Icon => _unitData.Icon;
        public int Layer => _unitData.Layer;
        public int AttackLayer => _unitData.AttackLayer;
        public int AggroLayer => _unitData.AggroLayer;
        public RVOLayer RVOLayer => _unitData.RVOLayer;
        public Skin Skin => _unitData.Skin;
        public virtual float HealthBoost => _unitData.HealthBoost;
        public WarriorObjectsPositionSettings PositionSettings => _unitData.PositionSettings;
        public bool IsPushable => _unitData.IsPushable;
        public bool IsWeaponMelee => WeaponData.WeaponType == WeaponType.SimpleMelee;

    }
}