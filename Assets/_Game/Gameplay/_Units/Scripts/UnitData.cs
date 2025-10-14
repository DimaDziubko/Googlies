using _Game.Core.Boosts;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Weapon.Scripts;
using Pathfinding.RVO;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{

    public class UnitData : IUnitData
    {
        public float CoinsPerKill { get; private set; }
        public float AttackDistance { get; private set; }
        public float Speed { get; private set; }
        public float AttackPerSecond { get; private set; }
        public UnitType Type { get; private set; }
        public IWeaponData WeaponData { get; private set; }
        public string PrefabKey { get; private set; }
        public IReadOnlyList<CurrencyData> Price { get; private set; }
        public int FoodPrice { get; private set; }
        public string Name { get; private set; }
        public float Health { get; private set; }
        public Sprite Icon { get; private set; }
        public int Layer { get; private set; }
        public int AttackLayer { get; private set; }
        public int AggroLayer { get; private set; }
        public RVOLayer RVOLayer { get; private set; }
        public Skin Skin { get; private set; }
        public float HealthBoost { get; private set; }
        public WarriorObjectsPositionSettings PositionSettings  { get; private set; }
        public bool IsPushable  { get; private set; }
        public bool IsWeaponMelee => WeaponData.WeaponType == WeaponType.SimpleMelee;

        public class UnitDataBuilder
        {
            float _coinsPerKill = 0;
            float _attackDistance;
            float _speed;
            float _attackPerSecond;
            UnitType _type;
            IWeaponData _weaponData;
            string _prefabKey;
            IReadOnlyList<CurrencyData> _price;
            int _foodPrice;
            string _name;
            float _health;
            Sprite _icon;
            int _layer;
            int _attackLayer;
            int _aggroLayer;
            RVOLayer _rvoLayer;
            Skin _skin = Skin.Ally;
            float _healthBoost = 1;
            WarriorObjectsPositionSettings _positionSettings;
            bool _isPushable;

            public UnitDataBuilder WithCoinsPerKill(float coinsPerKill)
            {
                _coinsPerKill = coinsPerKill;
                return this;
            }

            public UnitDataBuilder WithType(UnitType type)
            {
                _type = type;
                return this;
            }

            public UnitDataBuilder WithAttackDistance(float attackDistance)
            {
                _attackDistance = attackDistance;
                return this;
            }

            public UnitDataBuilder WithSpeed(float speed)
            {
                _speed = speed;
                return this;
            }

            public UnitDataBuilder WithAttackPerSecond(float attackPerSecond)
            {
                _attackPerSecond = attackPerSecond;
                return this;
            }

            public UnitDataBuilder WithWeapon(IWeaponData weaponData)
            {
                _weaponData = weaponData;
                return this;
            }

            public UnitDataBuilder WithPrefabKey(string prefabKey)
            {
                _prefabKey = prefabKey;
                return this;
            }

            public UnitDataBuilder WithPrice(IReadOnlyList<CurrencyData> price)
            {
                _price = price;
                return this;
            }

            public UnitDataBuilder WithFoodPrice(int foodPrice)
            {
                _foodPrice = foodPrice;
                return this;
            }

            public UnitDataBuilder WithNane(string name)
            {
                _name = name;
                return this;
            }

            public UnitDataBuilder WithHealth(float health)
            {
                _health = health;
                return this;
            }

            public UnitDataBuilder WithIcon(Sprite icon)
            {
                _icon = icon;
                return this;
            }

            public UnitDataBuilder WithLayer(int layer)
            {
                _layer = layer;
                return this;
            }

            public UnitDataBuilder WithAttackLayer(int attackLayer)
            {
                _attackLayer = attackLayer;
                return this;
            }

            public UnitDataBuilder WithAggroLayer(int aggroLayer)
            {
                _aggroLayer = aggroLayer;
                return this;
            }

            public UnitDataBuilder WithRVOLayer(RVOLayer rvoLayer)
            {
                _rvoLayer = rvoLayer;
                return this;
            }

            public UnitDataBuilder WithSkin(Skin skin)
            {
                _skin = skin;
                return this;
            }

            public UnitDataBuilder WithHealthBoost(float healthBoost)
            {
                _healthBoost = healthBoost;
                return this;
            }

            public UnitDataBuilder WithObjectsPositionSettings(WarriorObjectsPositionSettings positionSettings)
            {
                _positionSettings = positionSettings;
                return this;
            }
            
            public UnitDataBuilder WithIsPushable(bool isPushable)
            {
                _isPushable = isPushable;
                return this;
            }

            public UnitData Build()
            {
                var unitData = new UnitData
                {
                    CoinsPerKill = _coinsPerKill,
                    AttackDistance = _attackDistance,
                    Speed = _speed,
                    AttackPerSecond = _attackPerSecond,
                    Type = _type,
                    WeaponData = _weaponData,
                    PrefabKey = _prefabKey,
                    Price = _price,
                    FoodPrice = _foodPrice,
                    Name = _name,
                    Health = _health,
                    Icon = _icon,
                    Layer = _layer,
                    AttackLayer = _attackLayer,
                    AggroLayer = _aggroLayer,
                    RVOLayer = _rvoLayer,
                    Skin = _skin,
                    HealthBoost = _healthBoost,
                    PositionSettings = _positionSettings,
                    IsPushable = _isPushable,
                };
                return unitData;
            }
        }
    }
}