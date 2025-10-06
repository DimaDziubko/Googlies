using System;
using System.Collections.Generic;
using _Game.Core.Boosts;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using UnityEngine;

namespace _Game.UI._UpgradesScreen.Scripts
{
    public class UnitUpgrade : IProduct
    {
        public event Action StateChanged;
        public string Title => $"UnitUpgrade {Type}";

        public UnitType Type => _data.Type;
        public string Name => _data.Name;

        public IReadOnlyList<CurrencyData> Price => _data.Price;

        private IUnitData _data;

        private StatInfo[] _stats;
        private IReadOnlyList<CurrencyData> _price;

        public IReadOnlyList<StatInfo> Stats => _stats;
        public Sprite Icon => _data.Icon;

        public UnitUpgrade(IUnitData data)
        {
            _data = data;

            UpdateStats(data);
        }

        public void SetData(IUnitData data)
        {
            _data = data;
            UpdateStats(data);
            StateChanged?.Invoke();
        }

        private void UpdateStats(IUnitData data)
        {
            _stats = new[]
            {
                _data.GetStatInfo(StatType.Damage),
                _data.GetStatInfo(StatType.Health)
            };
        }

        internal bool GetWeaponType()
        {
            if (_data.WeaponData.WeaponType == WeaponType.SimpleMelee) return true;
            else return false;
        }
    }
}