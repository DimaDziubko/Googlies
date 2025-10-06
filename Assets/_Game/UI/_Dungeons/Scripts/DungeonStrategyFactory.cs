using System;
using System.Collections.Generic;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core.GameState;
using _Game.Core.Loading;
using _Game.Core.UserState._State;
using _Game.UI._MainMenu.Scripts;

namespace _Game.UI._Dungeons.Scripts
{
    public class DungeonStrategyFactory
    {
        private readonly Dictionary<DungeonType, IDungeonStrategy> _cachedStrategies = new();

        private readonly IGameStateMachine _state;
        private readonly IDungeonModelFactory _dungeonModelFactory;
        private readonly LoadingOperationFactory _loadingOperationFactory;
        private readonly IMainMenuProvider _menuProvider;

        public DungeonStrategyFactory(
            IGameStateMachine state,
            IDungeonModelFactory dungeonModelFactory,
            LoadingOperationFactory loadingOperationFactory,
            IMainMenuProvider menuProvider)
        {
            _state = state;
            _dungeonModelFactory = dungeonModelFactory;
            _loadingOperationFactory = loadingOperationFactory;
            _menuProvider = menuProvider;
        }
        
        public IDungeonStrategy GetStrategy(DungeonType dungeonType)
        {
            if (_cachedStrategies.TryGetValue(dungeonType, out var strategy))
            {
                return strategy;
            }

            strategy = CreateStrategy(dungeonType);
            _cachedStrategies[dungeonType] = strategy;
            return strategy;
        }

        private IDungeonStrategy CreateStrategy(DungeonType dungeonType)
        {
            switch (dungeonType)
            {
                case DungeonType.ZombieRush:
                    return new ZombieRushStrategy(
                        _state, 
                        _dungeonModelFactory.GetModel(dungeonType),
                        _loadingOperationFactory, _menuProvider);
                case DungeonType.Legends:
                    return new LegendsStrategy();
                case DungeonType.Bosses:
                    return new BossesStrategy();
                default:
                    throw new ArgumentOutOfRangeException(nameof(dungeonType), dungeonType, "Unsupported dungeon type");
            }
        }
    }
}