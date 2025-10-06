using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Core._DataProviders.UnitDataProvider
{
    public abstract class UnitDataProviderBase
    {
        public abstract IUnitData GetPlayerUnitDataFor(UnitType type, Skin skin);
        public abstract IUnitData GetEnemyUnitDataFor(UnitType type, Skin skin);
        public abstract IUnitData GetDecoratedGhostsUnitData(Skin skin);
    }
}