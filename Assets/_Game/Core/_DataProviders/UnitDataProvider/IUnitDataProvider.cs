using System.Collections.Generic;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Core._DataProviders.UnitDataProvider
{
    public interface IUnitDataProvider
    {
        IUnitData GetDecoratedUnitData(Faction faction, UnitType type, Skin skin);
        IEnumerable<IUnitData> GetAllPlayerUnits();
    }
}