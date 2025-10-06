using System;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Core._DataProviders.UnitDataProvider
{
    public interface IUnitCreationStrategy
    {
        IUnitData CreateUnitData(Faction faction, UnitType type, Skin skin);
    }
    
    public class StandardUnitCreationStrategy : IUnitCreationStrategy
    {
        private readonly UnitDataProviderBase _provider;

        public StandardUnitCreationStrategy(UnitDataProviderBase provider)
        {
            _provider = provider;
        }

        public IUnitData CreateUnitData(Faction faction, UnitType type, Skin skin)
        {
            switch (faction)
            {
                case Faction.Player:
                    return _provider.GetPlayerUnitDataFor(type, skin);
                case Faction.Enemy:
                    return _provider.GetEnemyUnitDataFor(type, skin);
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }
        }
    }

    public class GhostsUnitCreationStrategy : IUnitCreationStrategy
    {
        private readonly UnitDataProviderBase _provider;

        public GhostsUnitCreationStrategy(UnitDataProviderBase provider)
        {
            _provider = provider;
        }

        public IUnitData CreateUnitData(Faction faction, UnitType type, Skin skin) => _provider.GetDecoratedGhostsUnitData(skin);
    }
}