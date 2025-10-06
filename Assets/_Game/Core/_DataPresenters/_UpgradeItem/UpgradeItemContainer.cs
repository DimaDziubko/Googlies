using System;
using System.Collections.Generic;
using System.Linq;
using _Game.UI._UpgradesScreen.Scripts;

namespace _Game.Core._DataPresenters._UpgradeItem
{
    public class UpgradeItemContainer
    {
        private readonly Dictionary<UpgradeItemType, UpgradeItemModel> _upgradeItems;

        public UpgradeItemContainer(List<UpgradeItemModel> upgradeItems)
        {
            _upgradeItems = new(2);
            var foodItem = new UpgradeItemModel(UpgradeItemType.FoodProduction, "Food production");
            var baseHealthItem = new UpgradeItemModel(UpgradeItemType.BaseHealth, "Base health");
            _upgradeItems[UpgradeItemType.FoodProduction] = foodItem;
            _upgradeItems[UpgradeItemType.BaseHealth] = baseHealthItem;
        }
        
        public UpgradeItemModel GetItem(UpgradeItemType type)
        {
            if (_upgradeItems.TryGetValue(type, out var item))
            {
                return item;
            }
            throw new KeyNotFoundException($"Upgrade item with type {type} not found.");
        }
        
        public void AddOrUpdateItem(UpgradeItemModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Upgrade item cannot be null.");
            }
            _upgradeItems[item.Type] = item;
        }
        
        public bool RemoveItem(UpgradeItemType type)
        {
            return _upgradeItems.Remove(type);
        }
        
        public bool ContainsItem(UpgradeItemType type)
        {
            return _upgradeItems.ContainsKey(type);
        }
        
        public List<UpgradeItemModel> GetAllItems()
        {
            return _upgradeItems.Values.ToList();
        }
        
        public void Clear()
        {
            _upgradeItems.Clear();
        }
        
        public int Count => _upgradeItems.Count;
    }
}