using _Game.Core.Configs.Models;
using _Game.Core.Configs.Models._FoodBoostConfig;
using _Game.Core.Configs.Models._UpgradeItemConfig;
using _Game.UI._UpgradesScreen.Scripts;

namespace _Game.Core.Configs.Repositories.Economy
{
    public interface IEconomyConfigRepository
    {
        FoodBoostConfig GetFoodBoostConfig();
        UpgradeItemConfig GetConfigForType(UpgradeItemType type);
        int GetInitialFoodAmount();
        float GetInitialAmountFor(UpgradeItemType modelType);
    }
}