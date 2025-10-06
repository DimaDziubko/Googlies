using System;
using System.Collections.Generic;
using _Game.Core.Boosts;
using _Game.Core.UserState._State;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.UI.Common.Scripts;

namespace _Game.Core._DataPresenters._UpgradeItem
{
    public class UpgradeItemModel : IProduct
    {
        public event Action Changed;
        public string Title => $"Upgrade item {Type}";
        public float Value => _data.Value;
        public float InitialValue => _data.InitialValue;
        public string Name { get; private set; }
        
        public IReadOnlyList<CurrencyData> Price => new[]
        {
            new CurrencyData
            {
                Type = CurrencyType.Coins,
                Amount = _data.Price
            }
        };
        
        public UpgradeItemType Type { get; private set; }

        private UpgradeItemDynamicData _data;

        public UpgradeItemModel(
            UpgradeItemType type,
            string name)
        {
            Type = type;
            _data = new UpgradeItemDynamicData();
            Name = name;
        }

        public void SetData(UpgradeItemDynamicData newData)
        {
            _data = newData;
            Changed?.Invoke();
        }
    }
}