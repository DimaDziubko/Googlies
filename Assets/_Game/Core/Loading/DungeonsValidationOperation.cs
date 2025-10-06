using System;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class DungeonsValidationOperation : ILoadingOperation
    {
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;
        public string Description => "Dungeons validation...";
        
        private IDungeonsStateReadonly DungeonsState => _userContainer.State.DungeonsSavegame;

        public DungeonsValidationOperation(
            IUserContainer userContainer, 
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _logger = logger;
        }

        public UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.1f);
            foreach (var dungeon in _userContainer.GameConfig.Dungeons.Values)
            {
                if(!_userContainer.DungeonsStateHandler.HasDungeon(dungeon.Id))
                {
                    var newDungeon = new Dungeon()
                    {
                        Id = dungeon.Id,
                        KeysCount = dungeon.KeysCount,
                        VideosCount = dungeon.VideosCount,
                        DungeonType = dungeon.Type,
                        Level = 1,
                        MaxLevel = 0,
                        LastDungeonDay = DateTime.UtcNow
                    };
                    
                    _userContainer.DungeonsStateHandler.AddDungeon(newDungeon);
                }
            }


            foreach (var dungeon in DungeonsState.Dungeons)
            {
                if (!_userContainer.GameConfig.Dungeons.ContainsKey(dungeon.Id))
                {
                    _userContainer.DungeonsStateHandler.RemoveDungeon(dungeon.Id);
                }
            }
            
            onProgress?.Invoke(1);
            return UniTask.CompletedTask;
        }
    }
}