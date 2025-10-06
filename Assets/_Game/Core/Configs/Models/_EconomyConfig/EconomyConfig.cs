using System;
using _Game.Core.Configs.Models._UpgradeItemConfig;

namespace _Game.Core.Configs.Models._EconomyConfig
{
    [Serializable]
    public class EconomyConfig
    {
        public int Id;
        public float CoinPerBattle;
        public int InitialFoodAmount;

        public UpgradeItemConfig FoodProduction;
        public UpgradeItemConfig BaseHealth;
    }
}