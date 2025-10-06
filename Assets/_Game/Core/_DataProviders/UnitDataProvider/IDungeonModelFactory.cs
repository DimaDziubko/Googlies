using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;

namespace _Game.Core._DataProviders.UnitDataProvider
{
    public interface IDungeonModelFactory
    {
        IDungeonModel GetModel(DungeonType dungeonType);
        IEnumerable<IDungeonModel> GetModels();
    }

    public class DungeonModelFactory : IDungeonModelFactory, IDisposable 
    {
        private readonly Dictionary<DungeonType, IDungeonModel> _dungeonModels = new();
        
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;
        private IDungeonsStateReadonly Dungeons => _userContainer.State.DungeonsSavegame;

        public DungeonModelFactory(
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _logger = logger;
            _gameInitializer.OnPostInitialization += Initialize;
        }

        private void Initialize()
        {
            foreach (var dungeon in Dungeons.Dungeons)
            {
                _logger.Log("DUNGEONS IS NULL", DebugStatus.Warning);
                var model = new DungeonModel(dungeon, _userContainer.GameConfig.Dungeons[dungeon.Id], _logger);
                _dungeonModels[dungeon.DungeonType] = model;
            }
        }
        
        void IDisposable.Dispose() => _gameInitializer.OnPostInitialization -= Initialize;

        public IDungeonModel GetModel(DungeonType dungeonType) =>
            _dungeonModels.GetValueOrDefault(dungeonType);

        public IEnumerable<IDungeonModel> GetModels() => _dungeonModels.Values;
    }
}