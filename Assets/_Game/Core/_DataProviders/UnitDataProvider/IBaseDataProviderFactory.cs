using _Game.Core._DataProviders._BaseDataProvider;
using _Game.Gameplay._BattleField.Scripts;

namespace _Game.Core._DataProviders.UnitDataProvider
{
    public interface IBaseDataProviderFactory
    {
        IBaseDataProvider GetProvider(GameMode gameMode);
    }
}